using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class AddressCreated : IEvent
{
    public required Guid StreamId { get; init; }
    public DateTime Timestamp { get; set; }

    public required int Id { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public required string Country { get; init; }

}
