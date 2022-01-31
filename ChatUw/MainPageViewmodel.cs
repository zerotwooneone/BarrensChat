using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatUw.Authentication;
using ChatUw.Backend;
using ChatUw.Command;
using ChatUw.Message;
using ChatUw.NotificationHub;

namespace ChatUw
{
    public class MainPageViewmodel : ViewmodelBase
    {
        private readonly IMessageViewmodelFactory _messageViewmodelFactory;
        private readonly IRegistrationService _registrationService;
        private readonly IAuthenticationService _authenticationService;
        private readonly IBackendClient _backendClient;
        private readonly PushNotificationChannelProvider _pushNotificationChannelProvider;
        public ObservableCollection<MessageViewmodel> Messages { get; }
        public ICommand LoginCommand { get; }
        public ICommand LoadedCommand { get; }
        public ICommand SendCommand { get; set; }

        public MainPageViewmodel(IMessageViewmodelFactory messageViewmodelFactory,
            IRegistrationService registrationService,
            IAuthenticationService authenticationService,
            IBackendClient backendClient, 
            //INotificationChannelCache notificationChannelCache,
            PushNotificationChannelProvider pushNotificationChannelProvider)
        {
            _messageViewmodelFactory = messageViewmodelFactory;
            _registrationService = registrationService;
            _authenticationService = authenticationService;
            _backendClient = backendClient;
            _pushNotificationChannelProvider = pushNotificationChannelProvider;
            Messages = new ObservableCollection<MessageViewmodel>
            {
                new MessageViewmodel("message 1", true),
                new MessageViewmodel("message 2", false)
            };
            LoginCommand = new RelayCommand(LoginClicked, LoginEnabled);
            LoadedCommand = new RelayCommand(OnLoad);
            SendCommand = new RelayCommand(SendClicked, SendEnabled);
        }

        private bool SendEnabled()
        {
            var validReg = _registrationService.GetValidRegistrationFromCache() != null;
            var authenticationToken = _authenticationService.GetValidAuthModel();
            var validAuthToken = authenticationToken != null;
            return validReg ||
                   validAuthToken;
        }

        private void SendClicked()
        {
            throw new NotImplementedException();
        }

        private async void OnLoad()
        {
            if (LoginEnabled())
            {
                try
                {
                    var notificationChannel = await _pushNotificationChannelProvider.CreateNotificationChannel();
                    var auth = _authenticationService.GetValidAuthModel();
                    var reg = _registrationService.GetValidRegistrationFromCache();

                    string regId;
                    if (auth == null && reg == null)
                    {
                        var startup = await _backendClient.AppStartup(notificationChannel.Uri);
                        regId = startup.HubRegistration.RegistrationId;
                    }
                    else
                    {
                        regId = null;
                    }

                    auth = auth ?? 
                           await _authenticationService.GetAndSave3rdPartyAuth();

                    if (auth == null)
                        throw new ApplicationException("You must log in to use this app.");

                    const string someDumbUsername="someDumbUsername";

                    var register = await _backendClient.RegisterUser(notificationChannel.Uri,
                        regId,
                        someDumbUsername,
                        auth.Token);

                    if(!register.IsSuccess) throw new ApplicationException("Failed to register");

                    _registrationService.SetRegistration(register.HubRegistration.RegistrationId);
                }
                catch (Exception exception)
                {
                    Console.WriteLine(exception);
                }
            }
        }

        private bool LoginEnabled()
        {
            var noValidReg = _registrationService.GetValidRegistrationFromCache() == null;
            var authenticationToken = _authenticationService.GetValidAuthModel();
            var noValidAuthToken = authenticationToken == null;
            return noValidReg ||
                noValidAuthToken;
        }

        private async void LoginClicked()
        {
            try
            {
                var token = await EnsureUserIsLoggedIn();

                //await _registrationService.CreateAndSaveRegistration(token);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        private async Task<string> EnsureUserIsLoggedIn()
        {
            var authenticationToken = _authenticationService.GetValidAuthModel();

            if (authenticationToken != null) return authenticationToken.Token;

            var authModel = await _authenticationService.GetAndSave3rdPartyAuth();
            if (authModel == null)
                throw new ApplicationException("You must log in to use this app.");
            return authModel.Token;

        }
    }
}