/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Infrastructure.Enums;
using Infrastructure.Events;
using Infrastructure.Models;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ModuleRightPane.ViewModels
{
    public class RightPaneViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;
        private CancellationTokenSource _tokenSource;
        
        private bool _busy;
        public bool Busy
        {
            get { return _busy; }
            set
            {
                if (SetProperty(ref _busy, value))
                {
                    NewCommand.RaiseCanExecuteChanged();
                    SaveCommand.RaiseCanExecuteChanged();
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

        private string _lebelExecute;
        public string LabelExecute
        {
            get { return _lebelExecute; }
            set { SetProperty(ref _lebelExecute, value); }
        }

        private HttpClient _httpClient = null;
        public HttpClient HttpClient
        {
            get { return _httpClient; }
            set
            {
                if (SetProperty(ref _httpClient, value))
                    ExecuteCommand.RaiseCanExecuteChanged();
            }
        }

        private Guid _id;
        public Guid Id
        {
            get { return _id; }
            set { SetProperty(ref _id, value); }
        }

        private string _endpoint;
        public string Endpoint
        {
            get { return _endpoint; }
            set
            {
                if (SetProperty(ref _endpoint, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    ExecuteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private HttpActions _httpAction;
        public HttpActions HttpAction
        {
            get { return _httpAction; }
            set
            {
                if (SetProperty(ref _httpAction, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    ExecuteCommand.RaiseCanExecuteChanged();
                }
            }
        }

        private string _body;
        public string Body
        {
            get { return _body; }
            set
            {
                if (SetProperty(ref _body, value))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                    ExecuteCommand.RaiseCanExecuteChanged();
                }
            }
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
            _eventAggregator.GetEvent<HttpModelLoadEvent>().Subscribe(HttpModelLoadEventHandler);
            HttpAction = HttpActions.Get;
            Id = Guid.NewGuid();

            LabelExecute = Infrastructure.Properties.Resources.Execute;

            NewCommand = new DelegateCommand(NewAction, CanNewAction);
            SaveCommand = new DelegateCommand(SaveAction, CanSaveAction);
            ExecuteCommand = DelegateCommand.FromAsyncHandler(ExecuteActionAsync, CanExecuteAction);
        }

        private void HttpClientEventHandler(HttpClient httpClient)
        {
            if (HttpClient != null)
            {
                HttpClient.Dispose();
                HttpClient = null;
            }

            HttpClient = httpClient;
        }

        private void HttpModelLoadEventHandler(HttpModel model)
        {
            if (_tokenSource != null && !_tokenSource.IsCancellationRequested)
                _tokenSource.Cancel();

            if (model != null)
            {
                Id = model.Id;
                Endpoint = model.Endpoint;
                HttpAction = model.HttpAction;
                Body = model.Body;
            }
            else
                NewAction();
        }

        private void NewAction()
        {            
            Id = Guid.NewGuid();
            Endpoint = "";
            HttpAction = HttpActions.Get;
            Body = "";
            Response = "";
        }

        private void SaveAction()
        {
            string err = "";
            try
            {
                if (!string.IsNullOrEmpty(Body)
                    && !string.IsNullOrWhiteSpace(Body))
                {
                    Body = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(Body), Formatting.Indented);
                }

                _eventAggregator.GetEvent<HttpModelSaveEvent>().Publish(new HttpModel
                {
                    Id = Id,
                    Endpoint = Endpoint,
                    HttpAction = HttpAction,
                    Body = Body
                });
            }
            catch (Exception ex)
            {
                err = ex.Message ?? Infrastructure.Properties.Resources.Error;
            }
            finally
            {
                if (!string.IsNullOrEmpty(err))
                    Message = err;
            }
        }

        private async Task ExecuteActionAsync()
        {            
            string err = "";
            Response = "";
            
            try
            {
                if (LabelExecute == Infrastructure.Properties.Resources.Cancel 
                    && _tokenSource != null
                    && !_tokenSource.IsCancellationRequested)
                {                    
                    _tokenSource.Cancel();                    
                    return;
                }
                
                LabelExecute = Infrastructure.Properties.Resources.Cancel;
                Busy = true;
                Message = Infrastructure.Properties.Resources.Wait;
                _tokenSource = new CancellationTokenSource();

                HttpResponseMessage response = null;
                StringContent content = new StringContent(Body ?? "", System.Text.Encoding.UTF8, "application/json");

                switch (HttpAction)
                {
                    case HttpActions.Get:
                        response = await HttpClient.GetAsync(Endpoint, _tokenSource.Token);
                        break;
                    case HttpActions.Post:
                        response = await HttpClient.PostAsync(Endpoint, content, _tokenSource.Token);
                        break;
                    case HttpActions.Put:
                        response = await HttpClient.PutAsync(Endpoint, content, _tokenSource.Token);
                        break;
                }

                if (!response.IsSuccessStatusCode)
                    Message = response.StatusCode.ToString();

                string temp = await response.Content.ReadAsStringAsync();

                if (temp.StartsWith("{") || temp.StartsWith("["))
                    Response = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(temp), Formatting.Indented);
                else
                    Response = temp;
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

                Message = string.IsNullOrEmpty(err) ? Infrastructure.Properties.Resources.Ready : err;
                LabelExecute = Infrastructure.Properties.Resources.Execute;
                Busy = false;                
            }
        }

        private bool CanNewAction()
        {
            return !Busy;
        }

        private bool CanSaveAction()
        {
            return !string.IsNullOrEmpty(Endpoint) 
                && !string.IsNullOrWhiteSpace(Endpoint) 
                && (HttpAction != HttpActions.Get ? (!string.IsNullOrEmpty(Body) && !string.IsNullOrWhiteSpace(Body)) : true);        
        }

        private bool CanExecuteAction()
        {
            return HttpClient != null
                && !string.IsNullOrEmpty(Endpoint)
                && !string.IsNullOrWhiteSpace(Endpoint)
                && (HttpAction != HttpActions.Get ? (!string.IsNullOrEmpty(Body) && !string.IsNullOrWhiteSpace(Body)) : true);            
        }
    }
}