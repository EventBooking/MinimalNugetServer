using System.Collections.Generic;
using System.IO;

namespace MinimalNugetServer.ContentStores
{
	public class LoadNothingContentStore : IContentStore
	{
		private readonly Dictionary<string, string> _contents = new Dictionary<string, string>();

		public void Add( string contentId, string filePath )
		{
			_contents.Add( contentId, filePath );
		}

		public void Clear()
		{
			_contents.Clear();
		}

		public bool TryGetValue( string contentId, out byte[] content )
		{
			if ( !_contents.TryGetValue( contentId, out var fullFilePath ) )
			{
				content = null;
				return false;
			}

			using ( var fs = new FileStream( fullFilePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
			using ( var ms = new MemoryStream() )
			{
				fs.CopyTo( ms );
				content = ms.ToArray();
				return true;
			}
		}
	}
}