using System.Xml.Linq;

namespace DMarketCatalogEngine;

public class LinqSearchStrategy : ISearchStrategy
{
    public List<CatalogItem> search(string xmlPath, SearchFilter filter)
    {
        var results = new List<CatalogItem>();
        if (!File.Exists(xmlPath)) return results;

        XDocument doc = XDocument.Load(xmlPath);
        Dictionary<string, string> criteria = filter.getCriteria();

        IEnumerable<XElement> query =
            from itemElement in doc.Descendants("Item")
            where MatchesXmlAttributes(itemElement, criteria)
            select itemElement;

        foreach (XElement element in query)
        {
            CatalogItem item = CreateItemFromElement(element);
            if (filter.matches(item))
            {
                results.Add(item);
            }
        }

        return results;
    }

    private static bool MatchesXmlAttributes(XElement element, Dictionary<string, string> criteria)
    {
        foreach (var pair in criteria)
        {
            if (pair.Key == "Keyword")
            {
                continue;
            }

            string? attrVal = element.Attribute(pair.Key)?.Value;
            if (attrVal != pair.Value)
            {
                return false;
            }
        }
        return true;
    }

    private static CatalogItem CreateItemFromElement(XElement element)
    {
        string id = element.Attribute("Id")?.Value ?? string.Empty;
        string title = element.Attribute("Title")?.Value ?? string.Empty;
        var item = new CatalogItem(id, title);

        foreach (XAttribute attr in element.Attributes())
        {
            item.setAttribute(attr.Name.LocalName, attr.Value);
        }

        foreach (XElement tagElement in element.Elements("Tag"))
        {
            string? tagName = tagElement.Attribute("Name")?.Value;
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                item.addTag(tagName);
            }
        }

        return item;
    }
}