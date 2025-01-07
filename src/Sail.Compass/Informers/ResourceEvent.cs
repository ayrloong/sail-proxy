namespace Sail.Compass.Informers;

public struct ResourceEvent<TResource>
{
    public ResourceEvent(EventType eventType, TResource value, TResource oldValue = default)
    {
        EventType = eventType;
        Value = value;
        OldValue = oldValue;
    }

    public EventType EventType { get; }
    public TResource OldValue { get; }
    public TResource Value { get; }
}

public enum EventType
{
    Unknown = 0,
    Created = 1,
    Updated = 2,
    Deleted = 3,
    List = 4
}