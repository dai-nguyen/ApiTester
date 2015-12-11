/** 
* This file is part of the ApiTester project.
* Copyright (c) 2015 Dai Nguyen
* Author: Dai Nguyen
**/

using Infrastructure.Events;
using Infrastructure.Models;
using Microsoft.Win32;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

namespace ModuleLeftPane.ViewModels
{
    public class LeftPaneViewModel : BindableBase
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

        private string _filename;
        public string Filename
        {
            get { return _filename; }
            set { SetProperty(ref _filename, value); }
        }

        private LoginModel _loginModel;
        public LoginModel LoginModel
        {
            get { return _loginModel; }
            set
            {
                if (SetProperty(ref _loginModel, value))
                    SaveAsCommand.RaiseCanExecuteChanged();
            }
        }

        private HttpModel _selectedItem;
        public HttpModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (SetProperty(ref _selectedItem, value))
                {
                    _eventAggregator.GetEvent<HttpModelLoadEvent>().Publish(value);
                }
            }
        }

        public ObservableCollection<HttpModel> Items { get; private set; }        
        public InteractionRequest<IConfirmation> ConfirmationRequest { get; private set; }

        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand SaveAsCommand { get; private set; }
        public DelegateCommand DeleteItemCommand { get; private set; }

        public LeftPaneViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<HttpModelSaveEvent>().Subscribe(HttpModelSaveEventHandler);
            _eventAggregator.GetEvent<LoginEvent>().Subscribe(LoginEventHandler);
            Filename = Infrastructure.Properties.Resources.Filename;
            Items = new ObservableCollection<HttpModel>();
            ConfirmationRequest = new InteractionRequest<IConfirmation>();

            LoadCommand = new DelegateCommand(LoadAction);
            SaveAsCommand = new DelegateCommand(SaveAsAction);
            DeleteItemCommand = new DelegateCommand(DeleteItemAction, CanDeleteItemAction);
        }

        private void HttpModelSaveEventHandler(HttpModel model)
        {            
            foreach (var item in Items)
            {
                if (item.Id == model.Id)
                {                    
                    item.Endpoint = model.Endpoint;
                    item.HttpAction = model.HttpAction;
                    item.Body = model.Body;
                    SelectedItem = item;
                    return;
                }
            }

            Items.Add(model);
            SelectedItem = model;
            SaveAsCommand.RaiseCanExecuteChanged();
        }

        private void LoginEventHandler(LoginModel model)
        {
            LoginModel = model;
        }

        private void LoadAction()
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json documents (.json)|*.json";
            dlg.Multiselect = false;
            dlg.CheckFileExists = true;
            dlg.CheckPathExists = true;

            bool? result = dlg.ShowDialog();

            if (result != true)
            {
                Message = "Invalid file.";
                return;
            }

            Items.Clear();
            string err = "";

            try
            {
                string filename = dlg.FileName;
                int lastIndex = filename.LastIndexOf('\\') + 1;
                Filename = filename.Substring(lastIndex);
                var profile = JsonConvert.DeserializeObject<ProfileModel>(File.ReadAllText(filename));

                if (profile != null)
                {
                    foreach (var item in profile.Items)
                    {
                        Items.Add(item);
                    }
                }

                _eventAggregator.GetEvent<LoadLoginModelEvent>().Publish(profile != null ? profile.LoginModel : null);
            }
            catch (Exception ex)
            {
                err = ex.Message ?? Infrastructure.Properties.Resources.Error;
            }
            finally
            {
                Message = string.IsNullOrEmpty(err) ? Infrastructure.Properties.Resources.Ready : err;
            }
        }

        private void SaveAsAction()
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.DefaultExt = ".json";
            dlg.Filter = "Json documents (.json)|*.json";
            
            bool? result = dlg.ShowDialog();

            if (result != true)                
                return;
            
            string err = "";

            try
            {
                string filename = dlg.FileName;
                int lastIndex = filename.LastIndexOf('\\') + 1;
                Filename = filename.Substring(lastIndex);

                var profile = new ProfileModel
                {
                    LoginModel = LoginModel,
                    Items = Items
                };

                File.WriteAllText(filename, JsonConvert.SerializeObject(profile, Formatting.Indented));                
            }
            catch (Exception ex)
            {
                err = ex.Message ?? Infrastructure.Properties.Resources.Error;
            }
            finally
            {
                Message = string.IsNullOrEmpty(err) ? Infrastructure.Properties.Resources.Ready : err;
            }
        }

        private void DeleteItemAction()
        {
            ConfirmationRequest.Raise(new Confirmation
            {
                Content = Infrastructure.Properties.Resources.AreYouSure,
                Title = Infrastructure.Properties.Resources.Confirmation
            }, c =>
            {
                if (c.Confirmed)
                {
                    Items.Remove(SelectedItem);
                    SaveAsCommand.RaiseCanExecuteChanged();
                }
            });            
        }

        private bool CanDeleteItemAction()
        {
            return SelectedItem != null;
        }

        private bool CanSaveAction()
        {
            return Items.Count > 0 && LoginModel != null;
        }           
    }
}
