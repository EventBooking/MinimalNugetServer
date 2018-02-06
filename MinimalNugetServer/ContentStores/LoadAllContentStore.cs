using System.Collections.Generic;
using System.IO;

namespace MinimalNugetServer.ContentStores
{
	public class LoadAllContentStore : IContentStore
	{
		private readonly Dictionary<string, byte[]> _contents = new Dictionary<string, byte[]>();

		public void Add( string contentId, string filePath )
		{
			using ( var fs = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read ) )
			using ( var ms = new MemoryStream() )
			{
				fs.CopyTo( ms );
				_contents.Add( contentId, ms.ToArray() );
			}
		}

		public void Clear()
		{
			_contents.Clear();
		}

		public bool TryGetValue( string contentId, out byte[] content )
		{
			return _contents.TryGetValue( contentId, out content );
		}
	}
}