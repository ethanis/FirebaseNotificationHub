using System;
using Android.App;
using Firebase.Iid;
using Android.Util;
using System.Threading.Tasks;

namespace NHub.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.INSTANCE_ID_EVENT" })]
    public class FirebaseIIDService : FirebaseInstanceIdService
    {
        const string TAG = "MyFirebaseIIDService";
        public override void OnTokenRefresh()
        {
            // Called by Firebase when token is refreshed
            var refreshedToken = FirebaseInstanceId.Instance.Token;
            Log.Debug(TAG, "Refreshed token: " + refreshedToken);
            SendRegistrationToServer(refreshedToken);
        }
        void SendRegistrationToServer(string token)
        {
            // Update registration to notification hub with updated token
            Task.Run(async () =>
            {
                await NHub.Services.NotificationRegistrationService.Instance.RegisterDeviceAsync();
            });
        }
    }
}