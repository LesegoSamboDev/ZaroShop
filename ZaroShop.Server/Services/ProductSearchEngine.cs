using ZaroShop.Server.Data;
using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Services
{
    public class ProductSearchEngine
    {
        private readonly AppDbContext _db;
        private readonly Dictionary<string, List<Product>> _cache = new();

        public ProductSearchEngine(AppDbContext db) => _db = db;

        public List<Product> Search(string term)
        {
            if (_cache.TryGetValue(term, out var results)) return results;

            var freshResults = _db.Products
                .Where(p => p.Name.Contains(term) || p.Category!.Name.Contains(term))
                .ToList();

            _cache[term] = freshResults; // Simple Dictionary Caching
            return freshResults;
        }
    }
}
