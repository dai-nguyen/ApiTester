/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Infrastructure.Events;
using Infrastructure.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleHeader.ViewModels
{
    public class HeaderViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private HttpClient _httpClient;
        private CancellationTokenSource _tokenSource;

        private bool _busy;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (SetProperty(ref _busy, value))
                {
                    LoginCommand.RaiseCanExecuteChanged();
                    _eventAggregator.GetEvent<BusyEvent>().Publish(new BusyModel
                    {
                        Busy = _busy
                    });
                }
            }
        }

        private string _message;
        public string Message
        {
            get { return _message; }
            set
            {
                if (SetProperty(ref _message, value))
                {
                    _eventAggregator.GetEvent<MessageEvent>().Publish(new MessageModel
                    {                        
                        Message = Message
                    });
                }
            }
        }

        private string _labelLogin;
        public string LabelLogin
        {
            get { return _labelLogin; }
            set { SetProperty(ref _labelLogin, value); }
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

        public DelegateCommand LoginCommand { get; private set; }

        public HeaderViewModel(IEventAggregator eventAggregator)
        {
            _httpClient = new HttpClient();
            LabelLogin = Infrastructure.Properties.Resources.Login;
            _eventAggregator = eventAggregator;
            LoginCommand = DelegateCommand.FromAsyncHandler(LoginAsync, CanLogin);
            LoadCached();
        }

        private async Task LoginAsync()
        {
            string err = "";
            Token = "";

            try
            {
                if (LabelLogin == Infrastructure.Properties.Resources.Cancel
                    && _tokenSource != null 
                    && !_tokenSource.IsCancellationRequested)
                {                    
                    _tokenSource.Cancel();                                        
                    return;
                }

                LabelLogin = Infrastructure.Properties.Resources.Cancel;
                Message = Infrastructure.Properties.Resources.Wait;
                Busy = true;

                _tokenSource = new CancellationTokenSource();
                
                if (_httpClient != null)
                {
                    _httpClient.Dispose();
                    _httpClient = null;
                }

                _httpClient = new HttpClient();
                _httpClient.BaseAddress = new Uri(BaseAddress);
                _httpClient.DefaultRequestHeaders.Accept.Clear();
                HttpContent requestContent = new StringContent(
                    string.Format("grant_type=password&username={0}&password={1}", UserID, Password),
                    Encoding.UTF8, "application/x-www-form-urlencoded");

                var response = await _httpClient.PostAsync(TokenEndpoint, requestContent, _tokenSource.Token);

                if (!response.IsSuccessStatusCode)
                    return;

                var token = await response.Content.ReadAsAsync<TokenModel>(_tokenSource.Token);
                Token = token.AccessToken;
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

                _eventAggregator.GetEvent<HttpClientEvent>().Publish(_httpClient);

            }
            catch (Exception ex)
            {
                err = ex.Message ?? Infrastructure.Properties.Resources.Error;
            }
            finally
            {                
                if (_tokenSource != null)
                {
                    _tokenSource.Dispose();
                    _tokenSource = null;
                }

                LabelLogin = Infrastructure.Properties.Resources.Login;
                Message = string.IsNullOrEmpty(err) ? Infrastructure.Properties.Resources.Ready : err;
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
                !string.IsNullOrWhiteSpace(Password);                
        }

        private void LoadCached()
        {
            string filename = Path.Combine(Environment.CurrentDirectory, 
                Infrastructure.Properties.Resources.LoginCachedFile);

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
            string filename = Path.Combine(Environment.CurrentDirectory, 
                Infrastructure.Properties.Resources.LoginCachedFile);

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
