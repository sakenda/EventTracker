namespace EventTracker.Interfaces;

public interface IEvent
{
    public int StreamId { get; }

    public DateTime Timestamp { get; set; }

}
