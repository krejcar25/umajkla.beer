using System.Net.Http.Headers;
using System.Web.Http;

class WebApiConfig
{
    public static void Register(HttpConfiguration configuration)
    {
        configuration.Routes.MapHttpRoute("API Default", "api/{controller}/{id}/{extra}",
            new { id = RouteParameter.Optional, extra = RouteParameter.Optional });
        /*configuration.Formatters.JsonFormatter.SupportedMediaTypes
            .Add(new MediaTypeHeaderValue("text/html"));*/
    }
}