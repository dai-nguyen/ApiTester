/** 
* This file is part of the ApiTester project.
* Copyright (c) 2015 Dai Nguyen
* Author: Dai Nguyen
**/

namespace Infrastructure.Models
{
    public class LoginModel
    {
        public string BaseAddress { get; set; }
        public string TokenEndpoint { get; set; }
        public string UserID { get; set; }        
    }
}
