using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Models.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int? ParentCategoryId { get; set; }
        public ICollection<Category> SubCategories { get; set; } = new List<Category>();

    }
}
