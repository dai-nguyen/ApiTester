/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using System;
using System.Collections.Generic;

namespace Infrastructure.Models
{
    public class ProfileModel
    {
        public LoginModel LoginModel { get; set; }
        public IList<HttpModel> Items { get; set; }
    }
}
