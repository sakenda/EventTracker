using EventTracker.Interfaces;
using System.Text.Json.Serialization;

namespace EventTracker.TestClient2.EventModels;

[JsonPolymorphic]
[JsonDerivedType(typeof(StudentCreated), nameof(StudentCreated))]
[JsonDerivedType(typeof(StudentUpdated), nameof(StudentUpdated))]
[JsonDerivedType(typeof(StudentEnrolledCourses), nameof(StudentEnrolledCourses))]
public abstract class Event : IEvent
{
    public abstract int StreamId { get; }

    public DateTime Timestamp { get; set; }

}
