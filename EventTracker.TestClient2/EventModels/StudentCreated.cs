using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class StudentCreated : IEvent
{
    public required Guid StreamId { get; init; }
    public DateTime Timestamp { get; set; }

    public required int Id { get; set; }
    public required string Name { get; init; }
    public required string Lastname { get; init; }
    public required string Email { get; set; }
    public required DateTime DateOfBirth { get; init; }

}
