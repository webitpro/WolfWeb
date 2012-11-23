using System.Web.Mvc;

namespace WebIT.Temp.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            //LOGIN
            context.MapRoute(
                "Admin_Login",
                "Admin" + Config.ActiveConfiguration.ControllerExtension + "/Login",
                new { controller = "Login", action = "Login" }
            );

            //LOGOUT
            context.MapRoute(
                "Admin_Logout",
                "Admin" + Config.ActiveConfiguration.ControllerExtension + "/Logout",
                new { controller = "Login", action = "Logout" }
            );

            //NAVIGATION LINKS MANAGEMENT
           /* context.MapRoute(
                "LinksDefaultRoute",
                "Admin" + Config.ActiveConfiguration.ControllerExtension + "/{controller}/{action}/{tab}/{section}/{id}",
                new { controller = "Link", action = "Index", tab = 0, section = 0, id = UrlParameter.Optional }
            );

            //NAVIGATION SECTION MANAGEMENT
            context.MapRoute(
                "SectionDefaultRoute",
                "Admin" + Config.ActiveConfiguration.ControllerExtension + "/{controller}/{action}/{tab}/{id}",
                new { controller = "Section", action = "Index", tab = 0, id = UrlParameter.Optional }
            );  */          


            //ADMIN DEFAULT
            context.MapRoute(
                "AdminDefaultRoute",
                "Admin" + Config.ActiveConfiguration.ControllerExtension + "/{controller}/{action}/{id}",
                new { controller = "Dashboard", action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
