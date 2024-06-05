using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChocoLuxAPI.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class CartController : ControllerBase
    {
        //private readonly CartService _cartService;
        private readonly TokenGenerator _tokenGenerator;
        private readonly UserManager<UserModel> _userManager;
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<CartController> _logger;
        public CartController( TokenGenerator tokenGenerator, UserManager<UserModel> userManager, AppDbContext appDbContext, ILogger<CartController> logger)
        {
            
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _logger = logger;
        }

        [HttpPost("AddItemToCart")]
        public async Task<IActionResult> AddItemToCart([FromHeader] Guid sessionId, [FromBody] CartItemDto cartItemDto)
        {
            // Get user ID from claims
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User not authenticated");
            }
            //var user = await _userManager.GetUserAsync(User); // Assuming you are using ASP.NET Identity
            //if (user == null)
            //{
            //    return Unauthorized();
            //}

            // Log the session ID and user ID
            //_logger.LogInformation($"User ID: {user.Id}, Session ID: {sessionId}");
            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired session");
            }

            // Find or create a cart for the session
            var cart = await _appDbContext.TblCart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    SessionId = sessionId,
                    UserId = session.UserId
                };

                _appDbContext.TblCart.Add(cart);
            }

            // Add the new item to the cart
            var cartItem = new CartItem
            {
                CartItemId = Guid.NewGuid(),
                CartId = cart.CartId,
                ProductId = cartItemDto.ProductId,
                Quantity = cartItemDto.Quantity,
                ProductPrice = cartItemDto.ProductPrice,
                TotalPrice = cartItemDto.TotalPrice * cartItemDto.Quantity,
                CreatedAt = DateTime.UtcNow
            };
            cart.CartItems.Add(cartItem);

            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Added item to cart: {CartItemId}", cartItem.CartItemId);
            return Ok();
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

            cartItems.RemoveAll(item => item.ProductId == productId);
            var newToken = await _tokenGenerator.GenerateCartJwt(user, cartItems);
            HttpContext.Response.Cookies.Append("cartToken", newToken);

            return Ok(new { token = newToken });
        }

    }
}
//[HttpPost("AddItemToCart")]
//public async Task<IActionResult> AddItemToCart([FromBody] CartItemDto cartItem)
//{
//    //var token = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
//    //var cartItems = _cartService.DecodeJwt(token);

//    //cartItems.Add(cartItem);
//    //var newToken = _cartService.GenerateJwt(userId, cartItems);

//    //return Ok(new { token = newToken });
//    var user = await _userManager.GetUserAsync(User); // Assuming you are using ASP.NET Identity
//    if (user == null)
//    {
//        return Unauthorized();
//    }
//    var token = HttpContext.Request.Cookies["cartToken"];
//    List<CartItemDto> cartItems = string.IsNullOrEmpty(token) ? new List<CartItemDto>() : _tokenGenerator.DecodeCartJwt(token);

//    cartItems.Add(cartItem);
//    var newToken = await _tokenGenerator.GenerateCartJwt(user, cartItems);
//    HttpContext.Response.Cookies.Append("cartToken", newToken);

//    return Ok(new { token = newToken });
//}