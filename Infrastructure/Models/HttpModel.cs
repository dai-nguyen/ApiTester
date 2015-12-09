/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Infrastructure.Enums;
using Prism.Mvvm;
using System;

namespace Infrastructure.Models
{
    public class HttpModel : BindableBase
    {
        public Guid Id { get; set; }

        private string _endpoint;
        public string Endpoint
        {
            get { return _endpoint; }
            set { SetProperty(ref _endpoint, value); }
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
    }
}