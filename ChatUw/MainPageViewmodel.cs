using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ChatUw.Authentication;
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
        public ObservableCollection<MessageViewmodel> Messages { get; }
        public ICommand LoginCommand { get; }
        public ICommand LoadedCommand { get; }
        public ICommand SendCommand { get; set; }

        public MainPageViewmodel(IMessageViewmodelFactory messageViewmodelFactory,
            IRegistrationService registrationService,
            IAuthenticationService authenticationService)
        {
            _messageViewmodelFactory = messageViewmodelFactory;
            _registrationService = registrationService;
            _authenticationService = authenticationService;
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
                    var auth = _authenticationService.GetValidAuthModel() ?? 
                               await _authenticationService.GetAndSave3rdPartyAuth();

                    if (auth == null)
                        throw new ApplicationException("You must log in to use this app.");

                    var reg = await _registrationService.CreateAndSaveRegistration(auth.Token);

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

                await _registrationService.CreateAndSaveRegistration(token);
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