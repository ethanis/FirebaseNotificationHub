using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace NHub.iOS
{
    [Register("AppDelegate")]
    public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
    {
        public static NSData PushDeviceToken { get; private set; } = null;

        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            global::Xamarin.Forms.Forms.Init();

            LoadApplication(new App());

            var settings = UIUserNotificationSettings.GetSettingsForTypes(
                UIUserNotificationType.Alert | UIUserNotificationType.Badge | UIUserNotificationType.Sound,
                new NSSet());

            UIApplication.SharedApplication.RegisterUserNotificationSettings(settings);
            UIApplication.SharedApplication.RegisterForRemoteNotifications();

            return base.FinishedLaunching(app, options);
        }

        public override void RegisteredForRemoteNotifications(UIApplication application, NSData deviceToken)
        {
            AppDelegate.PushDeviceToken = deviceToken;
        }

        public override void FailedToRegisterForRemoteNotifications(
            UIApplication application,
            NSError error)
        {
            // TODO: Show error
        }

        public override void DidReceiveRemoteNotification(
            UIApplication application,
            NSDictionary userInfo,
            Action<UIBackgroundFetchResult> completionHandler)
        {
            NSDictionary aps = userInfo.ObjectForKey(new NSString("aps")) as NSDictionary;

            var messageKey = new NSString("alert");
            var silentMessageKey = new NSString("content-available");
            var actionKey = new NSString("action");

            string message = null;
            if (aps.ContainsKey(messageKey))
                message = (aps[messageKey] as NSString).ToString();

            // Show alert
            if (!string.IsNullOrEmpty(message))
            {
                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    var alert = UIAlertController.Create(
                        "Notification",
                        message,
                        UIAlertControllerStyle.Alert);

                    alert.AddAction(UIAlertAction.Create("Ok", UIAlertActionStyle.Default, null));

                    var vc = UIApplication.SharedApplication.KeyWindow.RootViewController;
                    while (vc.PresentedViewController != null)
                    {
                        vc = vc.PresentedViewController;
                    }

                    vc.ShowDetailViewController(alert, vc);
                });
            }

            // If message template is silent message, parse action param
            if (aps.ContainsKey(silentMessageKey))
            {
                System.Diagnostics.Debug.WriteLine($"[PNS] Silent message received");
                var action = userInfo.ObjectForKey(new NSString("action")) as NSString;

                System.Diagnostics.Debug.WriteLine($"[PNS] Action required of type: {action}");
            }
        }
    }
}
