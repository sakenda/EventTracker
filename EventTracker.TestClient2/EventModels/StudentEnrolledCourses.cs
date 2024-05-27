using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class StudentEnrolledCourses : IEvent
{
    public required Guid StreamId { get; init; }
    public DateTime Timestamp { get; set; }

    public List<string> Courses { get; init; }

}
