using System;
using System.Threading.Tasks;
using Foundation;
using NHub.Data;
using NHub.Services;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(NHub.iOS.Services.PushNotificationService))]

namespace NHub.iOS.Services
{
    public class PushNotificationService : IPushNotificationService
    {
        public DeviceInstallation GetDeviceRegistration(params string[] tags)
        {
            if (AppDelegate.PushDeviceToken == null)
            {
                return null;
            }

            // Format our app install information for NH
            var registrationId = AppDelegate.PushDeviceToken.Description
                .Trim('<', '>').Replace(" ", string.Empty).ToUpperInvariant();

            var installation = new DeviceInstallation
            {
                InstallationId = UIDevice.CurrentDevice.IdentifierForVendor.ToString(),
                Platform = "apns",
                PushChannel = registrationId
            };
            // Set up tags to request
            installation.Tags.AddRange(tags);
            // Set up templates to request
            PushTemplate genericTemplate = new PushTemplate
            {
                Body = "{\"aps\":{\"alert\":\"$(messageParam)\"}}"
            };
            PushTemplate silentTemplate = new PushTemplate
            {
                Body = "{\"aps\":{\"content-available\":\"$(silentMessageParam)\", \"sound\":\"\"},\"action\": \"$(actionParam)\"}"
            };
            installation.Templates.Add("genericTemplate", genericTemplate);
            installation.Templates.Add("silentTemplate", silentTemplate);

            return installation;
        }

        public string GetDeviceId()
        {
            return UIDevice.CurrentDevice.IdentifierForVendor.ToString();
        }
    }
}
