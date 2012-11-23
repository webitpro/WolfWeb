using System.Web.Mvc;
using WebIT.Temp;

namespace WebIT.Temp.Areas.API
{
    public class APIAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "API";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "API_default",
                "API" + Config.ActiveConfiguration.ControllerExtension + "/{controller}/{action}/{id}",
                new { controller = "User", action = "Login", id = UrlParameter.Optional }
            );
           
        }
    }
}
