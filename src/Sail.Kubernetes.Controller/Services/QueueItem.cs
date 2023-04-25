using Sail.Kubernetes.Controller.Dispatching;

namespace Sail.Kubernetes.Controller.Services;

public struct QueueItem : IEquatable<QueueItem>
{
    public QueueItem(string change, IDispatchTarget dispatchTarget)
    {
        Change = change;
        DispatchTarget = dispatchTarget;
    }

    public string Change { get; }
    public IDispatchTarget DispatchTarget { get; }


    public override bool Equals(object obj)
    {
        return obj is QueueItem item && Equals(item);
    }

    public bool Equals(QueueItem other)
    {
        return Change.Equals(other.Change, StringComparison.Ordinal) &&
               EqualityComparer<IDispatchTarget>.Default.Equals(DispatchTarget, other.DispatchTarget);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Change, DispatchTarget);
    }

    public static bool operator ==(QueueItem left, QueueItem right)
    {
        return left.Equals(right);

    }

    public static bool operator !=(QueueItem left, QueueItem right)
    {
        return !(left == right);
    }
}