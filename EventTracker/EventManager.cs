using EventTracker.Database;
using EventTracker.Interfaces;
using EventTracker.Models;
using System.Data.SqlClient;
using System.Text.Json;

namespace EventTracker;

public class EventManager
{
    private readonly EventService _eventService;

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
        string applicationName, string databaseName, string server,
        string userId, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(applicationName, nameof(applicationName));
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName, nameof(databaseName));
        ArgumentException.ThrowIfNullOrWhiteSpace(server, nameof(server));
        ArgumentException.ThrowIfNullOrWhiteSpace(userId, nameof(userId));
        ArgumentException.ThrowIfNullOrWhiteSpace(password, nameof(password));

        var credencials = new SqlCredential(userId, ConnectionString.ToSecureString(password));
        var connectionString = new ConnectionString(applicationName, databaseName, server, credencials);
        return await Initialize(connectionString);
    }

    public static async Task<EventManager> Initialize(ConnectionString connectionString)
    {
        ArgumentNullException.ThrowIfNull(connectionString, nameof(connectionString));

        var result = new EventManager(connectionString);
        await result._eventService.EnshureDatabase();
        return result;
    }

    public async Task Append(IEvent @event, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));
        ArgumentNullException.ThrowIfNull(@event, nameof(@event));

        @event.Timestamp = DateTime.Now;
        var json = JsonSerializer.Serialize(@event, options);

        await _eventService.AddEvent(@event, json);
    }

    public async Task<List<IEvent>> GetEvents(int streamId, JsonSerializerOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (streamId <= 0)
        {
            return default;
        }

        var result = await _eventService.GetEvents(streamId, options);
        return result;
    }

    public async Task<T> GetAppliedEvents<T>(int streamId, JsonSerializerOptions options) where T : IEventApply, new()
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        if (streamId <= 0)
        {
            return default(T);
        }

        var events = await _eventService.GetEvents(streamId, options);
        var item = new T();

        foreach (var @event in events)
        {
            item.Apply(@event);
        }

        return item;
    }

}
