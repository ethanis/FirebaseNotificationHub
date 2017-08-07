using System;
using System.Net.Http;
using System.Threading.Tasks;
using NHub.Data;

namespace NHub.Services
{
    public class NotificationRegistrationService : BaseService
    {
        private static NotificationRegistrationService instance;
        public static NotificationRegistrationService Instance => instance ?? (instance = new NotificationRegistrationService());

        private NotificationRegistrationService()
            : base((HttpClient client) => client.DefaultRequestHeaders.Add("ZUMO-API-VERSION", "2.0.0"))
        { }

        // TODO: Update with base address of server
        protected override string BaseAddress => "";

        public Task<bool> RegisterDeviceAsync(params string[] tags)
        {
            // Resolve dep with whatever IOC container
            var pushNotificationService = Xamarin.Forms.DependencyService.Get<IPushNotificationService>();

            // Get our registration information
            var deviceInstallation = pushNotificationService?.GetDeviceRegistration(tags);

            if (deviceInstallation == null)
                return Task.FromResult(true);

            // Put the device information to the server
            return PutAsync<bool, DeviceInstallation>("api/register", deviceInstallation);
        }

        public Task DeregisterDeviceAsync()
        {
            var pushNotificationService = Xamarin.Forms.DependencyService.Get<IPushNotificationService>();

            // Get device installationid for notification hub
            var deviceId = pushNotificationService.GetDeviceId();

            if (deviceId == null)
                return Task.FromResult(false);

            // Delete that installation id from our NH
            return DeleteAsync<object>($"api/register/{deviceId}");
        }
    }
}
