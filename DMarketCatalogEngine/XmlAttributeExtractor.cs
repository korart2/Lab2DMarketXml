using System.Xml.Linq;

namespace DMarketCatalogEngine;

public class XmlAttributeExtractor
{
    public Dictionary<string, List<string>> extractOptions(string xmlPath)
    {
        var options = new Dictionary<string, List<string>>();
        if (!File.Exists(xmlPath)) return options;

        XDocument doc = XDocument.Load(xmlPath);

        foreach (XElement itemElement in doc.Descendants("Item"))
        {
            foreach (XAttribute attr in itemElement.Attributes())
            {
                string name = attr.Name.LocalName;
                string value = attr.Value;

                if (string.IsNullOrWhiteSpace(value)) continue;

                if (!options.ContainsKey(name))
                {
                    options[name] = new List<string>();
                }

                if (!options[name].Contains(value))
                {
                    options[name].Add(value);
                }
            }
        }

        foreach (var list in options.Values)
        {
            list.Sort();
        }

        return options;
    }
}