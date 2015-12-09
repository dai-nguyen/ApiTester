/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Newtonsoft.Json;

namespace Infrastructure.Models
{
    public class TokenModel
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
