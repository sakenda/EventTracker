using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class StudentUpdated : IEvent
{
    public required Guid StreamId { get; init; }
    public DateTime Timestamp { get; set; }

    public required string Email { get; init; }
    public required string Lastname { get; init; }

}
