using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Tracing;
using Microsoft.Azure.NotificationHubs;
using NHub.Data;
using Microsoft.Azure.Mobile.Server.Config;

namespace NHub.Server.Controllers
{
    [MobileAppController]
    public class RequestPushController : ApiController
    {
        private NotificationHubClient hub;
        private NotificationHubClient Hub => hub ?? (hub = Helpers.Notifications.GetHub(Configuration.GetMobileAppSettingsProvider().GetMobileAppSettings()));

        // POST api/requestpush
        [HttpPost]
        public async Task<HttpResponseMessage> Post([FromBody]PushRequest pushRequest)
        {
            if (pushRequest == null)
                return Request.CreateResponse(HttpStatusCode.BadRequest);

            Dictionary<string, string> templateParams = new Dictionary<string, string>();

            if (pushRequest.Silent)
            {
                templateParams["silentMessageParam"] = "1";
                templateParams["actionParam"] = pushRequest.Action;
            }
            else
            {
                templateParams["messageParam"] = pushRequest.Text;
            }

            try
            {
                System.Diagnostics.Trace.WriteLine($"Tags: {string.Join(" || ", pushRequest.Tags)}");

                // Send the push notification and log the results.
                var result = await Hub.SendTemplateNotificationAsync(templateParams, string.Join(" || ", pushRequest.Tags));

                // Write the success result to the logs.
                System.Diagnostics.Trace.WriteLine($"Outcome: {result.State.ToString()}");

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                // Write the failure result to the logs.
                System.Diagnostics.Trace.WriteLine($"Push.SendAsync Error: {ex.Message}");

                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
