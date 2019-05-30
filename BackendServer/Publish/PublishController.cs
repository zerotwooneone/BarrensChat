using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackendServer.HubClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Azure.NotificationHubs;
using Newtonsoft.Json;

namespace BackendServer.Publish
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishController : ControllerBase
    {
        private readonly HubClientFactory _hubClientFactory;

        public PublishController(HubClientFactory hubClientFactory)
        {
            _hubClientFactory = hubClientFactory;
        }

        [HttpPost]
        public async Task<IActionResult> Post(PublishModel publishModel)
        {
            const int arbitraryLengthLimit = 240;
            if (string.IsNullOrWhiteSpace(publishModel.to_tag)) return BadRequest("Must provide a target user");
            if (publishModel.to_tag.Length > arbitraryLengthLimit) return BadRequest("Target user name invalid");
            if (string.IsNullOrWhiteSpace(publishModel.message)) return BadRequest("Must provide a message");
            if (publishModel.message.Length > arbitraryLengthLimit) return BadRequest("Message too long");

            var user = HttpContext.User.Identity.Name;
            string[] userTag = new string[2];
            userTag[0] = "username:" + publishModel.to_tag;
            userTag[0] = "nickname:" + publishModel.to_tag; //this is a hack and should be replaced
            userTag[1] = "from:" + user;

            var rawNotification = new RawNotificationModel
            {
                Message = publishModel.message
            };
            var rawJson = JsonConvert.SerializeObject(rawNotification);

            NotificationOutcome outcome;
            
            // Windows 8.1 / Windows Phone 8.1
            var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" +
                        "From " + user + ": " + publishModel.message + "</text></binding></visual></toast>";
            outcome = await _hubClientFactory.Primary.SendWindowsNativeNotificationAsync(toast, userTag);

            IDictionary<string, string> wnsHeaders = new Dictionary<string, string> { { "X-WNS-Type", "wns/raw" } };
            var windowsNotification = new WindowsNotification(rawJson, wnsHeaders);
            var windowsRawOutcome = await _hubClientFactory.Primary.SendNotificationAsync(
                windowsNotification, userTag);
            
            //    // iOS
            //    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + publishModel.message + "\"}}";
            //    outcome = await _hubClientFactory.Primary.SendAppleNativeNotificationAsync(alert, userTag);
            //    break;
            
            // Android
            var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + publishModel.message + "\"}}";
            outcome = await _hubClientFactory.Primary.SendFcmNativeNotificationAsync(notif, userTag);
            var fcmRawOutcome = await _hubClientFactory.Primary.SendNotificationAsync(
                new FcmNotification(rawJson), userTag);
            

            if (outcome != null)
            {
                if (!((outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Abandoned) ||
                      (outcome.State == Microsoft.Azure.NotificationHubs.NotificationOutcomeState.Unknown)))
                {
                    return Ok();
                }
            }

            return StatusCode((int)HttpStatusCode.InternalServerError);
        }
    }
}