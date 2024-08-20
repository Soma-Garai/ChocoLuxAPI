using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using ChocoLuxAPI.Services;
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
        private readonly MailService _mailService;
        public OrdersController(AppDbContext appDbContext, UserManager<UserModel> userManager, MailService mailService)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
            _mailService = mailService;
        }

        [HttpPost("checkout/{sessionId}")]
        //[Authorize(Policy = "Orders - Checkout")]
        public async Task<IActionResult> Checkout(Guid SessionId)
        {
            // Validate the user from the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }

            // Check if the session is valid
            var session = await _appDbContext.TblSession.FirstOrDefaultAsync(s => s.SessionId == SessionId);
            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                return Unauthorized("Invalid or expired session");
            }

            // Retrieve userId from session
            var userIdFromSession = session.UserId;

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

            // Return a response indicating the successful checkout and the order ID
            return Ok(new { orderId = order.OrderId });
        }



        [HttpGet("OrderConfirmation/{orderId}/{paymentType}")] 
        public async Task<IActionResult> OrderConfirmation(Guid orderId,string paymentType)
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
            // Map the order to an OrderDto
            var orderDto = new OrderDto
            {
                OrderId = order.OrderId,
                OrderDate = order.OrderDate,
                Status = "Order Confirmed",
                PaymentStatus=paymentType,
                TotalPrice = order.TotalPrice,
                OrderDetails = order.OrderDetails.Select(od => new OrderDetailDto
                {
                    ProductName = od.ProductName,
                    Quantity = od.Quantity,
                    ProductPrice = od.ProductPrice,
                    TotalPrice = od.TotalPrice
                }).ToList()
            };
            //update the OrderId in the Payment table here,it was null before
            // Update the Payment record to link to this order and set the payment status
            var payment = _appDbContext.TblPayment.FirstOrDefault(p => p.OrderId == null && p.PaymentType == paymentType);
            if (payment != null)
            {
                payment.OrderId = orderId;
                payment.PaymentStatus = paymentType.Equals("Cash On Delivery", StringComparison.OrdinalIgnoreCase) ? "Pending" : "Paid";
                await _appDbContext.SaveChangesAsync();
            }
            // Send the order confirmation email
             await _mailService.OrderConfirmationEmailAsync(orderId);
            // Return the order details as JSON
            return Ok(orderDto);
        }

    }
}
