
namespace EventTracker.TestClient2.EventModels;

internal class StudentEnrolledCourses : Event
{
    public required int Id { get; init; }

    public List<string> Courses { get; set; }

    public override int StreamId => Id;

}
