namespace ZaroShop.Server.Models.DTOs;

public record CategoryRequest(
    string Name,
    string? Description,
    int? ParentCategoryId
);

public record CategoryDto(
    int Id,
    string Name,
    string? Description,
    int? ParentCategoryId,
    List<CategoryDto> Children
);