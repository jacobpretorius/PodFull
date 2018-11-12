using System.Xml.Linq;

namespace PodFull.Web.Business.Helpers
{
	public static class XmlHelper
	{
		public static string ReadValue(XElement doc, XName name, string defaultValue = "")
		{
			if (doc.Element(name) != null)
			{
				return doc.Element(name).Value;
			}

			return defaultValue;
		}

		public static bool ReadBool(XElement doc, XName name)
		{
			if (doc.Element(name) != null)
			{
				return doc.Element(name).Value == "True";
			}

			return false;
		}

		public static int ReadInt(XElement doc, XName name)
		{
			if (doc.Element(name) != null)
			{
				int.TryParse(doc.Element(name).Value, out int result);
				return result;
			}

			return -1;
		}
	}
}