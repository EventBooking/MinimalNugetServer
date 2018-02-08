using System.Collections.Generic;

namespace MinimalNugetServer.Models
{
	public class VersionInfo
	{
		public string Version { get; set; }
		public string ContentId { get; set; }
	}

	public class VersionInfoComparer : IComparer<VersionInfo>
	{
		public int Compare( VersionInfo x, VersionInfo y )
		{
			if ( x == null && y == null ) return 0;
			if ( x == null ) return -1;
			if ( y == null ) return 1;

			var xSplit = x.Version.Split( '-' );
			var ySplit = y.Version.Split( '-' );

			// Compare version numbers

			var xVersionNumbers = xSplit[0];
			var yVersionNumbers = ySplit[0];

			if ( xVersionNumbers != yVersionNumbers ) return string.CompareOrdinal( xVersionNumbers, yVersionNumbers );

			// Compare pre-release tags

			var xPre = xSplit.Length > 1 ? xSplit[1] : string.Empty;
			var yPre = ySplit.Length > 1 ? ySplit[1] : string.Empty;

			if ( string.IsNullOrWhiteSpace( xPre ) ) return 1;
			if ( string.IsNullOrWhiteSpace( yPre ) ) return -1;
			return string.CompareOrdinal( xPre, yPre );
		}
	}
}