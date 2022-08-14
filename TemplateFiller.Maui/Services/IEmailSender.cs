namespace TemplateFiller.Maui.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string fileName);
    }
}