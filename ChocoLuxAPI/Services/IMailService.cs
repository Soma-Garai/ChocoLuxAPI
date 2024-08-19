using ChocoLuxAPI.Models;

namespace ChocoLuxAPI.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
        Task SendWelcomeEmailAsync(WelcomeRequest request);
        Task OrderConfirmationEmailAsync(MailOrderConfirmation request);
    }
}
