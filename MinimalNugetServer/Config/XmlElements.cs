using System.Xml.Linq;

namespace MinimalNugetServer.Config
{
	public static class XmlElements
	{
		public static readonly XName Feed = XmlNamespaces.Xmlns + "feed";
		public static readonly XName Entry = XmlNamespaces.Xmlns + "entry";
		public static readonly XName Id = XmlNamespaces.Xmlns + "id";
		public static readonly XName Content = XmlNamespaces.Xmlns + "content";

		public static readonly XName MCount = XmlNamespaces.M + "count";
		public static readonly XName MProperties = XmlNamespaces.M + "properties";

		public static readonly XName DId = XmlNamespaces.D + "Id";
		public static readonly XName DVersion = XmlNamespaces.D + "Version";

		public static readonly XName Baze = XNamespace.Xmlns + "base";
		public static readonly XName M = XNamespace.Xmlns + "m";
		public static readonly XName D = XNamespace.Xmlns + "d";
		public static readonly XName Georss = XNamespace.Xmlns + "georss";
		public static readonly XName Gml = XNamespace.Xmlns + "gml";
	}
}