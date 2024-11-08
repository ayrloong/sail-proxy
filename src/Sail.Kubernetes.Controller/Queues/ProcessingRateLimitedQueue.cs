using Sail.Kubernetes.Controller.Rate;

namespace Sail.Kubernetes.Controller.Queues;

public class ProcessingRateLimitedQueue<TItem>(double perSecond, int burst) : WorkQueue<TItem>
{
    private readonly Limiter _limiter = new(new Limit(perSecond), burst);

    protected override Task OnGetAsync(CancellationToken cancellationToken)
    {
        var delay = _limiter.Reserve().Delay();
        return Task.Delay(delay, cancellationToken);
    }
}