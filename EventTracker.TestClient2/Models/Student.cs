using EventTracker.Interfaces;
using EventTracker.TestClient2.EventModels;

namespace EventTracker.TestClient2.Models;

public class Student : IEventApply
{
    public Guid StreamId { get; set; }

    public int Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime DateOfBirth { get; set; }
    public Address Address { get; set; }
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
            case AddressCreated created:
                Address.Apply(created);
                break;
            case AddressUpdated updated:
                Address.Apply(updated);
                break;
            default:
                throw new ArgumentException(nameof(@event));
        }
    }

    private void Apply(StudentCreated created)
    {
        StreamId = created.StreamId;

        Id = created.Id;
        Name = created.Name;
        LastName = created.Lastname;
        Email = created.Email;
        DateOfBirth = created.DateOfBirth;
        Courses = new();
        Address = new();
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
