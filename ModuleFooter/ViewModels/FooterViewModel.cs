/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Infrastructure;
using Infrastructure.Events;
using Infrastructure.Models;
using Prism.Events;
using Prism.Mvvm;

namespace ModuleFooter.ViewModels
{
    public class FooterViewModel : BindableBase
    {
        private IEventAggregator _eventAggregator;

        private bool _isIndeterminate;
        public bool IsIndeterminate
        {
            get { return _isIndeterminate; }
            set { SetProperty(ref _isIndeterminate, value); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get { return _statusMessage; }
            set { SetProperty(ref _statusMessage, value); }
        }

        public FooterViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            _eventAggregator.GetEvent<BusyEvent>().Subscribe(BusyEventHandler, ThreadOption.UIThread);
            _eventAggregator.GetEvent<MessageEvent>().Subscribe(MessageEventHandler, ThreadOption.UIThread);
            StatusMessage = Infrastructure.Properties.Resources.Ready;
        }

        public void BusyEventHandler(BusyModel busyModel)
        {
            IsIndeterminate = busyModel.Busy;            
        }

        public void MessageEventHandler(MessageModel messageModel)
        {            
            StatusMessage = messageModel.Message;
        }
    }
}
