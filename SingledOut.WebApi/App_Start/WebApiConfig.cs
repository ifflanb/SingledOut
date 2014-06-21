﻿using System.Web.Http;
using System.Web.Http.Dispatcher;
using SingledOut.WebApi.Services;

namespace SingledOut.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                   name: "UsersLogin",
                   routeTemplate: "api/users/Login",
                   defaults: new { controller = "users", action = "Login" }
                );

            config.Routes.MapHttpRoute(
               name: "UsersSearch",
               routeTemplate: "api/userssearch/{sp}",
               defaults: new { controller = "userssearch", sp = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "Users",
               routeTemplate: "api/users/{id}",
               defaults: new { controller = "users", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
               name: "Courses",
               routeTemplate: "api/courses/{id}",
               defaults: new { controller = "courses", id = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
              name: "Enrollments",
              routeTemplate: "api/courses/{courseId}/students/{var}",
              defaults: new { controller = "Enrollments", var = RouteParameter.Optional }
          );

           // var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
           // jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var json = config.Formatters.JsonFormatter;
           // json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Replace the controller configuration selector
            config.Services.Replace(typeof(IHttpControllerSelector), new SingledOutControllerSelector((config)));

           // config.Filters.Add(new ForceHttpsAttribute());
        }
    }
}
