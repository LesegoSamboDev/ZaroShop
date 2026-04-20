using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Extensions
{
    public static class ProductExtensions
    {
        public static IEnumerable<Product> FilterByPriceRange(this IEnumerable<Product> source, decimal min, decimal max)
            => source.Where(p => p.Price >= min && p.Price <= max);
    }
}
