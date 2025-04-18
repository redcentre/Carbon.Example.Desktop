using RCS.Carbon.Licensing.Shared;

namespace Carbon.Example.Desktop.Model;

public sealed class LicenceNode(LicenceInfo licence) : AppNode(AppNodeType.Licence, nameof(Licence), licence.Name, licence.Id)
{
	public LicenceInfo Licence { get; } = licence;

	public override object Props => Licence;
}
