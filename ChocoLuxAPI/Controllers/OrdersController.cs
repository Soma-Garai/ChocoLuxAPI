using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Claims;


namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<UserModel> _userManager;
        public OrdersController(AppDbContext appDbContext, UserManager<UserModel> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;

        }

        [HttpPost("checkout/{sessionId}")]
        //[Authorize(Policy = "CheckoutPolicy")]
        public async Task<IActionResult> Checkout(Guid SessionId)
        {
            // Validate the user from the JWT token
            //var user = await _userManager.GetUserAsync(User);
            //if (user == null)
            //{
            //    return Unauthorized();
            //}

            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == SessionId);
            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired session");
            }

            // Retrieve userId from session
            var userIdFromSession = session.UserId;

            //// Verify that the userId from session matches the authenticated user
            //if (userIdFromSession != user.Id)
            //{
            //    return Unauthorized("User ID mismatch");
            //}

            // Find the cart for the session
            var cart = await _appDbContext.TblCart
                .Include(c => c.CartItems)
                .FirstOrDefaultAsync(c => c.SessionId == SessionId);

            if (cart == null || !cart.CartItems.Any())
            {
                return BadRequest("Cart is empty");
            }

            // Create a new order
            var order = new Orders
            {
                OrderId= Guid.NewGuid(),
                UserId = userIdFromSession,
                OrderDate = DateTime.Now,
                Status = Orders.OrderStatus.Shipped,  // Set the status as needed
                OrderDetails = new List<OrderDetails>()
            };

            // Save the order to the database
            _appDbContext.tblOrders.Add(order);
            _appDbContext.SaveChanges();

            // Calculate TotalPrice for each OrderDetail item and sum them up
            int? TotalPrice = 0;
            foreach (var item in cart.CartItems)
            {
                int quantity = item.Quantity;
                var productId = item.ProductId;
                var product = _appDbContext.tblProducts.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product == null)
                {
                    // Product not found, handle accordingly
                    // For example, return a BadRequest response
                    return BadRequest($"Product not found");
                }

                var orderDetail = new OrderDetails
                {
                    OrderItemId=Guid.NewGuid(),
                    ProductId=productId,
                    ProductName = product.ProductName,
                    Quantity = quantity,
                    ProductPrice = product.ProductPrice,
                    OrderId = order.OrderId
                };

                // Calculate TotalPrice for this order detail
                orderDetail.TotalPrice = orderDetail.Quantity * orderDetail.ProductPrice;

                // Add the order detail to the database
                _appDbContext.tblOrderDetails.Add(orderDetail);
                _appDbContext.SaveChanges();

                // Add the total price of this order detail to the total price of the order
                TotalPrice = TotalPrice + orderDetail.TotalPrice;
            }

            // Set the total price of the order
            order.TotalPrice = TotalPrice;
            //set the ExpiresAt in the Session table
            session.ExpiresAt = DateTime.Now;
            // Save the order to the database
            _appDbContext.SaveChanges();
            
            // Optionally, you can clear the cart or perform any other cleanup
            // For example, if the cart is stored in session, you can clear it here

            // Return a response indicating the successful checkout and the order ID
            return Ok(new { orderId = order.OrderId });
        }

        [HttpGet("order/{orderId}")]
        public IActionResult OrderConfirmation(Guid orderId)
        {
            // Retrieve the order from the database based on orderId
            var order = _appDbContext.tblOrders
                              .Include(o => o.OrderDetails) // Include OrderDetails for this order
                              .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                // Handle case where order is not found
                return NotFound($"Order with ID {orderId} not found");
            }

            // Return the order details as JSON
            return Ok(order);
        }

    }
}
