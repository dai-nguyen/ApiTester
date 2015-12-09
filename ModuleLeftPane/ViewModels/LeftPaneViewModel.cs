/** 
* This file is part of the ApiTester project.
* Copyright (c) 2015 Dai Nguyen
* Author: Dai Nguyen
**/

using Infrastructure.Events;
using Infrastructure.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand SaveAsCommand { get; private set; }
        public DelegateCommand DeleteItemCommand { get; private set; }


        public LeftPaneViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<HttpModelSaveEvent>().Subscribe(HttpModelSaveEventHandler);
            Filename = Infrastructure.Properties.Resources.Filename;
            Items = new ObservableCollection<HttpModel>();

            LoadCommand = new DelegateCommand(Load);
            SaveAsCommand = new DelegateCommand(SaveAs);
            DeleteItemCommand = new DelegateCommand(DeleteItem, CanDeleteItem);
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
                    return;
                }
            }

            Items.Add(model);            
        }

        private void Load()
        {

        }

        private void SaveAs()
        {

        }

        private void DeleteItem()
        {
            Items.Remove(SelectedItem);
        }

        private bool CanDeleteItem()
        {
            return SelectedItem != null;
        }

        private bool CanSave()
        {
            return Items.Count > 0;
        }        
    }
}
