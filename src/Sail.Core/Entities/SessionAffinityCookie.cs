namespace Sail.Core.Entities;

public class SessionAffinityCookie
{
    public string Path { get; set; }
    public string Domain { get; set; }
    public bool HttpOnly { get; set; }
}  