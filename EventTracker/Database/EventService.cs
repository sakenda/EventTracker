using EventTracker.Interfaces;
using EventTracker.Models;
using System.Text.Json;

namespace EventTracker.Database;

public class EventService
{
    private readonly EventRepository _repository;

    public EventService(ConnectionString connectionstring)
    {
        _repository = new EventRepository(connectionstring);
    }

    internal async Task EnshureDatabase()
    {
        if (!_repository.CheckDatabase().Result)
        {
            await _repository.CreateDatabase();
        }
    }

    internal async Task AddEvent(IEvent @event, string json)
    {
        await _repository.AddEvent(@event, json);
    }

    internal async Task<List<IEvent>> GetEvents(Guid streamId, JsonSerializerOptions options)
    {
        return await _repository.GetEvents(streamId, options);
    }

}
