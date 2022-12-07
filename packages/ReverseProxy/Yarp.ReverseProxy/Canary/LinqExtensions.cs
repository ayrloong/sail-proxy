using System;
using System.Collections.Generic;
using System.Linq;

namespace Yarp.ReverseProxy.Canary;

public static class LinqExtensions
{
    public static T SelectedWeight<T>(this IEnumerable<T> enumerable, Func<T, float> selector)
    {
        var sum = enumerable.Sum(selector);
        var index = (float)Random.Shared.NextDouble() * sum;
        float currentIndex = 0;
        foreach (var item in from weightedItem in enumerable
                 select new { Value = weightedItem, Weight = selector(weightedItem) })
        {
            currentIndex += item.Weight;

            if (currentIndex >= index)
                return item.Value;
        } 
        return default(T);
    }
}