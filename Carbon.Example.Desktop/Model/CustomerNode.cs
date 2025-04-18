using RCS.Carbon.Licensing.Shared;

namespace Carbon.Example.Desktop.Model;

public sealed class CustomerNode(LicenceCustomer customer) : AppNode(AppNodeType.Customer, nameof(Customer), customer.Name, customer.Id)
{
	public LicenceCustomer Customer { get; } = customer;

	public override object Props => Customer;
}
