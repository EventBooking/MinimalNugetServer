using System.Collections.Generic;

namespace MinimalNugetServer.Models
{
	public class PackageInfo
	{
		public string Id { get; set; }
		public string LatestVersion { get; set; }
		public string LatestContentId { get; set; }
		public List<VersionInfo> Versions { get; set; }
	}
}