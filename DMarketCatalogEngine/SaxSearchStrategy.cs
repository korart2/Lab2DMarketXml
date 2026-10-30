using System.Xml;

namespace DMarketCatalogEngine;

public class SaxSearchStrategy : ISearchStrategy
{
    public List<CatalogItem> search(string xmlPath, SearchFilter filter)
    {
        var results = new List<CatalogItem>();
        if (!File.Exists(xmlPath)) return results;

        using var reader = XmlReader.Create(xmlPath);
        CatalogItem? currentItem = null;

        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element && reader.Name == "Item")
            {
                string id = reader.GetAttribute("Id") ?? string.Empty;
                string title = reader.GetAttribute("Title") ?? string.Empty;
                currentItem = new CatalogItem(id, title);

                bool isEmptyElement = reader.IsEmptyElement;

                if (reader.HasAttributes)
                {
                    while (reader.MoveToNextAttribute())
                    {
                        currentItem.setAttribute(reader.Name, reader.Value);
                    }
                    reader.MoveToElement();
                }

                if (isEmptyElement)
                {
                    if (filter.matches(currentItem))
                    {
                        results.Add(currentItem);
                    }
                    currentItem = null;
                }
            }
            else if (reader.NodeType == XmlNodeType.Element && reader.Name == "Tag" && currentItem != null)
            {
                string? tagName = reader.GetAttribute("Name");
                if (!string.IsNullOrWhiteSpace(tagName))
                {
                    currentItem.addTag(tagName);
                }
            }
            else if (reader.NodeType == XmlNodeType.EndElement && reader.Name == "Item" && currentItem != null)
            {
                if (filter.matches(currentItem))
                {
                    results.Add(currentItem);
                }
                currentItem = null;
            }
        }

        return results;
    }
}