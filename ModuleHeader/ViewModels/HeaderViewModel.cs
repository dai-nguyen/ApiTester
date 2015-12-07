using Infrastructure.Events;
using Infrastructure.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ModuleHeader.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private bool _busy;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (SetProperty(ref _busy, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                    _eventAggregator.GetEvent<BusyEvent>().Publish(new BusyModel { Busy = _busy, Message = Message });
                }
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        private string _baseAddress;
        public string BaseAddress
        {
            get { return _baseAddress; }
            set
            {
                if (SetProperty(ref _baseAddress, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _tokenEndpoint;
        public string TokenEndpoint
        {
            get { return _tokenEndpoint; }
            set
            {
                if (SetProperty(ref _tokenEndpoint, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _token;
        public string Token
        {
            get { return _token; }
            set { SetProperty(ref _token, value); }
        }

        private string _userId;
        public string UserID
        {
            get { return _userId; }
            set
            {
                if (SetProperty(ref _userId, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        private string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                if (SetProperty(ref _password, value))
                    LoginCommand.RaiseCanExecuteChanged();
            }
        }

        public DelegateCommand LoginCommand { get; set; }

        public HeaderViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            LoginCommand = DelegateCommand.FromAsyncHandler(LoginAsync, CanLogin);
            LoadCached();
        }

        public async Task LoginAsync()
        {
            string err = "";

            try
            {
                Message = "Please wait...";
                Busy = true;
                
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(BaseAddress);
                    client.DefaultRequestHeaders.Accept.Clear();
                    HttpContent requestContent = new StringContent(
                        string.Format("grant_type=password&username={0}&password={1}", UserID, Password),
                        Encoding.UTF8, "application/x-www-form-urlencoded");

                    var response = await client.PostAsync(TokenEndpoint, requestContent);

                    if (!response.IsSuccessStatusCode)
                        return;

                    var token = await response.Content.ReadAsAsync<TokenModel>();
                    Token = token.AccessToken;
                }
            }
            catch (Exception ex)
            {
                err = ex.Message ?? "Error";
            }
            finally
            {
                Message = string.IsNullOrEmpty(err) ? "Done" : err;
                Busy = false;                
            }

            if (string.IsNullOrEmpty(err))
                SaveCached();
        }

        private bool CanLogin()
        {
            return !string.IsNullOrEmpty(BaseAddress) &&
                !string.IsNullOrWhiteSpace(BaseAddress) &&
                !string.IsNullOrEmpty(TokenEndpoint) &&
                !string.IsNullOrWhiteSpace(TokenEndpoint) &&
                !string.IsNullOrEmpty(UserID) &&
                !string.IsNullOrWhiteSpace(UserID) &&
                !string.IsNullOrEmpty(Password) &&
                !string.IsNullOrWhiteSpace(Password) &&
                !Busy;
        }

        private void LoadCached()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "LoginCached.json");

            if (File.Exists(filename))
            {
                try
                {
                    var loginModel = JsonConvert.DeserializeObject<LoginModel>(File.ReadAllText(filename));
                    BaseAddress = loginModel.BaseAddress;
                    TokenEndpoint = loginModel.TokenEndpoint;
                    UserID = loginModel.UserID;
                }
                catch { }
            }
        }

        private void SaveCached()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, "LoginCached.json");

            try
            {                
                var loginModel = new LoginModel
                {
                    BaseAddress = BaseAddress,
                    TokenEndpoint = TokenEndpoint,
                    UserID = UserID
                };

                File.WriteAllText(filename, JsonConvert.SerializeObject(loginModel, Formatting.Indented));
            }
            catch { }
        }
    }
}
