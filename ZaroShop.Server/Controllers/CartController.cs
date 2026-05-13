using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ZaroShop.Server.Interfaces;
using ZaroShop.Server.Models.Entities;

namespace ZaroShop.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _cartRepository;

        public CartController(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        // GET: api/cart
        [HttpGet]
        public ActionResult<IQueryable<CartItem>> GetCartItems()
        {
            // Even though GetAll returns IQueryable, we can return it as an ActionResult
            var items = _cartRepository.GetAll().ToList();
            return Ok(items);
        }

        // GET: api/cart/5
        [HttpGet("{id}")]
        public ActionResult<CartItem> GetItem(int id)
        {
            var item = _cartRepository.GetById(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        // POST: api/cart
        [HttpPost]
        public ActionResult<CartItem> AddToCart(CartItem item)
        {
            _cartRepository.Add(item);
            // Returns a 201 Created status with the location of the new resource
            return CreatedAtAction(nameof(GetItem), new { id = item.Id }, item);
        }

        // DELETE: api/cart/5
        [HttpDelete("{id}")]
        public IActionResult RemoveFromCart(int id)
        {
            var existing = _cartRepository.GetById(id);
            if (existing == null) return NotFound();

            _cartRepository.Delete(id);
            return NoContent();
        }

        // GET: api/cart/total
        [HttpGet("total")]
        public ActionResult<decimal> GetTotal()
        {
            var total = _cartRepository.GetAll().Sum(x => x.Price * x.Quantity);
            return Ok(total);
        }
    }
}
