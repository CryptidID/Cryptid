﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace CryptidRest
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services

            // Web API routes
            config.MapHttpAttributeRoutes();
        }
    }
}
