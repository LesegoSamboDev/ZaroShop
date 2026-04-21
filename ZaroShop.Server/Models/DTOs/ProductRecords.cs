using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Models.DTOs;

public record CreateProductRequest(
    string Name,
    string SKU,
    decimal Price,
    int Quantity,
    int CategoryId
);

public record ProductResponse(
    int Id,
    string Name,
    string SKU,
    decimal Price,
    int Quantity,
    string CategoryName 
)
{
    public static ProductResponse FromEntity(Product p) => new ProductResponse(
        p.Id,
        p.Name,
        p.SKU,
        p.Price,
        p.Quantity,
        p.Category?.Name ?? "Uncategorized" // Safely handles null categories
    );
};