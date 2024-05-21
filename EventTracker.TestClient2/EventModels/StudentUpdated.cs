
namespace EventTracker.TestClient2.EventModels;

internal class StudentUpdated : Event
{
    public required int Id { get; init; }

    public required string Email { get; init; }

    public required string Lastname { get; init; }

    public override int StreamId => Id;

}
