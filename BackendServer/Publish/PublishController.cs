using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BackendServer.HubClient;
using Microsoft.AspNetCore.Authorization;

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
            var user = HttpContext.User.Identity.Name;
            string[] userTag = new string[2];
            userTag[0] = "username:" + publishModel.to_tag;
            userTag[1] = "from:" + user;

            Microsoft.Azure.NotificationHubs.NotificationOutcome outcome = null;
            
            switch (publishModel.pns?.ToLower())
            {
                case "wns":
                    // Windows 8.1 / Windows Phone 8.1
                    var toast = @"<toast><visual><binding template=""ToastText01""><text id=""1"">" + 
                                "From " + user + ": " + publishModel.message + "</text></binding></visual></toast>";
                    outcome = await _hubClientFactory.Primary.SendWindowsNativeNotificationAsync(toast, userTag);
                    break;
                case "apns":
                    // iOS
                    var alert = "{\"aps\":{\"alert\":\"" + "From " + user + ": " + publishModel.message + "\"}}";
                    outcome = await _hubClientFactory.Primary.SendAppleNativeNotificationAsync(alert, userTag);
                    break;
                case "fcm":
                    // Android
                    var notif = "{ \"data\" : {\"message\":\"" + "From " + user + ": " + publishModel.message + "\"}}";
                    outcome = await _hubClientFactory.Primary.SendFcmNativeNotificationAsync(notif, userTag);
                    break;
                default:
                    return BadRequest("pns type incorrect");
            }

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

    public class PublishModel   
    {
        public string pns { get; set; }
        public string  message { get; set; } 
        public string  to_tag { get; set; }
    }
}