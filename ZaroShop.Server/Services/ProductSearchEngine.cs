using Microsoft.EntityFrameworkCore;
using ZaroShop.Server.Utilities; // Where your Core C# Challenge class lives
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

    public IEnumerable<Product> Search(string? term)
    {
        if (string.IsNullOrWhiteSpace(term)) return Enumerable.Empty<Product>();

        string cacheKey = term.Trim().ToLower();
        if (_cache.TryGetValue(cacheKey, out var cachedResults)) return cachedResults;

        var products = _productRepo.GetAll()
            .Include(p => p.Category)
            .ToList();

        var engine = new SearchEngine<Product>(products);

        engine.AddSearchField(p => p.Name, 1.0);           // Primary
        engine.AddSearchField(p => p.Category?.Name, 0.8); // Secondary
        engine.AddSearchField(p => p.SKU, 0.4);            // Technical

        var freshResults = engine.Search(term, fuzzinessThreshold: 2).ToList();

        _cache[cacheKey] = freshResults;
        return freshResults;
    }

    public void ClearCache() => _cache.Clear();
}