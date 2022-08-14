using Windows.ApplicationModel.AppService;
using Windows.Foundation.Collections;

namespace TemplateFiller.Maui.Services
{
    public partial class EmailSender : IEmailSender
    {
        public partial async Task SendEmailAsync(string fileName)
        {
            using (var emailService = new AppServiceConnection())
            {
                emailService.AppServiceName = "com.microsoft.templateFillerUwp";
                emailService.PackageFamilyName = "c07f06c0-369e-4c1b-9c3d-4da7006f322e_sftzr32grnbyw";
                var status = await emailService.OpenAsync();
                if (status != AppServiceConnectionStatus.Success)
                {
                    throw new Exception("Email can't be send");
                }

                var storageMessage = new ValueSet();
                storageMessage.Add("RequestType", "Storage");
                var storageResponse = await emailService.SendMessageAsync(storageMessage);
                if (storageResponse.Status != AppServiceResponseStatus.Success)
                {
                    throw new Exception("Email can't be send");
                }
                var shortName = Path.GetFileName(fileName);
                var storageFolder = storageResponse.Message["Location"] as string;
                var copyName = Path.Combine(storageFolder, shortName);
                File.Copy(fileName, copyName, true);

                var message = new ValueSet();
                message.Add("RequestType", "Send");
                message.Add("FileName", shortName);
                var response = await emailService.SendMessageAsync(message);
                if (File.Exists(copyName))
                {
                    File.Delete(copyName);
                }
                if (response.Status != AppServiceResponseStatus.Success ||
                    !response.Message.TryGetValue("Result", out object result) ||
                    (result as string) != "Success")
                {
                    throw new Exception("Email can't be send");
                }
            }
        }
    }
}
