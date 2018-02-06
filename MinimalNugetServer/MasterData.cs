using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Configuration;
using MinimalNugetServer.ContentStores;
using MinimalNugetServer.Models;

namespace MinimalNugetServer
{
	public class MasterData
	{
		private readonly string _packagesPath;
		private List<PackageInfo> _packages;
		private readonly IContentStore _contentStore;

		public MasterData( IConfiguration nugetConfig )
		{
			_packagesPath = nugetConfig["packages"];
			_contentStore = new LoadAllContentStore();
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

		private void ProcessPackageFiles()
		{
			var filePaths = Directory.GetFiles( _packagesPath, "*.nupkg", SearchOption.AllDirectories )
				.Where( x => !x.EndsWith( ".symbols.nupkg" ) )
				.ToList();

			var groups = filePaths.Select( path =>
				{
					var idVersion = new IdVersion( Path.GetFileNameWithoutExtension( path ) );
					return new
					{
						idVersion.Id,
						idVersion.Version,
						FilePath = path,
						ContentId = ( (uint) path.GetHashCode() ).ToString()
					};
				} )
				.GroupBy( x => x.Id )
				.ToList();

			groups.ForEach( g => g.ToList().ForEach( x => _contentStore.Add( x.ContentId, x.FilePath ) ) );

			_packages = groups.Select( group =>
				{
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

		public string FindContentId( string packageId, Version version )
		{
			var package = _packages.FirstOrDefault( x => string.Equals( x.Id, packageId, StringComparison.OrdinalIgnoreCase ) );
			var contentId = package?.Versions.FirstOrDefault( x => x.Version == version )?.ContentId;
			return contentId;
		}
	}
}