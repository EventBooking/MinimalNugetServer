using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace MinimalNugetServer
{
	public class Program
	{
		public static void Main( string[] args )
		{
			BuildWebHost().Run();
		}

		private static IWebHost BuildWebHost()
		{
			return new WebHostBuilder()
				.UseKestrel()
				.UseStartup<Startup>()
				.ConfigureAppConfiguration( config =>
				{
					config.SetBasePath( Directory.GetCurrentDirectory() )
						.AddEnvironmentVariables()
						.AddEnvironmentVariables( "NUGET_SERVER_" )
						.AddJsonFile( "config.json", false );
				} )
				.Build();
		}
	}
}