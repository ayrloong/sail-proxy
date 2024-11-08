using Yarp.Extensions.Resilience.Internal;

namespace Yarp.Extensions.Resilience.Utilities;

internal static class RoutingHelper
{
    public static T SelectByWeight<T>(this IEnumerable<T> endpoints, Func<T, double> weightProvider, Randomizer randomizer)
    {
        var accumulatedProbability = 0d;
        var weightSum = endpoints.Sum(weightProvider);

        var randomPercentageValue = randomizer.NextDouble(weightSum);
        foreach (var endpoint in endpoints)
        {
            var weight = weightProvider(endpoint);

            if (randomPercentageValue <= weight + accumulatedProbability)
            {
                return endpoint;
            }

            accumulatedProbability += weight;
        }

        throw new InvalidOperationException(
            $"The item cannot be selected because the weights are not correctly calculated.");
    }
}