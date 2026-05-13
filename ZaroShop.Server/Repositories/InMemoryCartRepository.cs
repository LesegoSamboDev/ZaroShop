using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Repositories
{
    public class InMemoryCartRepository : InMemoryRepositoryBase<CartItem>, ICartRepository
    {
        public decimal GetTotalValueAsync()
        {
            return _collection.Sum(item => item.Price * item.Quantity);
        }

    }
}
