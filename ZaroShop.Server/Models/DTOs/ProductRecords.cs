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
);