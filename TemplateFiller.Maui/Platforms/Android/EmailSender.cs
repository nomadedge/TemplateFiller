namespace TemplateFiller.Maui.Services
{
    public partial class EmailSender : IEmailSender
    {
        public partial async Task SendEmailAsync(string fileName)
        {
            var message = new EmailMessage();
            message.Attachments.Add(new EmailAttachment(fileName));
            await Email.Default.ComposeAsync(message);
        }
    }
}
