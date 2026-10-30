namespace DMarketCatalogEngine;

public class CatalogController
{
    private string xmlFilePath = string.Empty;
    private ISearchStrategy? strategy;
    private readonly HtmlTransformer transformer;
    private readonly XmlAttributeExtractor extractor;

    public CatalogController(string? xslFilePath = null)
    {
        string resolvedXsl = string.IsNullOrWhiteSpace(xslFilePath)
            ? FindStyleXslPath()
            : xslFilePath;

        transformer = new HtmlTransformer(resolvedXsl);
        extractor = new XmlAttributeExtractor();
        strategy = new LinqSearchStrategy();
    }

    public Dictionary<string, List<string>> loadXmlFile(string path)
    {
        xmlFilePath = path;
        return extractor.extractOptions(path);
    }

    public string getXmlFilePath() => xmlFilePath;

    public bool setStrategy(StrategyType type)
    {
        strategy = type switch
        {
            StrategyType.DOM => new DomSearchStrategy(),
            StrategyType.SAX => new SaxSearchStrategy(),
            StrategyType.LINQ => new LinqSearchStrategy(),
            _ => new LinqSearchStrategy()
        };
        return true;
    }

    public List<CatalogItem> search(SearchFilter filter)
    {
        if (string.IsNullOrWhiteSpace(xmlFilePath) || strategy == null)
        {
            return new List<CatalogItem>();
        }
        return strategy.search(xmlFilePath, filter);
    }

    public bool transformToHtml(string htmlPath)
    {
        if (string.IsNullOrWhiteSpace(xmlFilePath))
        {
            return false;
        }
        return transformer.transform(xmlFilePath, htmlPath);
    }

    private static string FindStyleXslPath()
    {
        string? dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            string candidate = Path.Combine(dir, "Data", "style.xsl");
            if (File.Exists(candidate))
            {
                return candidate;
            }
            dir = Directory.GetParent(dir)?.FullName;
        }
        return "style.xsl";
    }
}