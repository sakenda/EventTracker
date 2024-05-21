using EventTracker.Interfaces;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventTracker.TestClient2.EventModels;

public class StudentEventsConverter : JsonConverter<IEvent>
{
    public override IEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("Type", out JsonElement typeElement))
            {
                string type = typeElement.GetString();

                switch (type)
                {
                    case nameof(StudentCreated):
                        return JsonSerializer.Deserialize<StudentCreated>(root.GetRawText(), options);
                    case nameof(StudentUpdated):
                        return JsonSerializer.Deserialize<StudentUpdated>(root.GetRawText(), options);
                    case nameof(StudentEnrolledCourses):
                        return JsonSerializer.Deserialize<StudentEnrolledCourses>(root.GetRawText(), options);
                    default:
                        throw new NotSupportedException($"Type {type} is not supported.");
                }
            }

            throw new JsonException("Could not determine the type of the animal.");
        }
    }

    public override void Write(Utf8JsonWriter writer, IEvent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();

        if (value is StudentCreated created)
        {
            writer.WriteString("Type", nameof(StudentCreated));
            writer.WriteNumber(nameof(StudentCreated.Id), created.Id);
            writer.WriteString(nameof(StudentCreated.Name), created.Name);
            writer.WriteString(nameof(StudentCreated.Lastname), created.Lastname);
            writer.WriteString(nameof(StudentCreated.Email), created.Email);
            writer.WriteString(nameof(StudentCreated.DateOfBirth), created.DateOfBirth);
        }
        else if (value is StudentUpdated updated)
        {
            writer.WriteString("Type", nameof(StudentUpdated));
            writer.WriteNumber(nameof(StudentUpdated.Id), updated.Id);
            writer.WriteString(nameof(StudentUpdated.Lastname), updated.Lastname);
            writer.WriteString(nameof(StudentUpdated.Email), updated.Email);
        }
        else if (value is StudentEnrolledCourses enrolled)
        {
            writer.WriteString("Type", nameof(StudentEnrolledCourses));
            writer.WriteNumber(nameof(StudentEnrolledCourses.Id), enrolled.Id);
            writer.WritePropertyName(nameof(StudentEnrolledCourses.Courses));
            writer.WriteStartArray();
            foreach (var course in enrolled.Courses)
            {
                writer.WriteStringValue(course);
            }
            writer.WriteEndArray();
        }
        else
        {
            throw new NotSupportedException($"Type {value.GetType()} is not supported.");
        }

        writer.WriteEndObject();
    }
}
