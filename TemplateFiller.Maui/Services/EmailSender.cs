namespace TemplateFiller.Maui.Services
{
    public partial class EmailSender : IEmailSender
    {
        public partial Task SendEmailAsync(string fileName);
    }
}
