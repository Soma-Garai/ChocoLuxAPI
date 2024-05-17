using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private List<CartItem> cartItems = new List<CartItem>();

        [HttpGet]
        public IActionResult ViewCart()
        {
            var cart = new Cart { Items = cartItems };
            return Ok(cart);
        }

        [HttpPost("add")]
        public IActionResult AddToCart([FromBody] CartItem item)
        {
            cartItems.Add(item);
            return Ok();
        }

        [HttpDelete("remove/{productId}")]
        public IActionResult RemoveFromCart(int productId)
        {
            var itemToRemove = cartItems.FirstOrDefault(item => item.Product.product_id == productId);
            if (itemToRemove != null)
                cartItems.Remove(itemToRemove);
            return Ok();
        }

        //[HttpPut("update/{productId}")]
        //public IActionResult UpdateCartItemQuantity(int productId, [FromBody] int quantity)
        //{
        //    var cartItemToUpdate = _cartItems.FirstOrDefault(item => item.ProductId == productId);
        //    if (cartItemToUpdate != null)
        //    {
        //        cartItemToUpdate.Quantity = quantity;
        //        return Ok();
        //    }
        //    else
        //    {
        //        return NotFound();
        //    }
        //}

    }
}
