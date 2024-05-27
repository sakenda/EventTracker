﻿using EventTracker.Interfaces;

namespace EventTracker.TestClient2.EventModels;

internal class AddressUpdated : IEvent
{
    public required Guid StreamId { get; set; }
    public DateTime Timestamp { get; set; }

    public required string Street { get; set; }
    public required string City { get; set; }
    public required string PostalCode { get; set; }

}