using System;
using System.Text.RegularExpressions;

namespace MinimalNugetServer.Models
{
	public class IdVersion
	{
		private static readonly Regex VersionPattern = new Regex( @"[0-9]+\.[0-9]+\.[0-9]+(?:\-[A-Za-z0-9\-\.]+)?$", RegexOptions.Compiled );

		public string Id { get; private set; }
		public string Version { get; private set; }

		public IdVersion( string filename )
		{
			var id = VersionPattern.Split( filename )[0].TrimEnd( '.' );
			Id = id;
			
			var version = VersionPattern.Match( filename ).Value;
			Version = version;
		}
	}
}