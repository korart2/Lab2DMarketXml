namespace DMarketCatalogEngine;

public class SearchFilter
{
    private readonly Dictionary<string, string> criteria = new();

    public bool setCriterion(string attribute, string value)
    {
        if (string.IsNullOrWhiteSpace(attribute)) return false;

        if (string.IsNullOrWhiteSpace(value))
        {
            criteria.Remove(attribute);
        }
        else
        {
            criteria[attribute] = value.Trim();
        }
        return true;
    }

    public Dictionary<string, string> getCriteria() => new(criteria);

    public bool clear()
    {
        criteria.Clear();
        return true;
    }

    public bool matches(CatalogItem item)
    {
        foreach (var pair in criteria)
        {
            if (pair.Key == "Keyword")
            {
                if (!MatchesKeyword(item, pair.Value))
                {
                    return false;
                }
                continue;
            }

            string actualValue = item.getAttribute(pair.Key);
            if (actualValue != pair.Value)
            {
                return false;
            }
        }
        return true;
    }

    private static bool MatchesKeyword(CatalogItem item, string keyword)
    {
        if (item.getTitle().Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            item.getId().Contains(keyword, StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        foreach (var attr in item.getAttributes().Values)
        {
            if (attr.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        foreach (string tag in item.getTags())
        {
            if (tag.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}