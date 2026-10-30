using System.Xml.Linq;

namespace DMarketCatalogEngine;

public class LinqSearchStrategy : ISearchStrategy
{
    public List<CatalogItem> search(string xmlPath, SearchFilter filter)
    {
        var results = new List<CatalogItem>();
        if (!File.Exists(xmlPath)) return results;

        XDocument doc = XDocument.Load(xmlPath);

        IEnumerable<CatalogItem> query =
            from itemElement in doc.Descendants("Item")
            let item = CreateItemFromElement(itemElement)
            where filter.matches(item)
            select item;

        return query.ToList();
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