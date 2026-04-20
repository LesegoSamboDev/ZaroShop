using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ZaroShop.Server.Data;
using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.DTOs;
using ZaroShop.Server.Models.Entities;
using static ZaroShop.Server.Models.DTOs.ProductRecords;

namespace ZaroShop.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IProductSearchEngine _searchEngine;

    public ProductsController(AppDbContext context, IProductSearchEngine searchEngine)
    {
        _context = context;
        _searchEngine = searchEngine;
    }

    // GET: /api/products (Pagination, Category Filter, and Search)
    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromQuery] string? name,
        [FromQuery] int? categoryId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // Demonstrate usage of the Search Engine for the 'name' filter
        IEnumerable<Product> queryResult;

        if (!string.IsNullOrWhiteSpace(name))
        {
            queryResult = _searchEngine.Search(name);
        }
        else
        {
            queryResult = await _context.Products.Include(p => p.Category).ToListAsync();
        }

        // Apply Category Filter using custom LINQ logic if needed
        if (categoryId.HasValue)
        {
            queryResult = queryResult.Where(p => p.CategoryId == categoryId.Value);
        }

        // Manual Pagination
        var totalItems = queryResult.Count();
        var items = queryResult
            .OrderBy(p => p.Name) // Default sort
            .Skip((page - 1) * pageSize)
            .Take(pageSize);
            //.Select(p => new Models.DTOs.ProductDto(p.Id, p.Name, p.SKU, p.Price, p.Quantity, p.Category?.Name ?? "Uncategorized"));

        return Ok(new { Total = totalItems, Page = page, Items = items });
    }

    // GET: /api/products/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult<Product>> GetProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
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

        // Pattern Matching Validation
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
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();

        _searchEngine.ClearCache(); // Invalidate cache on change

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    // PUT: /api/products/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] Product updatedProduct)
    {
        if (id != updatedProduct.Id) return BadRequest();

        var existing = await _context.Products.FindAsync(id);
        if (existing is null) return NotFound();

        // Update fields
        existing.Name = updatedProduct.Name;
        existing.Price = updatedProduct.Price;
        existing.Quantity = updatedProduct.Quantity;
        existing.CategoryId = updatedProduct.CategoryId;
        existing.UpdatedAt = DateTime.UtcNow;

        try
        {
            await _context.SaveChangesAsync();
            _searchEngine.ClearCache();
        }
        catch (DbUpdateConcurrencyException)
        {
            return Conflict();
        }

        return NoContent();
    }

    // DELETE: /api/products/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return NotFound();

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        _searchEngine.ClearCache();

        // Custom JSON Serialization for the delete response
        var options = new JsonSerializerOptions { WriteIndented = true };
        var response = JsonSerializer.Serialize(new { Message = $"Product {id} deleted successfully" }, options);

        return Content(response, "application/json");
    }
}