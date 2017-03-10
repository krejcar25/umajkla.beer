using System.Net.Http.Headers;
using System.Web.Http;

class WebApiConfig
{
    public static void Register(HttpConfiguration configuration)
    {
        configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{task}/{id}",
            new { task = RouteParameter.Optional, id = RouteParameter.Optional });
        /*configuration.Formatters.JsonFormatter.SupportedMediaTypes
            .Add(new MediaTypeHeaderValue("text/html"));*/
    }
}