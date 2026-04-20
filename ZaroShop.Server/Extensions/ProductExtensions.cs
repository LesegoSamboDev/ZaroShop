using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Extensions;

public static class ProductFilterExtensions
{
    public static IEnumerable<Product> FilterByName(this IEnumerable<Product> source, string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return source;

        return source.Where(p => p.Name.Contains(name, StringComparison.OrdinalIgnoreCase));
    }

    public static IEnumerable<Product> FilterByCategory(this IEnumerable<Product> source, int? categoryId)
    {
        if (!categoryId.HasValue) return source;

        return source.Where(p => p.CategoryId == categoryId.Value);
    }

    public static IEnumerable<Product> FilterByPriceRange(this IEnumerable<Product> source, decimal? minPrice, decimal? maxPrice)
    {
        var result = source;
        if (minPrice.HasValue) result = result.Where(p => p.Price >= minPrice.Value);
        if (maxPrice.HasValue) result = result.Where(p => p.Price <= maxPrice.Value);

        return result;
    }

    public static IEnumerable<Product> FilterInStock(this IEnumerable<Product> source, bool onlyInStock)
    {
        return onlyInStock ? source.Where(p => p.Quantity > 0) : source;
    }
}