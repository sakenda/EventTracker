using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class StudentCreated : IEvent
{
    public required Guid StreamId { get; init; }
    public DateTime Timestamp { get; set; }

    public required int Id { get; init; }
    public required string Name { get; init; }
    public required string Lastname { get; init; }
    public required string Email { get; init; }
    public required DateTime DateOfBirth { get; init; }

}
