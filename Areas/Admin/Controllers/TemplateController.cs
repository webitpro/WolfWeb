using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Temp;

namespace WebIT.Temp.Areas.Admin.Controllers
{
    [AuthorizationFilter(SecurityRole.Administrator)]
    public class TemplateController : Controller
    {
        //
        // GET: /Admin/Template/

        public ActionResult WebPages(string id)
        {
            return Json(Form.DropDownElement.WebPages(Convert.ToInt32(id), null), JsonRequestBehavior.AllowGet);
        }

    }
}
