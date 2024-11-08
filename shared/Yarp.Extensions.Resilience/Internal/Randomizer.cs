namespace Yarp.Extensions.Resilience.Internal;

internal class Randomizer
{
#if NET6_0_OR_GREATER
    public virtual double NextDouble(double maxValue) => Random.Shared.NextDouble() * maxValue;

    public virtual int NextInt(int maxValue) => Random.Shared.Next(maxValue);
#else
    private static readonly System.Threading.ThreadLocal<Random> _randomInstance = new(() => new Random());
i
    public virtual double NextDouble(double maxValue) => _randomInstance.Value!.NextDouble() * maxValue;

    public virtual int NextInt(int maxValue) => _randomInstance.Value!.Next(maxValue);
#endif
}