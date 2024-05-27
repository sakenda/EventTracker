using EventTracker.Database;
using EventTracker.Interfaces;
using EventTracker.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace EventTracker;

public class EventManager
{
    private readonly EventService _eventService;
    private readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        Converters = { new EventConverter() },
        WriteIndented = true,
    };

    public ConnectionString Connectionstring { get; init; }

    private EventManager(ConnectionString connectionstring)
    {
        ArgumentNullException.ThrowIfNull(nameof(connectionstring), nameof(connectionstring));

        if (!connectionstring.IsValid())
        {
            throw new ArgumentNullException(nameof(connectionstring));
        }

        Connectionstring = connectionstring;
        _eventService = new EventService(connectionstring);
    }

    public static async Task<EventManager> Initialize(
        string tableName, string applicationName,
        string databaseName, string server,
        string userId, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationName, nameof(applicationName));
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName, nameof(databaseName));
        ArgumentException.ThrowIfNullOrWhiteSpace(server, nameof(server));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var credencials = new SqlCredential(userId, ConnectionString.ToSecureString(password));
        var connectionString = new ConnectionString(tableName, applicationName, databaseName, server, credencials);
        return await Initialize(connectionString);
    }

    public static async Task<EventManager> Initialize(ConnectionString connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));

        var result = new EventManager(connectionString);
        await result._eventService.EnshureDatabase();
        return result;
    }

    public async Task Append(IEvent @event)
    {
        ArgumentNullException.ThrowIfNull(@event, nameof(@event));

        @event.Timestamp = DateTime.Now;
        var json = JsonSerializer.Serialize(@event, _serializerOptions);

        await _eventService.AddEvent(@event, json);
    }

    public async Task<List<IEvent>> GetEvents(Guid streamId)
    {
        if (streamId == Guid.Empty)
        {
            return default;
        }

        var result = await _eventService.GetEvents(streamId, _serializerOptions);
        return result;
    }

    public async Task<T> GetAppliedEvents<T>(Guid streamId) where T : IEventApply, new()
    {
        if (streamId == Guid.Empty)
        {
            return default(T);
        }

        var events = await _eventService.GetEvents(streamId, _serializerOptions);
        var item = new T();

        foreach (var @event in events)
        {
            item.Apply(@event);
        }

        return item;
    }

}
