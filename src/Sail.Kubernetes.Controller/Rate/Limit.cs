using System.Diagnostics.CodeAnalysis;

namespace Sail.Kubernetes.Controller.Rate;

public struct Limit : IEquatable<Limit>
{
    private readonly double _tokensPerSecond;
    
    public Limit(double perSecond)
    {
        _tokensPerSecond = perSecond;
    }

    public static Limit Max { get; } = new Limit(double.MaxValue);


    public static bool operator ==(Limit left, Limit right)
    {
        return left.Equals(right);
    }


    public static bool operator !=(Limit left, Limit right)
    {
        return !(left == right);
    }


    public double TokensFromDuration(TimeSpan duration)
    {
        var sec = duration.Ticks / TimeSpan.TicksPerSecond * _tokensPerSecond;
        var nsec = duration.Ticks % TimeSpan.TicksPerSecond * _tokensPerSecond;
        return sec + nsec / TimeSpan.TicksPerSecond;
    }

    public TimeSpan DurationFromTokens(double tokens)
    {
        return TimeSpan.FromSeconds(tokens / _tokensPerSecond);
    }
    
    public override bool Equals(object obj)
    {
        return obj is Limit limit && Equals(limit);
    }
    public bool Equals([AllowNull] Limit other)
    {
        return _tokensPerSecond == other._tokensPerSecond;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(_tokensPerSecond);
    }
}