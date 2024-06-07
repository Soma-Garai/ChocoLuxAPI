﻿using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        public CartController( TokenGenerator tokenGenerator, UserManager<UserModel> userManager, AppDbContext appDbContext, ILogger<CartController> logger)
        {
            
            _tokenGenerator = tokenGenerator;
            _userManager = userManager;
            _appDbContext = appDbContext;
            _logger = logger;
        }

        [HttpPost("AddItemToCart")]
        public async Task<IActionResult> AddItemToCart([FromBody] CartOperationDto cartOperationDto)
        {
            var sessionId = cartOperationDto.SessionId;
            var userId= cartOperationDto.UserId;
            List<CartItemDto> cartItemsDto = cartOperationDto.CartItems;

            Session session= null;
            //if session is not yet created
            if (sessionId == null || !await _appDbContext.TblSession.AnyAsync(s => s.SessionId == sessionId))
            {
                // Create a new session
                var user = await _userManager.FindByIdAsync(userId); // Assuming you are using ASP.NET Identity
                if (user == null)
                {
                    return Unauthorized("User not found");
                }

                session = new Session
                {
                    SessionId = Guid.NewGuid(),
                    UserId = user.Id, // Assuming user.Id is the user's unique identifier
                    CreatedAt = DateTime.UtcNow
                };

                _appDbContext.TblSession.Add(session);
                await _appDbContext.SaveChangesAsync();

                sessionId = session.SessionId; // Set the sessionId to the new session's ID
            }
            else
            {
                // Check if the session is valid
                session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    return Unauthorized("Invalid or expired session");
                }
            }
            // Log the session ID and user ID
            _logger.LogInformation($"User ID: {session.UserId}, Session ID: {sessionId}");

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
                    UserId = session.UserId
                };

                _appDbContext.TblCart.Add(cart);
            }

            foreach (var cartItemDto in cartItemsDto)
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
                    CreatedAt = DateTime.UtcNow
                };
                cart.CartItems.Add(cartItem);
                _logger.LogInformation("Added item to cart: {CartItemId}", cartItem.CartItemId);
            }

            await _appDbContext.SaveChangesAsync(); //saving Cart and CartItems table
            return Ok(new { Message = "The Items are successfully added to the Cart", SessionId = session.SessionId });
        }



        [HttpGet("GetCartItems")]
        public async Task<IActionResult> GetCartItems([FromHeader] Guid sessionId)
        {
            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
            if (session == null)
            {
                return Unauthorized("Invalid or expired session");
            }

            // Find the cart for the session
            var cart = await _appDbContext.TblCart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == sessionId);

            if (cart == null)
            {
                return NotFound("Cart not found");
            }
            //The code iterates over each CartItem in the cart.CartItems collection, transforms each CartItem into
            //a CartItemDto, and then collects these transformed objects into a list.
            var cartItemsDto = cart.CartItems.Select(ci => new CartItemDto
            {
                ProductId = ci.ProductId,
                ProductName=ci.ProductName,
                ProductDescription=ci.ProductDescription,
                Quantity = ci.Quantity,
                ProductPrice = ci.ProductPrice,
                TotalPrice = ci.TotalPrice
            }).ToList();

            return Ok(cartItemsDto);
        }

        [HttpDelete("RemoveItemFromCart")]
        public async Task<IActionResult> RemoveItemFromCart([FromHeader] Guid sessionId, [FromHeader] Guid cartItemId)
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

            // Check if the cart is empty and expire the session if it is
            var cart = await _appDbContext.TblCart.Include(c => c.CartItems).FirstOrDefaultAsync(c => c.CartId == cartItem.CartId);
            if (cart != null && !cart.CartItems.Any())
            {
                _appDbContext.TblSession.Remove(session);
                await _appDbContext.SaveChangesAsync();
                return Ok(new { Message = "All items removed. Session expired." });
            }

            return Ok(new { Message = "Item removed from cart" });
        }


    }
}
//[HttpPost("AddItemToCart")]
//public async Task<IActionResult> AddItemToCart([FromHeader] Guid sessionId, [FromBody] List<CartItemDto> cartItemsDto)
//{
//    // Get user ID from claims
//    //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
//    //if (userId == null)
//    //{
//    //    return Unauthorized("User not authenticated");
//    //}
//    //var user = await _userManager.GetUserAsync(User); // Assuming you are using ASP.NET Identity
//    //if (user == null)
//    //{
//    //    return Unauthorized();
//    //}


//    // Check if the session is valid
//    var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == sessionId);
//    var userId = session?.UserId;
//    // Log the session ID and user ID
//    _logger.LogInformation($"User ID: {userId}, Session ID: {sessionId}");
//    if (session == null || session.ExpiresAt < DateTime.UtcNow)
//    {
//        return Unauthorized("Invalid or expired session");
//    }

//    // Find or create a cart for the session
//    var cart = await _appDbContext.TblCart
//        .Include(c => c.CartItems)
//        .FirstOrDefaultAsync(c => c.SessionId == sessionId);

//    if (cart == null)
//    {
//        cart = new Cart
//        {
//            CartId = Guid.NewGuid(),
//            SessionId = sessionId,
//            UserId = session.UserId
//        };

//        _appDbContext.TblCart.Add(cart);
//    }

//    foreach (var cartItemDto in cartItemsDto)
//    {
//        // Add the new item to the cart
//        var cartItem = new CartItem
//        {
//            CartItemId = Guid.NewGuid(),
//            CartId = cart.CartId,
//            ProductId = cartItemDto.ProductId,
//            Quantity = cartItemDto.Quantity,
//            ProductPrice = cartItemDto.ProductPrice,
//            TotalPrice = cartItemDto.TotalPrice * cartItemDto.Quantity,
//            CreatedAt = DateTime.UtcNow
//        };
//        cart.CartItems.Add(cartItem);
//        _logger.LogInformation("Added item to cart: {CartItemId}", cartItem.CartItemId);
//    }
//    await _appDbContext.SaveChangesAsync();
//    return Ok("The Items are successfully added to the Cart");
//}
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