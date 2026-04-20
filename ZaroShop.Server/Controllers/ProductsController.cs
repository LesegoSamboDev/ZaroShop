using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ZaroShop.Server.Extensions;
using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.DTOs;
using ZaroShop.Server.Models.Entities;
using static ZaroShop.Server.Models.DTOs.ProductRecords;

namespace ZaroShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IRepository<Product> _productRepo;
    private readonly IProductSearchEngine _searchEngine;

    public ProductsController(IRepository<Product> productRepo, IProductSearchEngine searchEngine)
    {
        _productRepo = productRepo;
        _searchEngine = searchEngine;
    }

    [HttpGet]
    public IActionResult GetProducts(
        [FromQuery] string? name,
        [FromQuery] int? categoryId,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] bool onlyInStock = false)
    {
        var products = _productRepo.GetAll();

        var filtered = products
            .FilterByName(name)
            .FilterByCategory(categoryId)
            .FilterByPriceRange(minPrice, maxPrice)
            .FilterInStock(onlyInStock)
            .ToList();

        // Demonstrating IComparable sorting if no specific filter is applied
        filtered.Sort();

        return Ok(filtered);
    }

    // GET: /api/products/{id}
    [HttpGet("{id}")]
    public ActionResult<Product> GetProduct(int id)
    {
        // Now using the Repository instead of _context
        var product = _productRepo.GetById(id);
        return product is null ? NotFound() : Ok(product);
    }

    // POST: /api/products (Demonstrating Manual Model Binding)
    [HttpPost]
    public async Task<IActionResult> CreateProduct()
    {
        using var reader = new StreamReader(Request.Body);
        var body = await reader.ReadToEndAsync();

        var request = JsonSerializer.Deserialize<CreateProductRequest>(body,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        // Pattern Matching Validation (Requirement)
        if (request is not { Name.Length: > 0, Price: > 0, Quantity: >= 0 })
        {
            return BadRequest("Invalid product data. Price must be > 0 and Quantity >= 0.");
        }

        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Price = request.Price,
            Quantity = request.Quantity,
            CategoryId = request.CategoryId
        };

        // Saving through the Repository
        _productRepo.Add(product);
        _searchEngine.ClearCache();

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // PUT: /api/products/{id}
    [HttpPut("{id}")]
    public IActionResult UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        if (id != updatedProduct.Id) return BadRequest();

        var existing = _productRepo.GetById(id);
        if (existing is null) return NotFound();

        // Update fields on the tracked entity
        existing.Name = updatedProduct.Name;
        existing.Price = updatedProduct.Price;
        existing.Quantity = updatedProduct.Quantity;
        existing.CategoryId = updatedProduct.CategoryId;

        _productRepo.Update(existing);
        _searchEngine.ClearCache();

        return NoContent();
    }

    // DELETE: /api/products/{id}
    [HttpDelete("{id}")]
    public IActionResult DeleteProduct(int id)
    {
        var existing = _productRepo.GetById(id);
        if (existing == null) return NotFound();

        _productRepo.Delete(id);
        _searchEngine.ClearCache();

        // Requirement: Custom JSON Serialization for the delete response
        var options = new JsonSerializerOptions { WriteIndented = true };
        var response = JsonSerializer.Serialize(new { Message = $"Product {id} deleted successfully", Timestamp = DateTime.Now }, options);

        return Content(response, "application/json");
    }
}