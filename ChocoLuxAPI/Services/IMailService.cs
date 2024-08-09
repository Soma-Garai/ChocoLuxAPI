using ChocoLuxAPI.Models;

namespace ChocoLuxAPI.Services
{
    public interface IMailService
    {
        Task SendEmailAsync(MailRequest mailRequest);
    }
}
