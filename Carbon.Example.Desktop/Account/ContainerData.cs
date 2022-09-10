using System;
using System.Linq;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop
{
	/// <summary>
	/// Data associated with a single container (job).
	/// </summary>
	public sealed class ContainerData
	{
#pragma warning disable CS8618     // Empty ctor required for JSON serialization                                                   
		public ContainerData()
		{
		}
#pragma warning restore CS8618

		public ContainerData(string name, DateTime? lastModified, string publicAccess, MetaData[] metadata)
		{
			Name = name;
			LastModified = lastModified;
			PublicAccess = publicAccess;
			Metadata = metadata?.Count() > 0 ? metadata.ToArray() : null;
		}

		public string Name { get; set; }
		public DateTime? LastModified { get; set; }
		public string PublicAccess { get; set; }
		public MetaData[] Metadata { get; set; }
	}
}
