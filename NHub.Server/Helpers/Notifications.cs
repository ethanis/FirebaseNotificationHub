using System;
using Microsoft.Azure.NotificationHubs;
using System.Configuration;
using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Config;

namespace NHub.Server.Helpers
{
    public static class Notifications
    {
        private static NotificationHubClient client;

        public static NotificationHubClient GetHub(MobileAppSettingsDictionary settings)
        {
            if (client == null)
            {
                // Get the Notification Hubs credentials.
                string notificationHubName = settings.NotificationHubName;
                string notificationHubConnection = settings
                    .Connections[MobileAppSettingsKeys.NotificationHubConnectionString].ConnectionString;

                // Create a new Notification Hub client.
                client = NotificationHubClient.CreateClientFromConnectionString(notificationHubConnection, notificationHubName);
            }

            return client;
        }
    }
}
