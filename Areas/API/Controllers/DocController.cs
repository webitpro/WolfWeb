using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Lib;
using WebIT.Temp.Models;

namespace WebIT.Temp.Areas.API.Controllers
{
    public class DocController : Controller
    {
        //
        // GET: /API/Doc/

        public ActionResult Download(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Document doc = db.Documents.SingleOrDefault(x => x.ID == id);
            if (doc != null)
            {
                return File(doc.Source, "application/octet-stream", doc.Title + ".pdf");
            }

            return null;

        }

    }
}
