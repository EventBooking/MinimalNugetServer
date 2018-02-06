using System;
using System.Text.RegularExpressions;

namespace MinimalNugetServer.Models
{
	public class IdVersion
	{
		private static readonly Regex VersionPattern = new Regex( @"[0-9]+\.[0-9]+\.[0-9]+(?:\-[A-Za-z0-9\-\.]+)?$", RegexOptions.Compiled );

		public string Id { get; private set; }
		public Version Version { get; private set; }

		public IdVersion( string filename )
		{
			Id = filename;

			var version = VersionPattern.Match( filename ).Value;
			Version = new Version( version );
		}
	}
}