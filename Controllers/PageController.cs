using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Temp;
using System.Drawing;
using System.IO;


namespace WebIT.Temp.Controllers
{
    public class PageController : Controller
    {
        //
        // GET: /Page/

        public ActionResult Index(string tab = "", string section = "", string page = "", int id = 0)
        {
            return View();
        }

    }
}
