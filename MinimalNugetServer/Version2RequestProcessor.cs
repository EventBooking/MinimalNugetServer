using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Microsoft.AspNetCore.Http;
using MinimalNugetServer.Config;
using MinimalNugetServer.Models;

namespace MinimalNugetServer
{
	public class Version2RequestProcessor
	{
		private readonly MasterData _masterData;

		public Version2RequestProcessor( MasterData masterData )
		{
			_masterData = masterData;
		}

		public async Task ProcessDownload( HttpContext context )
		{
			var contentId = context.Request.Path.ToString().TrimStart( '/' );
			var content = _masterData.GetContent( contentId );

			if ( content == null )
			{
				context.Response.StatusCode = 404;
				return;
			}

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/octet-stream";
			await context.Response.Body.WriteAsync( content, 0, content.Length );
		}

		public async Task ProcessSearch( HttpContext context )
		{
			var query = context.Request.Query.ToDictionary( x => x.Key, x => x.Value.First() );

			var searchTerm = query["searchTerm"]?.Trim( '\'' ) ?? string.Empty;

			if ( !int.TryParse( query["$skip"] ?? string.Empty, out var skip )
			     || !int.TryParse( query["$top"] ?? string.Empty, out var take ) )
			{
				context.Response.StatusCode = 400;
				return;
			}

			var packages = _masterData.Search( searchTerm, skip, take ).ToList();

			var doc = new XElement(
				XmlElements.Feed,
				new XAttribute( XmlElements.Baze, XmlNamespaces.Baze ),
				new XAttribute( XmlElements.M, XmlNamespaces.M ),
				new XAttribute( XmlElements.D, XmlNamespaces.D ),
				new XAttribute( XmlElements.Georss, XmlNamespaces.Georss ),
				new XAttribute( XmlElements.Gml, XmlNamespaces.Gml ),
				new XElement( XmlElements.MCount, packages.Count().ToString() ),
				packages.Select( pkg =>
					new XElement(
						XmlElements.Entry,
						new XElement( XmlElements.Id,
							$"{context.Request.Scheme}://{context.Request.Host}/v2/Packages(Id='{pkg.Id}',Version='{pkg.LatestVersion}')" ),
						new XElement(
							XmlElements.Content,
							new XAttribute( "type", "application/zip" ),
							new XAttribute( "src", $"{context.Request.Scheme}://{context.Request.Host}/v2/Download/{pkg.LatestContentId}" )
						),
						new XElement(
							XmlElements.MProperties,
							new XElement( XmlElements.DId, pkg.Id ),
							new XElement( XmlElements.DVersion, pkg.LatestVersion )
						)
					)
				).ToList()
			);

			var bytes = Encoding.UTF8.GetBytes( doc.ToString( SaveOptions.DisableFormatting ) );

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/xml";
			await context.Response.Body.WriteAsync( bytes, 0, bytes.Length );
		}

		public async Task ProcessPackages( HttpContext context )
		{
			var path = Uri.UnescapeDataString( context.Request.Path.Value );

			var start = path.IndexOf( '(' );
			if ( start == -1 )
			{
				context.Response.StatusCode = 400;
				return;
			}

			start++;

			var end = path.IndexOf( ')', start );
			if ( end == -1 )
			{
				context.Response.StatusCode = 400;
				return;
			}

			string id = null;
			string version = null;

			var parts = path.Substring( start, end - start ).Split( ',' );

			foreach ( var part in parts )
			{
				var kv = part.Split( new[] { '=' }, 2 );

				if ( string.Equals( kv[0], "id", StringComparison.OrdinalIgnoreCase ) )
					id = kv[1].Trim( '\'' );
				else if ( string.Equals( kv[0], "version", StringComparison.OrdinalIgnoreCase ) )
					version = kv[1].Trim( '\'' );
			}

			if ( string.IsNullOrWhiteSpace( id ) || version == null )
			{
				context.Response.StatusCode = 400;
				return;
			}

			var contentId = _masterData.FindContentId( id, version );

			if ( contentId == null )
			{
				context.Response.StatusCode = 404;
				return;
			}

			var doc = new XElement( XmlElements.Entry,
				new XAttribute( XmlElements.Baze, XmlNamespaces.Baze ),
				new XAttribute( XmlElements.M, XmlNamespaces.M ),
				new XAttribute( XmlElements.D, XmlNamespaces.D ),
				new XAttribute( XmlElements.Georss, XmlNamespaces.Georss ),
				new XAttribute( XmlElements.Gml, XmlNamespaces.Gml ),
				new XElement( XmlElements.Id, $"{context.Request.Scheme}://{context.Request.Host}/v2/Packages(Id='{id}',Version='{version}')" ),
				new XElement(
					XmlElements.Content,
					new XAttribute( "type", "application/zip" ),
					new XAttribute( "src", $"{context.Request.Scheme}://{context.Request.Host}/v2/Download/{contentId}" )
				),
				new XElement(
					XmlElements.MProperties,
					new XElement( XmlElements.DId, id ),
					new XElement( XmlElements.DVersion, version )
				)
			);

			var bytes = Encoding.UTF8.GetBytes( doc.ToString( SaveOptions.DisableFormatting ) );

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/xml";
			await context.Response.Body.WriteAsync( bytes, 0, bytes.Length );
		}

		public async Task FindPackage( HttpContext context )
		{
			var strings = context.Request.Query["id"];
			if ( strings.Count == 0 )
			{
				context.Response.StatusCode = 400;
				return;
			}

			var id = strings[0].Trim( '\'' );
			if ( id.Length == 0 )
			{
				context.Response.StatusCode = 400;
				return;
			}

			var versions = _masterData.GetPackageVersions( id )?.ToList() ?? new List<VersionInfo>();

			var doc = new XElement(
				XmlElements.Feed,
				new XAttribute( XmlElements.Baze, XmlNamespaces.Baze ),
				new XAttribute( XmlElements.M, XmlNamespaces.M ),
				new XAttribute( XmlElements.D, XmlNamespaces.D ),
				new XAttribute( XmlElements.Georss, XmlNamespaces.Georss ),
				new XAttribute( XmlElements.Gml, XmlNamespaces.Gml ),
				new XElement( XmlElements.MCount, versions.Count.ToString() ),
				versions.Select( x =>
					new XElement(
						XmlElements.Entry,
						new XElement( XmlElements.Id, $"{context.Request.Scheme}://{context.Request.Host}/v2/Packages(Id='{id}',Version='{x.Version}')" ),
						new XElement(
							XmlElements.Content,
							new XAttribute( "type", "application/zip" ),
							new XAttribute( "src", $"{context.Request.Scheme}://{context.Request.Host}/v2/Download/{x.ContentId}" )
						),
						new XElement(
							XmlElements.MProperties,
							new XElement( XmlElements.DId, id ),
							new XElement( XmlElements.DVersion, x.Version ),
							new XElement( XmlElements.DDescription, "Package Description" )
						)
					)
				)
			);

			var bytes = Encoding.UTF8.GetBytes( doc.ToString( SaveOptions.DisableFormatting ) );

			context.Response.StatusCode = 200;
			context.Response.ContentType = "application/xml";
			await context.Response.Body.WriteAsync( bytes, 0, bytes.Length );
		}
	}
}