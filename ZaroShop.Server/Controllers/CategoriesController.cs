using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.DTOs;
using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IRepository<Category> _categoryRepo;

    public CategoriesController(IRepository<Category> categoryRepo)
    {
        _categoryRepo = categoryRepo;
    }

    // GET: /api/categories (Flat List)
    [HttpGet]
    public IActionResult GetCategories()
    {
        var categories = _categoryRepo.GetAll();

        // Map to a simple flat DTO (ignoring children for the flat list)
        var flatList = categories.Select(c => new {
            c.Id,
            c.Name,
            c.Description,
            c.ParentCategoryId
        });

        return Ok(flatList);
    }

    // GET: /api/categories/tree (Hierarchical Structure)
    [HttpGet("tree")]
    public IActionResult GetCategoryTree()
    {
        var allCategories = _categoryRepo.GetAll().ToList();

        // 1. Identify Root Categories (ParentCategoryId is null)
        // 2. Recursively build the tree using the DTO record
        var tree = allCategories
            .Where(c => c.ParentCategoryId == null)
            .Select(c => BuildNode(c, allCategories))
            .ToList();

        // Requirement: Custom JSON Serialization
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        return new JsonResult(tree, options);
    }

    // POST: /api/categories
    [HttpPost]
    public IActionResult CreateCategory([FromBody] CategoryRequest request)
    {
        // Simple Manual Validation (Pattern Matching could be used here too)
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return BadRequest("Category name is required.");
        }

        var newCategory = new Category
        {
            // Simple ID generation for In-Memory demo
            Id = _categoryRepo.GetAll().Any() ? _categoryRepo.GetAll().Max(c => c.Id) + 1 : 1,
            Name = request.Name,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId
        };

        _categoryRepo.Add(newCategory);

        return CreatedAtAction(nameof(GetCategories), new { id = newCategory.Id }, newCategory);
    }

    /// <summary>
    /// Recursive helper to build the CategoryDto tree.
    /// </summary>
    private CategoryDto BuildNode(Category current, List<Category> all)
    {
        return new CategoryDto(
            current.Id,
            current.Name,
            current.Description,
            current.ParentCategoryId,
            // Recursively find children where ParentId matches current Id
            all.Where(c => c.ParentCategoryId == current.Id)
               .Select(c => BuildNode(c, all))
               .ToList()
        );
    }
}