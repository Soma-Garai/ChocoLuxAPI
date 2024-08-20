using ChocoLuxAPI.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;

namespace ChocoLuxAPI.Services
{
    public class MailService :IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly AppDbContext _context;
        public MailService(IOptions<MailSettings> mailSettings, AppDbContext context)
        {
            _mailSettings = mailSettings.Value;    //to access the data from the appsettings.json at runtime
            _context = context;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;
            var builder = new BodyBuilder();
            if (mailRequest.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailRequest.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        public async Task SendWelcomeEmailAsync(WelcomeRequest request)
        {
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "WelcomeTemplate.cshtml");
            StreamReader str = new StreamReader(FilePath);
            string MailText = str.ReadToEnd();
            str.Close();
            MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(request.ToEmail));
            email.Subject = $"Welcome {request.UserName}";
            var builder = new BodyBuilder();
            builder.HtmlBody = MailText;
            email.Body = builder.ToMessageBody();
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

        //public async Task OrderConfirmationEmailAsync(MailOrderConfirmation request)
        //{
        //    string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "OrderConfirmation.cshtml");
        //    StreamReader str = new StreamReader(FilePath);
        //    string MailText = str.ReadToEnd();
        //    str.Close();
        //    MailText = MailText.Replace("[username]", request.UserName).Replace("[email]", request.ToEmail);
        //    var email = new MimeMessage();
        //    email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
        //    email.To.Add(MailboxAddress.Parse(request.ToEmail));
        //    email.Subject = $"Welcome {request.UserName}";
        //    var builder = new BodyBuilder();
        //    builder.HtmlBody = MailText;
        //    email.Body = builder.ToMessageBody();
        //    using var smtp = new SmtpClient();
        //    smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
        //    smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
        //    await smtp.SendAsync(email);
        //    smtp.Disconnect(true);
        //}
        public async Task OrderConfirmationEmailAsync(Guid orderId)
        {
            // Retrieve the order from the database based on orderId
            var order = _context.tblOrders
                              .Include(o => o.OrderDetails) // Include OrderDetails for this order
                              .FirstOrDefault(o => o.OrderId == orderId);
            if (order == null)
            {
                throw new Exception("Order not found");
            }
            var payment = _context.TblPayment.FirstOrDefault(p => p.OrderId == orderId);

            if (payment == null)
            {
                throw new Exception("Payment details not found");
            }
            // Load the email template
            string FilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", "OrderConfirmation.cshtml");
            string MailText;

            using (StreamReader str = new StreamReader(FilePath))
            {
                MailText = await str.ReadToEndAsync();
            }
            
            // Replace placeholders with actual values
            MailText = MailText
                .Replace("[username]", "Soma") 
                .Replace("[email]", "soma@pitangent.com") 
                .Replace("[OrderId]", order.OrderId.ToString())
                .Replace("[OrderDate]", order.OrderDate.ToString("yyyy-MM-dd HH:mm:ss"))
                .Replace("[OrderStatus]", "Order Confirmed")
                .Replace("[PaymentStatus]",payment.PaymentStatus /*== "Cash On Delivery" ? "Pending" : $"Paid by {order.Payment.PaymentStatus}"*/)
                .Replace("[TotalPrice]", order.TotalPrice.ToString());

            // Build the order items table
            string orderItemsHtml = "";
            foreach (var item in order.OrderDetails)
            {
                orderItemsHtml += $"<tr><td>{item.ProductName}</td><td>{item.Quantity}</td><td>{item.ProductPrice:C}</td><td>{item.TotalPrice:C}</td></tr>";
            }
            MailText = MailText.Replace("[OrderItems]", orderItemsHtml);

            // Create email message
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse("soma@pitangent.com")); // Assuming the order has the user's email
            email.Subject = $"Order Confirmation - Order #{order.OrderId}";

            var builder = new BodyBuilder
            {
                HtmlBody = MailText
            };

            email.Body = builder.ToMessageBody();

            // Send the email
            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }

    }
}
