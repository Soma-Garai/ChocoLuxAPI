using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RestSharp;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography.Xml;

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

        public CartController( TokenGenerator tokenGenerator, UserManager<UserModel> userManager, 
            AppDbContext appDbContext, ILogger<CartController> logger)
        {
            
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _logger = logger; 
        }

       // [Authorize(Policy = "Cart-CreateSessionForCart")]
        [HttpPost("CreateSessionForCart/{UserId}")]
        public async Task<IActionResult> CreateSessionForCart(string userId)
        {
            // Create a new session
            var session = new Session
            {
                SessionId = Guid.NewGuid(),
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = null
            };

            _appDbContext.TblSession.Add(session);
            await _appDbContext.SaveChangesAsync();

            return Ok(session.SessionId);
        }

        [Authorize(Policy = "Cart - AddItemToCart")]
        [HttpPost("AddItemToCart")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartOperationDto cartOperationDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            _logger.LogInformation("AddItemToCart method started");
            var sessionId = cartOperationDto.SessionId;
            //var userId= cartOperationDto.UserId;
            //List<CartItemDto> cartItemsDto = cartOperationDto.CartItems;         
            var cartItemDto = cartOperationDto.CartItems;

            // Find or create a cart for the session
            var cart = await _appDbContext.TblCart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                cart = new Cart
                {
                    CartId = Guid.NewGuid(),
                    SessionId = sessionId.Value,
                    UserId = userId
                };
                //saving Cart table
                await _appDbContext.TblCart.AddAsync(cart);
                await _appDbContext.SaveChangesAsync();
            }
            // Check if the product is already in the cart
            var existingCartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == cartItemDto.ProductId);

            if (existingCartItem != null)
            {
                // Update quantity and total price
                existingCartItem.Quantity += cartItemDto.Quantity;
                existingCartItem.TotalPrice = existingCartItem.ProductPrice * existingCartItem.Quantity;
                _appDbContext.TblCartItems.Update(existingCartItem);
                _logger.LogInformation("Updated item in cart: {CartItemId}", existingCartItem.CartItemId);
            }
            else
            {
                // Add the new item to the cart
                var cartItem = new CartItem
                {
                    CartItemId = Guid.NewGuid(),
                    CartId = cart.CartId,
                    ProductId = cartItemDto.ProductId,
                    Quantity = cartItemDto.Quantity,
                    ProductPrice = cartItemDto.ProductPrice,
                    TotalPrice = cartItemDto.ProductPrice * cartItemDto.Quantity,
                    CreatedAt = DateTime.Now
                };
                _appDbContext.TblCartItems.Add(cartItem);
                _logger.LogInformation("Added item to cart: {CartItemId}", cartItem.CartItemId);
            }

            await _appDbContext.SaveChangesAsync();
            return Ok(new { Message = "The Item is successfully added to the Cart" });

        }

        //[Authorize(Policy = "Cart-GetCartItems")]
        [HttpGet("GetCartItems/{sessionId}")]
        public async Task<IActionResult> GetCartItems(Guid sessionId)
        {
            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null)
            {
                return Unauthorized("Invalid or expired session");
            }

            // this query is used to fetch the cart and "its items" based on the user's session ID
            var cart = await _appDbContext.TblCart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            var CartSessionId = cart.SessionId;
            if (cart == null)
            {
                return NotFound("Cart not found");
            }
            
            //The code iterates over each CartItem in the cart.CartItems collection, transforms each CartItem into
            //a CartItemDto, and then collects these transformed objects into a list.
            var cartItemsDto = cart.CartItems.Select(ci => new CartItemDto
            {
                SessionId = CartSessionId,
                CartItemId = ci.CartItemId,
                ProductId = ci.ProductId,
                Quantity = ci.Quantity,
                ProductPrice = ci.ProductPrice,
                TotalPrice = ci.TotalPrice
                
            }).ToList();

            return Ok(cartItemsDto);
        }
        //delete method for the delete button
        //[Authorize(Policy = "Cart-RemoveItemFromCart")]
        [HttpPost("RemoveItemFromCart")]
        public async Task<IActionResult> RemoveItemFromCart(Guid sessionId, Guid cartItemId)
        {
            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null)
            {
                return Unauthorized("Invalid or expired session");
            }

            // Find the cart item and remove it
            var cartItem = await _appDbContext.TblCartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            _appDbContext.TblCartItems.Remove(cartItem);
            await _appDbContext.SaveChangesAsync();

            // Check if the cart is empty and set the expiresAt time in the session table 
            var cart = await _appDbContext.TblCart.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);
            if (cart != null && !cart.CartItems.Any())
            {
                //_appDbContext.TblSession.Remove(session);
                session.ExpiresAt = DateTime.Now;
                await _appDbContext.SaveChangesAsync();
                return Ok(new { Message = "All items removed. Session expiration updated.", expiresAt = session.ExpiresAt });
            }

            return Ok(new { Message = "Item removed from cart" });
        }
        //delete method for the javascript
        [HttpPost("RemoveFromCart")]
        public async Task<IActionResult> RemoveFromCart(Guid sessionId, Guid cartItemId)
        {
            //var sessionId = cartItemDto.SessionId;
            //var cartItemId = cartItemDto.CartItemId;
            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null)
            {
                return Unauthorized("Invalid or expired session");
            }

            // Find the cart item and remove it
            var cartItem = await _appDbContext.TblCartItems.FirstOrDefaultAsync(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            _appDbContext.TblCartItems.Remove(cartItem);
            await _appDbContext.SaveChangesAsync();

            // Check if the cart is empty and set the expiresAt time in the session table 
            var cart = await _appDbContext.TblCart.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);
            if (cart != null && !cart.CartItems.Any())
            {
                //_appDbContext.TblSession.Remove(session);
                session.ExpiresAt = DateTime.Now;
                await _appDbContext.SaveChangesAsync();
                return Ok(new { Message = "All items removed. Session expiration updated.", expiresAt = session.ExpiresAt });
            }

            return Ok(new { Message = "Item removed from cart" });
        }

        //update method for the javascript
        //[Authorize(Policy = "Cart-UpdateCartItem")]
        [HttpPost("UpdateCartItem")]
        public async Task<IActionResult> UpdateCartItem(CartItemDto cartItemDto)
        {
            var sessionId= cartItemDto.SessionId;
            var cartItemId = cartItemDto.CartItemId;
            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null)
            {
                return Unauthorized("Invalid or expired session");
            }

            // this query is used to fetch the cart and its items based on the user's session ID
            var cart = await _appDbContext.TblCart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }

            // Find the item to update
            var cartItem = cart.CartItems.FirstOrDefault(ci => ci.CartItemId == cartItemId);
            if (cartItem == null)
            {
                return NotFound("Cart item not found");
            }

            // Update the cart item
            cartItem.Quantity = cartItemDto.Quantity;
            cartItem.ProductPrice = cartItemDto.ProductPrice;
            cartItem.TotalPrice = (cartItemDto.ProductPrice ?? 0) * cartItemDto.Quantity;

            await _appDbContext.SaveChangesAsync();
            _logger.LogInformation("Updated item in cart: {CartItemId}", cartItem.CartItemId);
            // Create an updated CartItemDto to return
            var updatedCartItemDto = new CartItemDto
            {
                ProductId= cartItemDto.ProductId,
                CartItemId = cartItem.CartItemId,
                SessionId = cartItemDto.SessionId,
                Quantity = cartItem.Quantity,
                ProductPrice = cartItem.ProductPrice,
                TotalPrice = cartItem.TotalPrice
            };

            return Ok(updatedCartItemDto);

        }

        //[Authorize(Policy = "Cart-ClearCart")]
        [HttpPost("ClearCart/{sessionId}")]
        public async Task<IActionResult> ClearCart(Guid sessionId) 
        {
            var cart = await _appDbContext.TblCart
                            .Include(c => c.CartItems)
                            .FirstOrDefaultAsync(c=> c.SessionId == sessionId);
            // Remove all items from the cart
            _appDbContext.TblCartItems.RemoveRange(cart.CartItems);
            // Optionally, remove the cart itself
            _appDbContext.TblCart.Remove(cart);
            // Remove the session from TblSession
            var session = await _appDbContext.TblSession
                                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session != null)
            {
                _appDbContext.TblSession.Remove(session);
            }
            await _appDbContext.SaveChangesAsync();
            return Ok(new { Message = "The cart has been cleared" });

        }

    }
}