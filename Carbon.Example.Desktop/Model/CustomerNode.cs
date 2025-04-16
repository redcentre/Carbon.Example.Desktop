using RCS.Carbon.Licensing.Shared;

namespace Carbon.Example.Desktop.Model;

public sealed class CustomerNode(long id, LicenceCustomer customer) : AppNode(AppNodeType.Customer, id, customer.Name, customer.Id)
{
	public LicenceCustomer Customer { get; } = customer;

	public override object Props => Customer;
}
