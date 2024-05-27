using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class AddressUpdated : IEvent
{
    public required Guid StreamId { get; init; }
    public DateTime Timestamp { get; set; }

    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }

}
