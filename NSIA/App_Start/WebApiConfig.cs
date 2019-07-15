using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.ExceptionHandling;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json.Serialization;
using NSIA.CustomHandler;
using NSIA.ExLogger;

namespace NSIA
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            // Configure Web API to use only bearer token authentication.
            //Register Exception Handler  
            config.Services.Add(typeof(IExceptionLogger), new ExceptionManagerApi());
            //Registering GlobalExceptionHandler
            config.Services.Replace(typeof(IExceptionHandler), new GlobalExceptionHandler());

            var cors = new EnableCorsAttribute(" http://localhost:4200, https://sandbox.interswitchng.com", "*", "*");
            config.EnableCors(cors);

            config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);
            // Web API routes
            config.MapHttpAttributeRoutes();
            config.MessageHandlers.Add(new TokenValidationHandler());
            
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
