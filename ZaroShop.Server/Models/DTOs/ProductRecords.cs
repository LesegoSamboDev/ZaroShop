namespace ZaroShop.Server.Models.DTOs
{
    public class ProductRecords
    {
        public record ProductDto(int Id, string Name, string SKU, decimal Price, int Quantity, string CategoryName);
        public record CreateProductRequest(string Name, string SKU, decimal Price, int Quantity, int CategoryId);
    }
}
