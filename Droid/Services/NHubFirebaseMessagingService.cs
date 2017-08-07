using System;
using Android.App;
using Android.Content;
using Android.Media;
using Android.Util;
using Firebase.Messaging;

namespace NHub.Droid.Services
{
    [Service]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class NHubFirebaseMessagingService : FirebaseMessagingService
    {
        const string TAG = "NHubFirebaseMsgService";
        public override void OnMessageReceived(RemoteMessage message)
        {
            Log.Debug(TAG, $"From: {message.From}");

            // Check if notification should be silent
            if (message.Data.ContainsKey("silent"))
            {
                var action = message.Data["action"];
                Log.Debug(TAG, $"Notification Message Action: {action}");
                PerformSilentNotification(action);
            }
            else
            {
                // Pull message body out of the template we registered with
                var messageBody = message.Data["message"];
                if (string.IsNullOrWhiteSpace(messageBody))
                    return;

                Log.Debug(TAG, $"Notification Message Body: {messageBody}");
                SendNotification(messageBody);
            }
        }

        void PerformSilentNotification(string action)
        {
            System.Diagnostics.Debug.WriteLine($"[PNS] Perform action of type: {action}");
        }

        void SendNotification(string messageBody)
        {
            // Display notification however necessary
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(this, 0, intent, PendingIntentFlags.OneShot);

            var notificationBuilder = new Notification.Builder(this)
                .SetSmallIcon(Resource.Drawable.icon)
                .SetContentTitle("FCM Message")
                .SetContentText(messageBody)
                .SetAutoCancel(true)
                .SetContentIntent(pendingIntent);

            var notificationManager = NotificationManager.FromContext(this);
            notificationManager.Notify(0, notificationBuilder.Build());
        }
    }
}
