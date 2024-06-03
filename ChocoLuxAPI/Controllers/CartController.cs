using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ChocoLuxAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        //private readonly CartService _cartService;
        private readonly TokenGenerator _tokenGenerator;
        private readonly UserManager<UserModel> _userManager;

        public CartController( TokenGenerator tokenGenerator, UserManager<UserModel> userManager)
        {
            
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
        }

        [HttpPost("AddItemToCart")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartItemDto cartItem)
        {
            //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //var cartItems = _cartService.DecodeJwt(token);

            //cartItems.Add(cartItem);
            //var newToken = _cartService.GenerateJwt(userId, cartItems);

            //return Ok(new { token = newToken });
            var user = await _userManager.GetUserAsync(User); // Assuming you are using ASP.NET Identity
            if (user == null)
            {
                return Unauthorized();
            }
            var token = HttpContext.Request.Cookies["cartToken"];
            List<CartItemDto> cartItems = string.IsNullOrEmpty(token) ? new List<CartItemDto>() : _tokenGenerator.DecodeCartJwt(token);

            cartItems.Add(cartItem);
            var newToken =await _tokenGenerator.GenerateCartJwt(user, cartItems);
            HttpContext.Response.Cookies.Append("cartToken", newToken);

            return Ok(new { token = newToken });
        }

        [HttpGet]
        public IActionResult GetCartItems()
        {
            //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //var cartItems = _cartService.DecodeJwt(token);

            //return Ok(cartItems);
            var token = HttpContext.Request.Cookies["cartToken"];
            List<CartItemDto> cartItems = string.IsNullOrEmpty(token) ? new List<CartItemDto>() : _tokenGenerator.DecodeCartJwt(token);

            return Ok(cartItems);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveItemFromCart([FromQuery] Guid productId)
        {
            //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            //var cartItems = _cartService.DecodeJwt(token);

            //cartItems.RemoveAll(item => item.Product.ProductId == productId);
            //var newToken = _cartService.GenerateJwt(userId, cartItems);

            //return Ok(new { token = newToken });
            var user = await _userManager.GetUserAsync(User); // Assuming you are using ASP.NET Identity
            if (user == null)
            {
                return Unauthorized();
            }

            var token = HttpContext.Request.Cookies["cartToken"];
            var cartItems = string.IsNullOrEmpty(token) ? new List<CartItemDto>() : _tokenGenerator.DecodeCartJwt(token);

            cartItems.RemoveAll(item => item.Product.ProductId == productId);
            var newToken = await _tokenGenerator.GenerateCartJwt(user, cartItems);
            HttpContext.Response.Cookies.Append("cartToken", newToken);

            return Ok(new { token = newToken });
        }

    }
}
