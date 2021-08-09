﻿using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace CFSWebService
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            //config.SuppressDefaultHostAuthentication();
            //config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));


            // Web API routes
            config.MapHttpAttributeRoutes();
            config.Routes.MapHttpRoute(
              name: "cfsrecordDefault",
              routeTemplate: "api/cfsrecord/{RecordedDate}",
              defaults: new { Controller = "CFSRecord" }
              );

            config.Routes.MapHttpRoute(
              name: "CFSRecordSyncDefault",
              routeTemplate: "api/CFSRecordSync",
              defaults: new { Controller = "CFSRecordSync" }
              );

            //config.Routes.MapHttpRoute(
            //     name: "DefaultApi1",
            //     routeTemplate: "api/{controller}/{action}/{id}",
            //     defaults: new { id = RouteParameter.Optional }
            // );

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

              config.Routes.MapHttpRoute(
              name: "DefaultAccount",
              routeTemplate: "{controller}"             
              );        

            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

        }
    }
}
