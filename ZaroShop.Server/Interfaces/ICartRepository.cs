using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Interfaces
{
    public interface ICartRepository : IRepository<CartItem>
    {
        decimal GetTotalValueAsync();
    }
}
