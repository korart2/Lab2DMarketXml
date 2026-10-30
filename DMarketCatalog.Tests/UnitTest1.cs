using DMarketCatalogEngine;

namespace DMarketCatalog.Tests;

public class DMarketXmlTests
{
    private static string GetDataFilePath(string fileName)
    {
        string baseDir = AppContext.BaseDirectory;
        string solutionRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
        return Path.Combine(solutionRoot, "Data", fileName);
    }

    [Fact]
    public void AttributeExtractor_ExtractsAllUniqueValuesFromXml()
    {
        string xmlPath = GetDataFilePath("dmarket.xml");
        var ctrl = new CatalogController(GetDataFilePath("style.xsl"));

        Dictionary<string, List<string>> options = ctrl.loadXmlFile(xmlPath);

        Assert.True(options.ContainsKey("Game"));
        Assert.Equal(3, options["Game"].Count); // CS2, Dota 2, Rust
        Assert.Contains("CS2", options["Game"]);
        Assert.Contains("Dota 2", options["Game"]);
        Assert.Contains("Rust", options["Game"]);
    }

    [Theory]
    [InlineData(StrategyType.DOM)]
    [InlineData(StrategyType.SAX)]
    [InlineData(StrategyType.LINQ)]
    public void AllThreeStrategies_ReturnIdenticalFilteredResults(StrategyType type)
    {
        string xmlPath = GetDataFilePath("dmarket.xml");
        var ctrl = new CatalogController(GetDataFilePath("style.xsl"));
        ctrl.loadXmlFile(xmlPath);

        var filter = new SearchFilter();
        filter.setCriterion("Game", "CS2");
        filter.setCriterion("Rarity", "Covert");

        ctrl.setStrategy(type);
        List<CatalogItem> items = ctrl.search(filter);

        Assert.Equal(2, items.Count);
        Assert.Contains(items, i => i.getTitle() == "AWP | Asiimov");
        Assert.Contains(items, i => i.getTitle() == "Karambit | Doppler");
    }

    [Fact]
    public void HtmlTransformer_GeneratesHtmlReportSuccessfully()
    {
        string xmlPath = GetDataFilePath("dmarket.xml");
        string xslPath = GetDataFilePath("style.xsl");
        string htmlPath = Path.Combine(Path.GetTempPath(), $"dmarket_report_{Guid.NewGuid()}.html");

        try
        {
            var ctrl = new CatalogController(xslPath);
            ctrl.loadXmlFile(xmlPath);

            Assert.True(ctrl.transformToHtml(htmlPath));
            Assert.True(File.Exists(htmlPath));

            string htmlContent = File.ReadAllText(htmlPath);
            Assert.Contains("AWP | Asiimov", htmlContent);
            Assert.Contains("Karambit | Doppler", htmlContent);
        }
        finally
        {
            if (File.Exists(htmlPath))
            {
                File.Delete(htmlPath);
            }
        }
    }
}