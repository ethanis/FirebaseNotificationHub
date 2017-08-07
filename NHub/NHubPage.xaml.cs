using System.Threading.Tasks;
using NHub.Services;
using Xamarin.Forms;

namespace NHub
{
    public partial class NHubPage : ContentPage
    {
        public NHubPage()
        {
            InitializeComponent();

            registerButton.Clicked += OnRegisterButtonClicked;
            deregisterButton.Clicked += OnDeregisterButtonClicked;
        }

        void OnRegisterButtonClicked(object sender, System.EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    var result = await NotificationRegistrationService.Instance.RegisterDeviceAsync();
                    if (!result)
                    {
                        System.Diagnostics.Debug.WriteLine("Error registering with notification hub");
                    }
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PushNotificationError]: Device registration failed with error {ex.Message}");
                }
            });
        }

        void OnDeregisterButtonClicked(object sender, System.EventArgs e)
        {
            Task.Run(async () =>
            {
                try
                {
                    await NotificationRegistrationService.Instance.DeregisterDeviceAsync();
                    System.Diagnostics.Debug.WriteLine("Should be deregistered");
                }
                catch (System.Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[PushNotificationError]: Device deregistration failed with error {ex.Message}");
                }
            });
        }
    }
}
