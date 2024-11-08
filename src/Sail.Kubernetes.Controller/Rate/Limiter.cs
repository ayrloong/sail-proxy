namespace Sail.Kubernetes.Controller.Rate;

public class Limiter
{
    private readonly object _sync = new();
    private readonly Limit _limit;
    private readonly TimeProvider _timeProvider;
    private readonly int _burst;
    private double _tokens;
    
    private DateTimeOffset _last;

    private DateTimeOffset _lastEvent;

    public Limiter(Limit limit, int burst, TimeProvider timeProvider = default)
    {
        _limit = limit;
        _burst = burst;
        _timeProvider = timeProvider ?? TimeProvider.System;
    }
    
    public bool Allow()
    {
        return AllowN(_timeProvider.GetUtcNow(), 1);
    }
    
    public bool AllowN(DateTimeOffset now, int number)
    {
        return ReserveImpl(now, number, TimeSpan.Zero).Ok;
    }
    
    public Reservation Reserve()
    {
        return Reserve(_timeProvider.GetUtcNow(), 1);
    }
    
    public Reservation Reserve(DateTimeOffset now, int count)
    {
        return ReserveImpl(now, count, TimeSpan.MaxValue);
    }

    public Task WaitAsync(CancellationToken cancellationToken)
    {
        return WaitAsync(1, cancellationToken);
    }
    
    public async Task WaitAsync(int count, CancellationToken cancellationToken)
    {
        // https://github.com/golang/time/blob/master/rate/rate.go#L226
        int burst = default;
        Limit limit = default;
        lock (_sync)
        {
            burst = _burst;
            limit = _limit;
        }

        if (count > burst && limit != Limit.Max)
        {
            throw new Exception($"rate: Wait(count={count}) exceeds limiter's burst {burst}");
        }

        // Check if ctx is already cancelled
        cancellationToken.ThrowIfCancellationRequested();

        // Determine wait limit
        var waitLimit = limit.DurationFromTokens(count);

        while (true)
        {
            var now = _timeProvider.GetUtcNow();
            var r = ReserveImpl(now, count, waitLimit);
            if (r.Ok)
            {
                var delay = r.DelayFrom(now);
                if (delay > TimeSpan.Zero)
                {
                    await Task.Delay(delay, cancellationToken).ConfigureAwait(false);
                }

                return;
            }

            await Task.Delay(waitLimit, cancellationToken).ConfigureAwait(false);
        }
    }


    private Reservation ReserveImpl(DateTimeOffset now, int number, TimeSpan maxFutureReserve)
    {
        lock (_sync)
        {
            if (_limit == Limit.Max)
            {
                return new Reservation(
                    timeProvider: _timeProvider,
                    limiter: this,
                    ok: true,
                    tokens: number,
                    timeToAct: now);
            }

            var (newNow, last, tokens) = Advance(now);
            now = newNow;

            // Calculate the remaining number of tokens resulting from the request.
            tokens -= number;

            // Calculate the wait duration
            TimeSpan waitDuration = default;
            if (tokens < 0)
            {
                waitDuration = _limit.DurationFromTokens(-tokens);
            }

            // Decide result
            var ok = number <= _burst && waitDuration <= maxFutureReserve;

            // Prepare reservation
            if (ok)
            {
                var reservation = new Reservation(
                    timeProvider: _timeProvider,
                    limiter: this,
                    ok: true,
                    tokens: number,
                    limit: _limit,
                    timeToAct: now.Add(waitDuration));

                _last = newNow;
                _tokens = tokens;
                _lastEvent = reservation.TimeToAct;

                return reservation;
            }
            else
            {
                var reservation = new Reservation(
                    timeProvider: _timeProvider,
                    limiter: this,
                    ok: false,
                    limit: _limit);

                _last = last;

                return reservation;
            }
        }
    }
    
    private (DateTimeOffset newNow, DateTimeOffset newLast, double newTokens) Advance(DateTimeOffset now)
    {
        lock (_sync)
        {
            var last = _last;
            if (now < last)
            {
                last = now;
            }

            // Avoid making delta overflow below when last is very old.
            var maxElapsed = _limit.DurationFromTokens(_burst - _tokens);
            var elapsed = now - last;
            if (elapsed > maxElapsed)
            {
                elapsed = maxElapsed;
            }

            // Calculate the new number of tokens, due to time that passed.
            var delta = _limit.TokensFromDuration(elapsed);
            var tokens = _tokens + delta;
            if (tokens > _burst)
            {
                tokens = _burst;
            }

            return (now, last, tokens);
        }
    }
}