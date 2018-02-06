using System;

namespace MinimalNugetServer.Models
{
	public class VersionInfo
	{
		public Version Version { get; set; }
		public string ContentId { get; set; }
	}
}