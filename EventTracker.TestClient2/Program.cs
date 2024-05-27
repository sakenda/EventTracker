using EventTracker;
using EventTracker.TestClient2.EventModels;
using EventTracker.TestClient2.Models;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var eventManager = await EventManager.Initialize(
            "Test", "Events", ".", "TestUser", "Test1234");

        var newStudent = new StudentCreated()
        {
            StreamId = Guid.NewGuid(),
            Id = 1,
            Name = "Michael",
            Lastname = "Jimmy",
            Email = "jimmy@gmail.com",
            DateOfBirth = new DateTime(1986, 7, 12),
        };
        await eventManager.Append(newStudent);

        var newAddress = new AddressCreated
        {
            StreamId = newStudent.StreamId,
            Id = 1,
            Street = "Geistkircher Hof 18",
            City = "St. Ingbert",
            PostalCode = "66386",
            Country = "Germany",
        };
        await eventManager.Append(newAddress);

        var updateStudent = new StudentUpdated()
        {
            StreamId = newStudent.StreamId,
            Lastname = "Conny",
            Email = "conny@gmail.de",
        };
        await eventManager.Append(updateStudent);

        var studentEnrolled = new StudentEnrolledCourses()
        {
            StreamId = newStudent.StreamId,
            Courses = new List<string>()
            {
                "English",
                "Math"
            }
        };
        await eventManager.Append(studentEnrolled);

        var addressUpdated = new AddressUpdated
        {
            StreamId = newAddress.StreamId,
            Street = newAddress.Street,
            PostalCode = newAddress.PostalCode,
            City = "Rohrbach"
        };
        await eventManager.Append(addressUpdated);

        // Read and Build Student
        Student student = await eventManager.GetAppliedEvents<Student>(newStudent.StreamId);
        var json = JsonSerializer.Serialize(student, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);

        

        Console.WriteLine("Done!");
    }
}