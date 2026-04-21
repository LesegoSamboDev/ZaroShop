namespace ZaroShop.Server.Models.Entities;

// Implement IComparable with the Product type
public class Product : IComparable<Product>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int CategoryId { get; set; }
    public virtual Category? Category { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public int CompareTo(Product? other)
    {
        if (other is null) return 1;

        int priceComparison = Price.CompareTo(other.Price);
        if (priceComparison != 0)
        {
            return priceComparison;
        }

        return string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }
}