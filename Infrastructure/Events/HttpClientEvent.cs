/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Prism.Events;
using System.Net.Http;

namespace Infrastructure.Events
{
    public class HttpClientEvent : PubSubEvent<HttpClient>
    {
    }
}
