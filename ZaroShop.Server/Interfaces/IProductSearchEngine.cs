using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Interfaces
{
    public interface IProductSearchEngine
    {
        IEnumerable<Product> Search(string searchTerm);

        void ClearCache();
    }
}
