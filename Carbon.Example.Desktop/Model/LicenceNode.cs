using RCS.Carbon.Licensing.Shared;

namespace Carbon.Example.Desktop.Model;

public sealed class LicenceNode(long id, LicenceInfo licence) : AppNode(AppNodeType.Licence, id, licence.Name, licence.Id)
{
	public LicenceInfo Licence { get; } = licence;

	public override object Props => Licence;
}
