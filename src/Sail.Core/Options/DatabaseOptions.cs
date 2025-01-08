namespace Sail.Core.Options;

public class DatabaseOptions
{
    public const string Name = "Database";
    public string DatabaseName { get; set; }
    public string ConnectionString { get; set; }
}