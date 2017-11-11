using System.Web;
using beer.umajkla.web.App_Start;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using AspNetHaack;

[assembly: PreApplicationStartMethod(typeof(FormsAuthenticationConfig), "Register")]
namespace beer.umajkla.web.App_Start {
    public static class FormsAuthenticationConfig {
        public static void Register() {
            DynamicModuleUtility.RegisterModule(typeof(SuppressFormsAuthenticationRedirectModule));
        }
    }
}
