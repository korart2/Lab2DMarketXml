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
            criteria[attribute] = value;
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
            string actualValue = item.getAttribute(pair.Key);
            if (actualValue != pair.Value)
            {
                return false;
            }
        }
        return true;
    }
}