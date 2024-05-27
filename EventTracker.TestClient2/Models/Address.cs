using EventTracker.Interfaces;
using EventTracker.TestClient2.EventModels;

namespace EventTracker.TestClient2.Models;

public class Address : IEventApply
{
    public int Id { get; set; }
    public string Street { get; set; }
    public string City { get; set; }
    public string PostalCode { get; set; }
    public string Country { get; set; }

    public void Apply(IEvent @event)
    {
        switch (@event)
        {
            case AddressCreated created:
                Apply(created);
                break;
            case AddressUpdated updated:
                Apply(updated);
                break;
            default:
                throw new ArgumentException(nameof(@event));
        }
    }

    private void Apply(AddressCreated created)
    {
        Id = created.Id;
        Street = created.Street; 
        City = created.City;
        PostalCode = created.PostalCode;
        Country = created.Country;
    }

    private void Apply(AddressUpdated updated)
    {
        Street = updated.Street;
        City = updated.City;
        PostalCode = updated.PostalCode;
    }

}
