namespace Sail.Kubernetes.Controller.Rate;

public class Reservation
{
    private readonly TimeProvider _timeProvider;
    private readonly Limiter _limiter;
    private readonly Limit _limit;
    private readonly double _tokens;

    public Reservation(
        TimeProvider timeProvider,
        Limiter limiter,
        bool ok,
        double tokens = default,
        DateTimeOffset timeToAct = default,
        Limit limit = default)
    {
        _timeProvider = timeProvider;
        _limiter = limiter;
        Ok = ok;
        _tokens = tokens;
        TimeToAct = timeToAct;
        _limit = limit;
    }

    public bool Ok { get; }

    public DateTimeOffset TimeToAct { get; }

    public TimeSpan Delay()
    {
        return DelayFrom(_timeProvider.GetUtcNow());
    }

    public TimeSpan DelayFrom(DateTimeOffset now)
    {
        // https://github.com/golang/time/blob/master/rate/rate.go#L134
        if (!Ok)
        {
            return TimeSpan.MaxValue;
        }

        var delay = TimeToAct - now;
        if (delay < TimeSpan.Zero)
        {
            return TimeSpan.Zero;
        }

        return delay;
    }
}