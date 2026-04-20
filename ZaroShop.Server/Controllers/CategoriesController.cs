using Microsoft.AspNetCore.Mvc;
using ZaroShop.Server.Models.DTOs;
using ZaroShop.Server.Models.Entities;
using ZaroShop.Server.Interfaces;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> _categoryRepo;

    public CategoriesController(IRepository<Category> categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    [HttpGet("tree")]
    public IActionResult GetCategoryTree()
    {
        var all = _categoryRepo.GetAll().ToList();

        // Build tree: Get roots and let the DTO recursion handle the rest
        var tree = all.Where(c => c.ParentCategoryId == null)
                      .Select(c => MapToDto(c, all));

        // Custom JSON Serialization (Indented)
        var options = new System.Text.Json.JsonSerializerOptions { WriteIndented = true };
        return Json(tree, options);
    }

    // Recursive helper for the DTO
    private CategoryDto MapToDto(Category category, List<Category> all) => new(
        category.Id,
        category.Name,
        category.Description,
        category.ParentCategoryId,
        all.Where(c => c.ParentCategoryId == category.Id)
           .Select(c => MapToDto(c, all))
           .ToList()
    );
}