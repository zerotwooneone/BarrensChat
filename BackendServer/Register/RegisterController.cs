using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using BackendServer.HubClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.NotificationHubs;
using Microsoft.Azure.NotificationHubs.Messaging;

namespace BackendServer.Register
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly HubClientFactory _hubClientFactory;

        public RegisterController(HubClientFactory hubClientFactory)
        {
            _hubClientFactory = hubClientFactory;
        }

        public class DeviceRegistration
        {
            public string Platform { get; set; }
            public string Handle { get; set; }
            public string[] Tags { get; set; }
        }

        [HttpPost]
        // POST api/register
        // This creates a registration id
        public async Task<string> Post(string handle = null)
        {
            string newRegistrationId = null;
            var hub = _hubClientFactory.Primary;

            // make sure there are no existing registrations for this push handle (used for iOS and Android)
            if (handle != null)
            {
                var registrations = await hub.GetRegistrationsByChannelAsync(handle, 100);

                foreach (RegistrationDescription registration in registrations)
                {
                    if (newRegistrationId == null)
                    {
                        newRegistrationId = registration.RegistrationId;
                    }
                    else
                    {
                        await hub.DeleteRegistrationAsync(registration);
                    }
                }
            }

            if (newRegistrationId == null)
            {
                newRegistrationId = await hub.CreateRegistrationIdAsync();
            }

            return newRegistrationId;
        }

        [HttpPut]
        [Route("{id}")]
        // PUT api/register/5
        // This creates or updates a registration (with provided channelURI) at the specified id
        public async Task<IActionResult> Put(string id, DeviceRegistration deviceUpdate)
        {
            RegistrationDescription registration;
            switch (deviceUpdate.Platform)
            {
                case "mpns":
                    registration = new MpnsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "wns":
                    registration = new WindowsRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "apns":
                    registration = new AppleRegistrationDescription(deviceUpdate.Handle);
                    break;
                case "fcm":
                    registration = new FcmRegistrationDescription(deviceUpdate.Handle);
                    break;
                default:
                    return BadRequest();
            }

            registration.RegistrationId = id;
            var username = HttpContext.User.Identity.Name;

            // add check if user is allowed to add these tags
            registration.Tags = new HashSet<string>(deviceUpdate.Tags);
            registration.Tags.Add("username:" + username);

            try
            {
                var hub = _hubClientFactory.Primary;
                await hub.CreateOrUpdateRegistrationAsync(registration);
            }
            catch (MessagingException e)
            {
                var webex = e.InnerException as WebException;
                if (webex.Status == WebExceptionStatus.ProtocolError)
                {
                    var response = (HttpWebResponse)webex.Response;
                    if (response.StatusCode == HttpStatusCode.Gone)
                        return StatusCode((int)HttpStatusCode.Gone);
                }
            }

            return Ok();
        }

        [HttpDelete]
        // DELETE api/register/5
        public async Task<IActionResult> Delete(string id)
        {
            var hub = _hubClientFactory.Primary;
            await hub.DeleteRegistrationAsync(id);
            return Ok();
        }

        
    }
}