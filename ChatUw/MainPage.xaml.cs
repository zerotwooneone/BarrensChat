using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Net.Http;
using Windows.Storage;
using System.Net.Http.Headers;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Networking.PushNotifications;
using Auth0.OidcClient;
using ChatUw.Http;
using ChatUw.Settings;
using Newtonsoft.Json;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace ChatUw
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void PushClick(object sender, RoutedEventArgs e)
        {
            await sendPush(ToUserTagTextBox.Text, this.NotificationMessageTextBox.Text);
        }
        
        private async Task sendPush(string userTag, string message)
        {
            var httpClientFactory = new LocalHostTestHttpClientFactory();
            var settingsCache = SettingsCache.GetInstance();
            var POST_URL = $"{MagicValues.BackendUrl}/api/Publish";

            var publishModel = new PublishModel
            {
                message = message,
                to_tag = userTag
            };

            using (var httpClient = httpClientFactory.CreateHttpClient(settingsCache.GetAuthenticationToken()))
            {
                try
                {
                    var json = JsonConvert.SerializeObject(publishModel);
                    var content = new StringContent(json,
                        System.Text.Encoding.UTF8, "application/json");
                    var response = await httpClient.PostAsync(POST_URL, content);
                }
                catch (Exception ex)
                {
                    MessageDialog alert = new MessageDialog(ex.Message, "Failed to send message");
                    await alert.ShowAsync();
                }
            }
        }
    }
}
