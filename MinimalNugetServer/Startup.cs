using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MinimalNugetServer.Content;

namespace MinimalNugetServer
{
	public class Startup
	{
		private readonly IConfiguration _config;

		public Startup( IConfiguration config )
		{
			_config = config;
		}

		public void ConfigureServices( IServiceCollection services )
		{
		}

		public void Configure( IApplicationBuilder app )
		{
			var masterData = new PackageManager( _config["nuget:packages"] );
			var requestProcessor = new RequestProcessor( masterData );

			app.Map( "/v2/Download", builder => builder.Run( requestProcessor.ProcessDownload ) );
			app.Map( "/v2/Search()", builder => builder.Run( requestProcessor.ProcessSearch ) );
			app.MapWhen( x => x.Request.Path.ToString().StartsWith( "/v2/Packages(" ), builder => builder.Run( requestProcessor.ProcessPackages ) );
			app.Map( "/v2/FindPackagesById()", builder => builder.Run( requestProcessor.FindPackage ) );
		}
	}
}