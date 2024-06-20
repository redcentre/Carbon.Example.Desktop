using System;
using RCS.Carbon.Shared;

namespace Carbon.Example.Desktop;

#pragma warning disable CS8618
/// <summary>
/// Data associated with the blob contents of a single container (job).
/// </summary>
public sealed class JobContents
{
	public string Name { get; set; }
	public DateTime? LastModified { get; set; }
	public string PublicAccess { get; set; }
	public MetaData[] Metadata { get; set; }
	public Vartree[] Vartrees { get; set; }
	public Dashboard[] Dashboards { get; set; }
	public VDir RootVDir { get; set; }
}

public sealed class Vartree
{
	public string Name { get; set; }
	public long? Bytes { get; set; }
	public DateTime? Modified { get; set; }
}

public sealed class Dashboard
{
	public string Name { get; set; }
	public long? Bytes { get; set; }
	public DateTime? Modified { get; set; }
	public MetaData[] Metadata { get; set; }
}

public sealed class VDir
{
	public string Name { get; set; }
	public Blob[] Blobs { get; set; }
	public VDir[] VDirs { get; set; }
}

public sealed class Blob
{
	public string Name { get; set; }
	public long? Bytes { get; set; }
	public DateTime? Modified { get; set; }
	public MetaData[] Metadata { get; set; }
}
