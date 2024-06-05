using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;


namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<UserModel> _userManager;
        public OrdersController(AppDbContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;

        }

        [HttpPost("checkout")]
        //[Authorize(Policy = "CheckoutPolicy")]
        //public IActionResult Checkout([FromBody] Cart model)
        //{
        //    // Retrieve the cart from the request body
        //    var cart = model ?? new Cart(); // Assuming the client sends the cart in the request body

        //    // Get the current logged-in user ID
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        //    if (string.IsNullOrEmpty(userId))
        //    {
        //        return Unauthorized(); // User not found or not authenticated
        //    }

        //    // Create a new order
        //    var order = new Orders
        //    {
        //        UserId = userId,
        //        OrderDate = DateTime.Now,
        //        Status = Orders.OrderStatus.Shipped,  // Set the status as needed
        //        OrderDetails = new List<OrderDetails>()
        //    };

        //    // Save the order to the database
        //    _context.tblOrders.Add(order);
        //    _context.SaveChanges();

        //    // Calculate TotalPrice for each OrderDetail item and sum them up
        //    int? TotalPrice = 0;
        //    foreach (var item in cart.CartItems)
        //    {
        //        int quantity = item.Quantity;
        //        var productDto = item.Product;
        //        //var product = _context.tblProducts.FirstOrDefault(p => p.product_id == item.Product.product_id);
        //        if (productDto == null)
        //        {
        //            // Product not found, handle accordingly
        //            // For example, return a BadRequest response
        //            return BadRequest($"Product not found");
        //        }

        //        var orderDetail = new OrderDetails
        //        {
        //            OrderId = order.OrderId,

        //            //ProductId = productViewModel.product_id,
        //            ProductName = productDto.ProductName,
        //            Quantity = quantity,
        //            ProductPrice = productDto.ProductPrice
        //        };

        //        // Calculate TotalPrice for this order detail
        //        orderDetail.TotalPrice = orderDetail.Quantity * orderDetail.ProductPrice;

        //        // Add the order detail to the database
        //        _context.tblOrderDetails.Add(orderDetail);
        //        _context.SaveChanges();

        //        // Add the total price of this order detail to the total price of the order
        //        TotalPrice = TotalPrice + orderDetail.TotalPrice;
        //    }

        //    // Set the total price of the order
        //    order.TotalPrice = TotalPrice;

        //    // Save the order to the database
        //    _context.SaveChanges();

        //    // Optionally, you can clear the cart or perform any other cleanup
        //    // For example, if the cart is stored in session, you can clear it here

        //    // Return a response indicating the successful checkout and the order ID
        //    return Ok(new { orderId = order.OrderId });
        //}

        [HttpGet("order/{orderId}")]
        public IActionResult OrderConfirmation(Guid orderId)
        {
            // Retrieve the order from the database based on orderId
            var order = _context.tblOrders
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
