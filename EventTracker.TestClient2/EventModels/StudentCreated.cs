
namespace EventTracker.TestClient2.EventModels;

internal class StudentCreated : Event
{
    public required int Id { get; init; }

    public required string Name { get; init; }

    public required string Lastname { get; init; }

    public required string Email { get; set; }

    public required DateTime DateOfBirth { get; init; }

    public override int StreamId => Id;

}
