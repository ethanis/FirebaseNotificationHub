using System;
using System.Threading.Tasks;
using NHub.Data;

namespace NHub.Services
{
    public interface IPushNotificationService
    {
        DeviceInstallation GetDeviceRegistration(params string[] tags);
        string GetDeviceId();
    }
}
