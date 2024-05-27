namespace EventTracker.Interfaces;

public interface IEvent
{
    public Guid StreamId { get; }

    public DateTime Timestamp { get; set; }

}
