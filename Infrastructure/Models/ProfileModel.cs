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
        public string Name { get; set; }
        public DateTime DateModified { get; set; }
        public string ModifiedBy { get; set; }
        public IList<HttpModel> Endpoints { get; set; }
    }
}
