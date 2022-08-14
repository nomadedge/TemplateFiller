using System;
using System.IO;
using Windows.ApplicationModel.AppService;
using Windows.ApplicationModel.Background;
using Windows.ApplicationModel.Email;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;

namespace TemplateFiller.UwpService
{
    public sealed class EmailService : IBackgroundTask
    {
        private BackgroundTaskDeferral _backgroundTaskDeferral;
        private AppServiceConnection _appServiceconnection;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral so that the service isn't terminated.
            _backgroundTaskDeferral = taskInstance.GetDeferral();

            // Associate a cancellation handler with the background task.
            taskInstance.Canceled += OnTaskCanceled;

            // Retrieve the app service connection and set up a listener for incoming app service requests.
            var details = taskInstance.TriggerDetails as AppServiceTriggerDetails;
            _appServiceconnection = details.AppServiceConnection;
            _appServiceconnection.RequestReceived += OnRequestReceived;
        }

        private async void OnRequestReceived(AppServiceConnection sender, AppServiceRequestReceivedEventArgs args)
        {
            // This function is called when the app service receives a request.
            var messageDeferral = args.GetDeferral();
            ValueSet message = args.Request.Message;
            ValueSet returnData = new ValueSet();

            try
            {
                var requestType = message["RequestType"] as string;
                var localFolder = ApplicationData.Current.LocalFolder.Path;
                if (requestType == "Storage")
                {
                    returnData.Add("Location", localFolder);
                }
                else
                {
                    var shortName = message["FileName"] as string;
                    var fileName = Path.Combine(localFolder, shortName);
                    var emailMessage = new EmailMessage();
                    var file = await StorageFile.GetFileFromPathAsync(fileName);
                    var fileReference = RandomAccessStreamReference.CreateFromFile(file);
                    emailMessage.Attachments.Add(new EmailAttachment(fileName, fileReference));
                    await EmailManager.ShowComposeNewEmailAsync(emailMessage);
                }

                // Return the data to the caller.
                await args.Request.SendResponseAsync(returnData);
            }
            catch (Exception e)
            {
                // Your exception handling code here.
            }
            finally
            {
                // Complete the deferral so that the platform knows that we're done responding to the app service call.
                // Note for error handling: this must be called even if SendResponseAsync() throws an exception.
                messageDeferral.Complete();
            }
        }

        private void OnTaskCanceled(IBackgroundTaskInstance sender, BackgroundTaskCancellationReason reason)
        {
            if (_backgroundTaskDeferral != null)
            {
                // Complete the service deferral.
                _backgroundTaskDeferral.Complete();
            }
        }
    }
}
