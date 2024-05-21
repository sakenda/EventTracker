using EventTracker;
using EventTracker.TestClient2.EventModels;
using EventTracker.TestClient2.Models;
using System.Text.Json;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var eventManager = await EventManager.Initialize(
            "Test", "Events", "(localdb)\\MSSQLLocalDB", "TestUser", "Test1234");

        var options = new JsonSerializerOptions
        {
            Converters = { new StudentEventsConverter() },
            WriteIndented = true
        };

        int studentId = 1;

        if (false)
        {
            var newStudent = new StudentCreated()
            {
                Id = studentId,
                Name = "Michael",
                Lastname = "Jimmy",
                Email = "jimmy@gmail.com",
                DateOfBirth = new DateTime(1986, 7, 12)
            };
            await eventManager.Append(newStudent, options);

            var updateStudent = new StudentUpdated()
            {
                Id = studentId,
                Lastname = "Conny",
                Email = "conny@gmail.de",
            };
            await eventManager.Append(updateStudent, options);

            var studentEnrolled = new StudentEnrolledCourses()
            {
                Id = studentId,
                Courses = new List<string>()
                {
                    "English",
                    "Math"
                }
            };
            await eventManager.Append(studentEnrolled, options);
        }

        // Student found
        Student student = await eventManager.GetAppliedEvents<Student>(studentId, options);
        var json = JsonSerializer.Serialize(student, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(json);

        if (false)
        {
            // Create another Student
            var newStudent2 = new StudentCreated()
            {
                Id = 2,
                Name = student.Name,
                Lastname = student.LastName,
                Email = student.Email,
                DateOfBirth = student.DateOfBirth
            };
            var newStudentEnrolled = new StudentEnrolledCourses() { Id = newStudent2.Id, Courses = student.Courses };
            await eventManager.Append(newStudent2, options);
            await eventManager.Append(newStudentEnrolled, options);
        }

        // If Student is not found
        Student noStudent = await eventManager.GetAppliedEvents<Student>(0, options);
        var jsonNoStudent = JsonSerializer.Serialize(noStudent, new JsonSerializerOptions { WriteIndented = true });
        Console.WriteLine(jsonNoStudent);

        Console.WriteLine("Done!");
    }
}