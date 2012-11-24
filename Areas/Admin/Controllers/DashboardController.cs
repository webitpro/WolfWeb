using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebIT.Temp.Areas.Admin.Controllers
{
    [AuthorizationFilter(SecurityRole.Administrator)]
    public class DashboardController : Controller
    {
        //
        // GET: /Admin/Dashboard/

        public ActionResult Index()
        {
            return View();
        }

    }
}
