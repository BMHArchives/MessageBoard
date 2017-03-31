using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;

namespace MessageBoard
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
            jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            config.Formatters.JsonFormatter.
            SupportedMediaTypes.Add(new System.Net.Http.Headers.MediaTypeHeaderValue("text/html"));
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
             name: "RepliesRoute",
             routeTemplate: "api/v1/topics/{topicid}/replies/{id}",
             defaults: new { controller = "replies", id = RouteParameter.Optional }
                );
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/v1/topics/{id}",
                defaults: new { controller = "topics", id = RouteParameter.Optional }
            );
            
        }
    }
}
