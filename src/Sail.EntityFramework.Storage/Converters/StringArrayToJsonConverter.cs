
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Sail.EntityFramework.Storage.Converters;

public class StringArrayToJsonConverter() : ValueConverter<List<string>, string>(
    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
    v => (!string.IsNullOrEmpty(v)
        ? JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default)
        : new List<string>())!)
{
    public static readonly StringArrayToJsonConverter Instance = new();
}