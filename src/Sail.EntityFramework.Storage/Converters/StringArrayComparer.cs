using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Sail.EntityFramework.Storage.Converters;

public class StringArrayComparer() : ValueComparer<List<string>>((c1, c2) => c1.SequenceEqual(c2),
    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
    c => c)
{
    public static readonly StringArrayComparer Instance = new();
}