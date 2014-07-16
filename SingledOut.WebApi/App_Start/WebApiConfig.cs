using System.Web.Http;
using System.Web.Http.Dispatcher;
using SingledOut.WebApi.Services;

namespace SingledOut.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "AccountRegister",
                routeTemplate: "api/account/Register/{userModel}",
                defaults: new { controller = "account", action = "Register", userLocationModel = RouteParameter.Optional }
            );

            config.Routes.MapHttpRoute(
               name: "AccountRetrievePassword",
               routeTemplate: "api/account/retrievepassword/{email}",
               defaults: new { controller = "account", action = "RetrievePassword" }
            );

            config.Routes.MapHttpRoute(
                name: "AccountLogin",
                routeTemplate: "api/account/Login",
                defaults: new { controller = "account", action = "Login" }
            );

            config.Routes.MapHttpRoute(
               name: "UsersSearch",
               routeTemplate: "api/userssearch/{sp}",
               defaults: new { controller = "userssearch", sp = RouteParameter.Optional }
           );

            config.Routes.MapHttpRoute(
                name: "Users",
                routeTemplate: "api/users/{id}",
                defaults: new {controller = "users", id = RouteParameter.Optional}
            );

            config.Routes.MapHttpRoute(
                name: "UserLocations",
                routeTemplate: "api/userlocations/{userLocationModel}",
                defaults: new { controller = "UserLocations", userLocationModel = RouteParameter.Optional });

            config.Routes.MapHttpRoute(
                name: "DeleteUserLocations",
                routeTemplate: "api/userlocations/DeleteUserLocation/{id}",
                defaults: new { controller = "UserLocations", action = "DeleteUserLocation" });

            
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
