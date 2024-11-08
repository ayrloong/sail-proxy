namespace Sail.Core.Entities;

public enum HeaderMatchMode
{
    ExactHeader,
    HeaderPrefix,
    Contains,
    NotContains,
    Exists,
    NotExists
}