using Infrastructure.Enums;
using Infrastructure.Events;
using Infrastructure.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ModuleRightPane.ViewModels
{
    public class RightPaneViewModel : BindableBase
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
                    //NewCommand.RaiseCanExecuteChanged();
                    //SaveCommand.RaiseCanExecuteChanged();
                    ExecuteCommand.RaiseCanExecuteChanged();

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

        private HttpClient _httpClient;
        public HttpClient HttpClient
        {
            get { return _httpClient; }
            set
            {
                if (SetProperty(ref _httpClient, value))
                    ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        private string _endpoint;
        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                if (SetProperty(ref _endpoint, value))
                    ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        private HttpActions _httpAction;
        public HttpActions HttpAction
        {
            get { return _httpAction; }
            set { SetProperty(ref _httpAction, value); }
        }

        private string _body;
        public string Body
        {
            get { return _body; }
            set { SetProperty(ref _body, value); }
        }

        private string _response;
        public string Response
        {
            get { return _response; }
            set { SetProperty(ref _response, value); }
        }

        public DelegateCommand NewCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand ExecuteCommand { get; private set; }

        public RightPaneViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<HttpClientEvent>().Subscribe(HttpClientEventHandler);
            HttpAction = HttpActions.Get;

            ExecuteCommand = DelegateCommand.FromAsyncHandler(ExecuteAsync, CanExecute);
        }

        public void HttpClientEventHandler(HttpClient httpClient)
        {
            if (HttpClient != null)
            {
                HttpClient.Dispose();
                HttpClient = null;
            }

            HttpClient = httpClient;
        }

        private async Task ExecuteAsync()
        {
            string err = "";
            Response = "";

            try
            {
                Busy = true;

                HttpResponseMessage response = null;
                StringContent content = new StringContent(Body, System.Text.Encoding.UTF8, "application/json");

                switch (HttpAction)
                {
                    case HttpActions.Get:
                        response = await HttpClient.GetAsync(Endpoint);
                        break;
                    case HttpActions.Post:
                        response = await HttpClient.PostAsync(Endpoint, content);
                        break;
                    case HttpActions.Put:
                        response = await HttpClient.PutAsJsonAsync(Endpoint, content);
                        break;
                }

                if (!response.IsSuccessStatusCode)
                    Message = response.StatusCode.ToString();

                Response = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(await response.Content.ReadAsStringAsync()), Formatting.Indented);
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
        }

        private bool CanExecute()
        {
            return !string.IsNullOrEmpty(Endpoint) &&
                !string.IsNullOrWhiteSpace(Endpoint) &&
                !Busy &&
                HttpClient != null;
        }
    }
}