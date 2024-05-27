using EventTracker.Interfaces;
using System.Collections;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EventTracker;

internal class EventConverter : JsonConverter<IEvent>
{
    public override IEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using (JsonDocument doc = JsonDocument.ParseValue(ref reader))
        {
            JsonElement root = doc.RootElement;

            if (root.TryGetProperty("Type", out JsonElement typeElement))
            {
                string typeName = typeElement.GetString();
                Type eventType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == typeName);

                if (eventType != null && typeof(IEvent).IsAssignableFrom(eventType))
                {
                    return (IEvent)JsonSerializer.Deserialize(root.GetRawText(), eventType, options);
                }
                else
                {
                    throw new NotSupportedException($"Type {typeName} is not supported or does not implement {nameof(IEvent)}.");
                }
            }

            throw new JsonException("Could not determine the type.");
        }
    }

    public override void Write(Utf8JsonWriter writer, IEvent value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Type", value.GetType().FullName);
        WriteObjects(writer, value);
        writer.WriteEndObject();
    }

    private void WriteValue(Utf8JsonWriter writer, string propName, object propValue)
    {
        if (propValue == null)
        {
            if (propName != null)
                writer.WriteNull(propName);
            else
                writer.WriteNullValue();
            return;
        }

        switch (Type.GetTypeCode(propValue.GetType()))
        {
            case TypeCode.Boolean:
                if (propName != null)
                    writer.WriteBoolean(propName, (bool)propValue);
                else
                    writer.WriteBooleanValue((bool)propValue);
                break;
            case TypeCode.Char:
            case TypeCode.String:
                if (propName != null)
                    writer.WriteString(propName, propValue.ToString());
                else
                    writer.WriteStringValue(propValue.ToString());
                break;
            case TypeCode.SByte:
            case TypeCode.Byte:
            case TypeCode.Int16:
            case TypeCode.UInt16:
            case TypeCode.Int32:
            case TypeCode.UInt32:
            case TypeCode.Int64:
            case TypeCode.UInt64:
                if (propName != null)
                    writer.WriteNumber(propName, Convert.ToInt64(propValue));
                else
                    writer.WriteNumberValue(Convert.ToInt64(propValue));
                break;
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Decimal:
                if (propName != null)
                    writer.WriteNumber(propName, Convert.ToDouble(propValue));
                else
                    writer.WriteNumberValue(Convert.ToDouble(propValue));
                break;
            case TypeCode.DateTime:
                if (propName != null)
                    writer.WriteString(propName, ((DateTime)propValue).ToString("o"));
                else
                    writer.WriteStringValue(((DateTime)propValue).ToString("o"));
                break;
            case TypeCode.Object:
                if (propValue is Guid)
                {
                    if (propName != null)
                        writer.WriteString(propName, propValue.ToString());
                    else
                        writer.WriteStringValue(propValue.ToString());
                }
                else if (propValue.GetType().IsEnum)
                {
                    if (propName != null)
                        writer.WriteString(propName, propValue.ToString());
                    else
                        writer.WriteStringValue(propValue.ToString());
                }
                else
                {
                    if (propName != null)
                    {
                        writer.WriteStartObject(propName);
                        WriteObjects(writer, propValue);
                        writer.WriteEndObject();
                    }
                    else
                    {
                        writer.WriteStartObject();
                        WriteObjects(writer, propValue);
                        writer.WriteEndObject();
                    }
                }
                break;
            default:
                throw new NotSupportedException($"Type {propValue.GetType()} is not supported.");
        }
    }

    private void WriteArray(Utf8JsonWriter writer, IEnumerable array, string propName = null)
    {
        if (propName != null)
        {
            writer.WriteStartArray(propName);
        }
        else
        {
            writer.WriteStartArray();
        }

        foreach (var item in array)
        {
            WriteValue(writer, null, item);
        }

        writer.WriteEndArray();
    }

    private void WriteObjects(Utf8JsonWriter writer, object value)
    {
        var type = value.GetType();
        var properties = type.GetProperties();

        foreach (var prop in properties)
        {
            var propValue = prop.GetValue(value);

            if (typeof(IEnumerable).IsAssignableFrom(prop.PropertyType) && prop.PropertyType != typeof(string))
            {
                WriteArray(writer, (IEnumerable)propValue, prop.Name);
            }
            else
            {
                WriteValue(writer, prop.Name, propValue);
            }
        }
    }

}
