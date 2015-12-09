/** 
 * This file is part of the ApiTester project.
 * Copyright (c) 2015 Dai Nguyen
 * Author: Dai Nguyen
**/

using Infrastructure.Models;
using Prism.Events;

namespace Infrastructure.Events
{
    public class HttpModelLoadEvent : PubSubEvent<HttpModel>
    {
    }
}
