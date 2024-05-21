using EventTracker.Interfaces;
using EventTracker.TestClient2.EventModels;

namespace EventTracker.TestClient2.Models;

public class Student : IEventApply
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public DateTime DateOfBirth { get; set; }

    public List<string> Courses { get; set; }

    public void Apply(IEvent @event)
    {
        switch (@event)
        {
            case StudentCreated created:
                Apply(created);
                break;
            case StudentUpdated updated:
                Apply(updated);
                break;
            case StudentEnrolledCourses enrolled:
                Apply(enrolled);
                break;
            default:
                throw new ArgumentException(nameof(@event));
        }
    }

    private void Apply(StudentCreated created)
    {
        Id = created.Id;
        Name = created.Name;
        LastName = created.Lastname;
        Email = created.Email;
        DateOfBirth = created.DateOfBirth;
        Courses = new();
    }

    private void Apply(StudentUpdated updated)
    {
        LastName = updated.Lastname;
        Email = updated.Email;
    }

    private void Apply(StudentEnrolledCourses enrolled)
    {
        Courses.AddRange(enrolled.Courses);
    }

}
