using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Repositories
{
    public class InMemoryCategoryRepository : IRepository<Category>
    {
        private readonly List<Category> _categories = new();

        public IEnumerable<Category> GetAll() => _categories;
        public void Add(Category entity) => _categories.Add(entity);
    }
}
