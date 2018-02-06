using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MinimalNugetServer
{
	public class Startup
	{
		private readonly IHostingEnvironment _env;
		private readonly IConfiguration _config;

		public Startup( IHostingEnvironment env, IConfiguration config )
		{
			_env = env;
			_config = config;
		}


		public void ConfigureServices( IServiceCollection services )
		{
		}

		public void Configure( IApplicationBuilder app )
		{
			var masterData = new MasterData( _config.GetSection( "nuget" ) );

			var requestProcessor = new Version2RequestProcessor();
			requestProcessor.Initialize( masterData );
			
			app.Run( async context =>
			{
				if ( context.Request.Path.StartsWithSegments( requestProcessor.ApiPrefix ) )
					await requestProcessor.ProcessRequest( context );
				else
					context.Response.StatusCode = 404;
			} );
		}
	}
}