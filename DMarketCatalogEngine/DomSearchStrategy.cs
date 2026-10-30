using System.Xml;

namespace DMarketCatalogEngine;

public class DomSearchStrategy : ISearchStrategy
{
    public List<CatalogItem> search(string xmlPath, SearchFilter filter)
    {
        var results = new List<CatalogItem>();
        if (!File.Exists(xmlPath)) return results;

        var doc = new XmlDocument();
        doc.Load(xmlPath);

        string xpath = BuildXPath(filter);
        XmlNodeList? nodes = doc.SelectNodes(xpath);
        if (nodes == null) return results;

        foreach (XmlNode node in nodes)
        {
            if (node.Attributes == null) continue;

            string id = node.Attributes["Id"]?.Value ?? string.Empty;
            string title = node.Attributes["Title"]?.Value ?? string.Empty;
            var item = new CatalogItem(id, title);

            foreach (XmlAttribute attr in node.Attributes)
            {
                item.setAttribute(attr.Name, attr.Value);
            }

            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == "Tag" && child.Attributes?["Name"] != null)
                {
                    item.addTag(child.Attributes["Name"]!.Value);
                }
            }

            if (filter.matches(item))
            {
                results.Add(item);
            }
        }

        return results;
    }

    private static string BuildXPath(SearchFilter filter)
    {
        Dictionary<string, string> criteria = filter.getCriteria();
        if (criteria.Count == 0)
        {
            return "//Item";
        }

        var conditions = new List<string>();
        foreach (var pair in criteria)
        {
            conditions.Add($"@{pair.Key}='{pair.Value}'");
        }

        return $"//Item[{string.Join(" and ", conditions)}]";
    }
}