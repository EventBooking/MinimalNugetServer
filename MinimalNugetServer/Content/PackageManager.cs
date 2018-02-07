using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using MinimalNugetServer.Models;

namespace MinimalNugetServer.Content
{
	public class PackageManager
	{
		private readonly string _packagesPath;
		private List<PackageInfo> _packages;
		private readonly ContentStore _contentStore;

		public PackageManager( string packagesPath )
		{
			_packagesPath = packagesPath;
			_contentStore = new ContentStore();
			ProcessPackageFiles();
		}

		public IEnumerable<VersionInfo> GetPackageVersions( string id )
		{
			var package = _packages.FirstOrDefault( x => x.Id == id );
			return package?.Versions;
		}

		public byte[] GetContent( string contentId )
		{
			return _contentStore.TryGetValue( contentId, out var content ) ? content : null;
		}

		public IEnumerable<PackageInfo> Search( string searchTerm, int skip, int take )
		{
			var matches = _packages.Where( x => x.Id.IndexOf( searchTerm, StringComparison.OrdinalIgnoreCase ) > -1 )
				.Skip( skip )
				.Take( take )
				.ToList();

			return matches;
		}

		public void AddPackage( string fileName, Stream stream )
		{
			var idVersion = new IdVersion( Path.GetFileNameWithoutExtension( fileName ) );
			var filePath = Path.Combine( _packagesPath, fileName );
			var version = new VersionInfo
			{
				ContentId = ( (uint) filePath.GetHashCode() ).ToString(),
				Version = idVersion.Version
			};

			using ( var fs = new FileStream( filePath, FileMode.Create, FileAccess.Write, FileShare.None ) )
			{
				stream.CopyTo( fs );
			}
			_contentStore.Add( version.ContentId, filePath );
			
			var package = _packages.FirstOrDefault( x => x.Id == idVersion.Id );
			if ( package == null )
			{
				package = new PackageInfo
				{
					Id = idVersion.Id,
					Versions = new List<VersionInfo> { version },
					LatestContentId = version.ContentId,
					LatestVersion = version.Version
				};

				_packages.Add( package );
			}
			else
			{
				// TODO: potential issue here -- how to get the latest version?
				package.Versions = package.Versions.Concat( new[] { version } ).OrderBy( x => x.Version ).ToList();
				package.LatestContentId = package.Versions.Last().ContentId;
				package.LatestVersion = package.Versions.Last().Version;
			}
		}

		private void ProcessPackageFiles()
		{
			var filePaths = Directory.GetFiles( _packagesPath, "*.nupkg", SearchOption.AllDirectories )
				.Where( x => !x.EndsWith( ".symbols.nupkg" ) )
				.ToList();

			var groups = filePaths.Select( filePath =>
				{
					var idVersion = new IdVersion( Path.GetFileNameWithoutExtension( filePath ) );
					return new
					{
						idVersion.Id,
						idVersion.Version,
						FilePath = filePath,
						ContentId = ( (uint) filePath.GetHashCode() ).ToString()
					};
				} )
				.GroupBy( x => x.Id )
				.ToList();

			groups.ForEach( g => g.ToList().ForEach( x => _contentStore.Add( x.ContentId, x.FilePath ) ) );

			_packages = groups.Select( group =>
				{
					// TODO: potential issue here -- how to get the latest version?
					var versions = group.Select( x => new VersionInfo { Version = x.Version, ContentId = x.ContentId } ).OrderBy( x => x.Version ).ToList();
					return new PackageInfo
					{
						Id = group.Key,
						Versions = versions,
						LatestContentId = versions.Last().ContentId,
						LatestVersion = versions.Last().Version
					};
				} )
				.OrderBy( x => x.Id )
				.ToList();
		}

		public string FindContentId( string packageId, string version )
		{
			var package = _packages.FirstOrDefault( x => string.Equals( x.Id, packageId, StringComparison.OrdinalIgnoreCase ) );
			var contentId = package?.Versions.FirstOrDefault( x => x.Version == version )?.ContentId;
			return contentId;
		}
	}
}