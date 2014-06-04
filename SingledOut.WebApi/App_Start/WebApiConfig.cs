using System.Linq;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Newtonsoft.Json.Serialization;
using SingledOut.WebApi.Filters;
using SingledOut.WebApi.Services;

namespace SingledOut.WebApi
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
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
              routeTemplate: "api/courses/{courseId}/students/{userName}",
              defaults: new { controller = "Enrollments", userName = RouteParameter.Optional }
          );

           // var jsonFormatter = config.Formatters.OfType<JsonMediaTypeFormatter>().First();
           // jsonFormatter.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var json = config.Formatters.JsonFormatter;
            json.SerializerSettings.PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects;
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            //Replace the controller configuration selector
            config.Services.Replace(typeof(IHttpControllerSelector), new SingledOutControllerSelector((config)));

           // config.Filters.Add(new ForceHttpsAttribute());
        }
    }
}
