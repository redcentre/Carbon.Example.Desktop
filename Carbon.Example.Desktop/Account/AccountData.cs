using System;

namespace Carbon.Example.Desktop
{
	/// <summary>
	/// Information about one storage account which corresponds to a customer.
	/// </summary>
	public sealed class AccountData
	{
#pragma warning disable CS8618		// Empty ctor required for JSON serialization
		public AccountData()
		{
		}
#pragma warning restore CS8618

		public AccountData(string name, DateTime? creationTime, string id, string kind, string region, DateTime? key1CreationTime, string accessTier, bool? allowSharedKeyAccess, bool? enableHttpsTrafficOnly, string minimumTlsVersion, string connectString)
		{
			Name = name;
			CreationTime = creationTime;
			Id = id;
			Kind = kind;
			Region = region;
			Key1CreationTime = key1CreationTime;
			AccessTier = accessTier;
			AllowSharedKeyAccess = allowSharedKeyAccess;
			EnableHttpsTrafficOnly = enableHttpsTrafficOnly;
			MinimumTlsVersion = minimumTlsVersion;
			ConnectString = connectString;
		}

		public string Name { get; set; }
		public DateTime? CreationTime { get; set; }
		public string Id { get; set; }
		public string Kind { get; set; }
		public string Region { get; set; }
		public DateTime? Key1CreationTime { get; set; }
		public string AccessTier { get; set; }
		public bool? AllowSharedKeyAccess { get; set; }
		public bool? EnableHttpsTrafficOnly { get; set; }
		public string MinimumTlsVersion { get; set; }
		public string ConnectString { get; set; }
	}
}
