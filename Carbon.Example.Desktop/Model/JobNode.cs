using RCS.Carbon.Licensing.Shared;

namespace Carbon.Example.Desktop.Model;

public sealed class JobNode(LicenceJob job) : AppNode(AppNodeType.Job, nameof(Job), job.Name, job.Id)
{
	public LicenceJob Job { get; } = job;

	public CustomerNode CustomerParentNode => (CustomerNode)Parent!;

	public override object Props => Job;
}
