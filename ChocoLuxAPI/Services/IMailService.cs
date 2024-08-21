using ChocoLuxAPI.Models;

namespace ChocoLuxAPI.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendWelcomeEmailAsync(string userName);
        Task OrderConfirmationEmailAsync(Guid orderId);
    }
}
