using System.ComponentModel.DataAnnotations;

namespace Sail.Core.Options;

public class DatabaseOptions
{
    public const string Name = "Database";
    [Required] public string ConnectionString { get; set; }
}