using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
			var masterData = new MasterData( _config.GetSection( "nuget" ) );

			var requestProcessor = new Version2RequestProcessor( masterData );

			app.Map( "/v2/download", builder => builder.Run( requestProcessor.ProcessDownload ) );
			app.Map( "/v2/Search()", builder => builder.Run( requestProcessor.ProcessSearch ) );
			app.Map( "/v2/Packages(", builder => builder.Run( requestProcessor.ProcessPackages ) );
			app.Map( "/v2/FindPackagesById()", builder => builder.Run( requestProcessor.FindPackage ) );
		}
	}
}