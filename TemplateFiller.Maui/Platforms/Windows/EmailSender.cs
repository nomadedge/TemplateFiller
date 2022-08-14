using System.Diagnostics;

namespace TemplateFiller.Maui.Services
{
    public partial class EmailSender : IEmailSender
    {
        public partial async Task SendEmailAsync(string fileName)
        {
            if (File.Exists(fileName))
            {
                Process.Start("explorer.exe", $"/select,\"{fileName}\"");
                await Launcher.OpenAsync(new Uri("mailto:"));
            }
        }
    }
}
