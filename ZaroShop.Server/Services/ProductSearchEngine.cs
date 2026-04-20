using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Services;

public class ProductSearchEngine : IProductSearchEngine
{
    private readonly IRepository<Product> _productRepo;

    private readonly Dictionary<string, IEnumerable<Product>> _cache = new();

    public ProductSearchEngine(IRepository<Product> productRepo)
    {
        _productRepo = productRepo;
    }

    public IEnumerable<Product> Search(string term)
    {
        string cacheKey = term?.Trim().ToLower() ?? string.Empty;

        if (string.IsNullOrEmpty(cacheKey)) return Enumerable.Empty<Product>();

        if (_cache.TryGetValue(cacheKey, out var cachedResults))
        {
            return cachedResults;
        }

        var freshResults = _productRepo.GetAll()
            .Where(p => p.Name.Contains(term!, StringComparison.OrdinalIgnoreCase) ||
                       (p.Category != null && p.Category.Name.Contains(term!, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        _cache[cacheKey] = freshResults;

        return freshResults;
    }

    public void ClearCache()
    {
        _cache.Clear();
    }
}