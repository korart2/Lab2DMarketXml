using System.Xml.Xsl;

namespace DMarketCatalogEngine;

public class HtmlTransformer
{
    private readonly string xslFilePath;

    public HtmlTransformer(string xslFilePath)
    {
        this.xslFilePath = xslFilePath;
    }

    public bool transform(string xmlPath, string htmlPath)
    {
        if (!File.Exists(xmlPath) || !File.Exists(xslFilePath))
        {
            return false;
        }

        var xslt = new XslCompiledTransform();
        xslt.Load(xslFilePath);
        xslt.Transform(xmlPath, htmlPath);
        return true;
    }
}