namespace DMarketCatalogEngine;

public interface ISearchStrategy
{
    List<CatalogItem> search(string xmlPath, SearchFilter filter);
}