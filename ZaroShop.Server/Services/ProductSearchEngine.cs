using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;
using ZaroShop.Core.Utilities; // Where your Core C# Challenge class lives

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

        // 1. Initialize the Utility Engine with fresh data from Repo
        var engine = new SearchEngine<Product>(_productRepo.GetAll());

        // 2. Configure the weights (Logic belongs here!)
        engine.AddSearchField(p => p.Name, 1.0);           // Primary focus
        engine.AddSearchField(p => p.Category?.Name, 0.8); // High focus
        engine.AddSearchField(p => p.SKU, 0.4);            // Technical focus

        // 3. Execute Fuzzy Search (distance 2 allows for "lptop" -> "laptop")
        var freshResults = engine.Search(term, fuzzinessThreshold: 2).ToList();

        _cache[cacheKey] = freshResults;
        return freshResults;
    }

    public void ClearCache() => _cache.Clear();
}