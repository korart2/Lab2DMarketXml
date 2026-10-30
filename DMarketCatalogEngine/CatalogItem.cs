using System.Text;

namespace DMarketCatalogEngine;

public class CatalogItem
{
    private readonly string id;
    private readonly string title;
    private readonly Dictionary<string, string> attributes = new();
    private readonly List<string> tags = new();

    public CatalogItem(string id, string title)
    {
        this.id = id;
        this.title = title;
    }

    public string getId() => id;
    public string getTitle() => title;

    public bool setAttribute(string name, string value)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;
        attributes[name] = value;
        return true;
    }

    public string getAttribute(string name)
    {
        if (attributes.TryGetValue(name, out string? value))
        {
            return value;
        }
        return string.Empty;
    }

    public Dictionary<string, string> getAttributes() => new(attributes);

    public bool addTag(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag)) return false;
        tags.Add(tag);
        return true;
    }

    public List<string> getTags() => new(tags);

    public string getDisplayInfo()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"[{id}] {title}");

        foreach (var pair in attributes)
        {
            if (pair.Key == "Id" || pair.Key == "Title")
            {
                continue;
            }
            sb.AppendLine($"   {pair.Key}: {pair.Value}");
        }

        if (tags.Count > 0)
        {
            sb.AppendLine($"   Теги: {string.Join(", ", tags)}");
        }

        return sb.ToString().TrimEnd();
    }
}