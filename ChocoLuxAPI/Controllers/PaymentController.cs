using ChocoLuxAPI.DTO;
using ChocoLuxAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace ChocoLuxAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly AppDbContext _context;
        public PaymentController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("payment")]
        public IActionResult ProcessPayment(/*Guid sessionId, [FromBody]*/ PaymentDetailsDto paymentDetails)
        {
            // Validate the user from the JWT token
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized("User ID not found in token.");
            }
            var paymentSuccess = SimulatePaymentProcessing(paymentDetails);

            if (paymentSuccess)
            {
                // Determine the payment status based on the payment type
               // string paymentStatus = paymentDetails.PaymentType.Equals("Cash On Delivery", StringComparison.OrdinalIgnoreCase) ? "Pending" : "Paid";
                // Map PaymentDetailsDto to Payment entity
                var payment = new Payment
                {
                    PaymentId = Guid.NewGuid(),
                    OrderId = null, //OrderId is generated after payment process.
                    UserId = userId,
                    PaymentDate = DateTime.Now,
                    PaymentType = paymentDetails.PaymentType,
                    PaymentStatus = null
                };

                // Save the Payment entity to the database
                _context.TblPayment.Add(payment);
                _context.SaveChanges();

                return Ok(new PaymentResultDto { Success = true });
            }

            return Ok(new PaymentResultDto { Success = false });
        }

        private bool SimulatePaymentProcessing(PaymentDetailsDto paymentDetails)
        {
            // Simulate payment logic, for now, always return true
            return true;
        }

    }
}
