using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Azure.NotificationHubs;
using NHub.Data;
using Microsoft.Azure.Mobile.Server.Config;

namespace NHub.Server.Controllers
{
    [MobileAppController]
    public class RegisterController : ApiController
    {
        private NotificationHubClient hub;
        private NotificationHubClient Hub => hub ?? (hub = Helpers.Notifications.GetHub(Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings()));

        // PUT api/register
        [HttpPut]
        public async Task<HttpResponseMessage> Put([FromBody]DeviceInstallation deviceUpdate)
        {
            if (deviceUpdate == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            Dictionary<string, InstallationTemplate> templates = new Dictionary<string, InstallationTemplate>();
            foreach (var t in deviceUpdate.Templates)
            {
                templates.Add(t.Key, new InstallationTemplate { Body = t.Value.Body });
            }

            Installation installation = new Installation()
            {
                InstallationId = deviceUpdate.InstallationId,
                PushChannel = deviceUpdate.PushChannel,
                Tags = deviceUpdate.Tags,
                Templates = templates
            };

            switch (deviceUpdate.Platform)
            {
                case "apns":
                    installation.Platform = NotificationPlatform.Apns;
                    break;
                case "gcm":
                    installation.Platform = NotificationPlatform.Gcm;
                    break;
                default:
                    throw new HttpResponseException(HttpStatusCode.BadRequest);
            }

            // In the backend we can control if a user is allowed to add tags
            installation.Tags = new List<string>(deviceUpdate.Tags);

            try
            {
                await Hub.CreateOrUpdateInstallationAsync(installation);
                return Request.CreateResponse(true);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PNS Error]: Issue registering device {ex.Message}");
                return Request.CreateResponse(false);
            }
        }

        // DELETE api/register/5
        [HttpDelete]
        public async Task<HttpResponseMessage> Delete(string id)
        {
            try
            {
                await Hub.DeleteInstallationAsync(id);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[PNS Error]: Issue deleting device installation {ex.Message}");
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
