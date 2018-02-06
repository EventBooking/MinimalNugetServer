using System.Xml.Linq;

namespace MinimalNugetServer.Config
{
	public static class XmlNamespaces
	{
		public static readonly XNamespace Xmlns = "http://www.w3.org/2005/Atom";
		public static readonly XNamespace Baze = "https://www.nuget.org/api/v2/curated-feeds/microsoftdotnet";
		public static readonly XNamespace M = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";
		public static readonly XNamespace D = "http://schemas.microsoft.com/ado/2007/08/dataservices";
		public static readonly XNamespace Georss = "http://www.georss.org/georss";
		public static readonly XNamespace Gml = "http://www.opengis.net/gml";
	}
}