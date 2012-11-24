using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Temp;
using WebIT.Temp.Models;
using WebIT.Lib;

namespace WebIT.Temp.Areas.Admin.Controllers
{
    [AuthorizationFilter(SecurityRole.Administrator)]
    public class WebPageController : Controller
    {
        private const int resultsPerPage = 25;

        /// <summary>
        /// Load Tab Listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? page)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<WebPage> query = db.WebPages.OrderBy(x => x.TemplateID).ThenBy(x => x.Title).Select(x => x);
            query = Pager.Setup<WebPage>(this, query.AsQueryable(), page, resultsPerPage);
            return View(query);
        }

        public ActionResult SetHome(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
            if (p != null)
            {
                //update current one
                WebPage currentHomePg = db.WebPages.SingleOrDefault(x => x.IsHomePage == true);
                if (currentHomePg != null)
                {
                    currentHomePg.IsHomePage = false;
                }

                p.IsHomePage = true;

                db.SubmitChanges();
                
            }

            return RedirectToAction("Index", "WebPage");
        }

        /// <summary>
        /// Load View (ADD/EDIT)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Manage(int id = 0)
        {
            if (id > 0)
            {
                //edit
                DBDataContext db = Utils.DB.GetContext();
                WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
                if (p != null)
                {
                    ViewData["Title"] = "Edit Web Page";
                    ViewData["Action"] = "Update";
                    return View("Manage", p);
                }
                else
                {
                    //cannot find account in database
                    return RedirectToAction("Index", "WebPages");
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Web Page";
                ViewData["Action"] = "Add";
                return View("Manage");
            }
        }


        /// <summary>
        /// Add Web Page
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(WebPage p)
        {
            if (ModelState.IsValid)
            {
                DBDataContext db = Utils.DB.GetContext();
                db.WebPages.InsertOnSubmit(p);

                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "WebPage");
                }
                catch (Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "WebPage/Add");
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                }
            }

            ViewData["Title"] = "Add Web Page";
            ViewData["Action"] = "Add";

            return View("Manage", p);
        }

        /// <summary>
        /// Update Web Page
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(int id, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
            if (p != null)
            {

                if (ModelState.IsValid)
                {
                    TryUpdateModel(p);
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "WebPage");
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Report.Exception(ex, "WebPage/Update");
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    }
                }


                ViewData["Title"] = "Edit Web Page";
                ViewData["Action"] = "Update";
                return View("Manage", p);
            }

            return RedirectToAction("Index", "WebPage");
        }

        /// <summary>
        /// Delete Web Page
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage p = db.WebPages.SingleOrDefault(x => x.ID == id);
            if (p != null)
            {
                //get all headers
                IEnumerable<Header> headers = p.Headers.Where(x => x.WebPageID == p.ID);
                foreach (Header h in headers)
                {
                    if (System.IO.File.Exists(HttpContext.Server.MapPath(h.ImagePath)))
                    {
                        System.IO.File.Delete(HttpContext.Server.MapPath(h.ImagePath));
                    }
                    if (System.IO.File.Exists(HttpContext.Server.MapPath(h.MoviePath)))
                    {
                        System.IO.File.Delete(HttpContext.Server.MapPath(h.MoviePath));
                    }
                }
                db.Headers.DeleteAllOnSubmit(headers);

                //body
                Body b = p.Bodies.SingleOrDefault(x => x.WebPageID == p.ID);
               

                //check template
                switch (p.Template.Code.ToLower())
                {
                    case "def":
                        //sidebar
                        IEnumerable<Sidebar> sidebars = p.Sidebars.Where(x => x.WebPageID == p.ID);
                        foreach(Sidebar s in sidebars)
                        {
                            if (!s.Type.Name.Equals("Link"))
                            {
                                if (System.IO.File.Exists(HttpContext.Server.MapPath(s.Source)))
                                {
                                    System.IO.File.Delete(HttpContext.Server.MapPath(s.Source));
                                }
                            }

                            if (System.IO.File.Exists(HttpContext.Server.MapPath(s.Thumb)))
                            {
                                System.IO.File.Delete(HttpContext.Server.MapPath(s.Thumb));
                            }
                        }
                        db.Sidebars.DeleteAllOnSubmit(sidebars);
                        break;                   
                    case "pdf-prev":
                        //uploaded PDF
                        if (b != null)
                        {
                            if (System.IO.File.Exists(HttpContext.Server.MapPath("/Content/data/" + b.HTML)))
                            {
                                System.IO.File.Delete(HttpContext.Server.MapPath("/Content/data/" + b.HTML));
                            }
                        }
                        break;
                    case "qa":
                        //check FAQ for Tab
                        IEnumerable<Tab> tabs = db.Tabs.Where(x => x.FAQPageID == p.ID);
                        foreach (Tab t in tabs)
                        {
                            t.FAQPageID = null;
                        }
                        break;
                    case "iframe":
                        break;
                }

                //delete body  
                if (b != null)
                {
                    db.Bodies.DeleteOnSubmit(b);
                }
                
                //navigation Links
                IEnumerable<NavigationLink> links = db.NavigationLinks.Where(x => x.WebPageID == p.ID);
                db.NavigationLinks.DeleteAllOnSubmit(links);               
          
                //delete web page
                db.WebPages.DeleteOnSubmit(p);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Web Pages/Delete");
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Web Page does not exist in the database");
            }

            return RedirectToAction("Index", "WebPage");
        }

    }
}
