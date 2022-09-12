using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop
{
	public sealed class VarMeta
	{
#pragma warning disable CS8618     // Empty ctor required for JSON serialization                                                   
		public VarMeta()
		{
		}
#pragma warning restore CS8618

		public VarMeta(GenNode[] nodes, MetaData[] metadata)
		{
			Nodes = nodes;
			Metadata = metadata;
		}

		public GenNode[] Nodes { get; set; }
		public MetaData[] Metadata { get; set; }
	}
}
