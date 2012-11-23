using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Temp.Models;
using WebIT.Lib;

namespace WebIT.Temp.Areas.Admin.Controllers
{
    [AuthorizationFilter(SecurityRole.Administrator)]
    public class LinkController : Controller
    {
        private const int resultsPerPage = 25;

        /// <summary>
        /// Load Link Listing
        /// </summary>
        /// <param name="section">Section ID</param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult Index(int section, int? page)
        {

            DBDataContext db = Utils.DB.GetContext();
            Section s = db.Sections.SingleOrDefault(x => x.ID == section);
            if (s != null)
            {
                ViewData["SectionID"] = section.ToString();
                ViewData["TabID"] = s.TabID.ToString();
            }
            IEnumerable<NavigationLink> query = db.NavigationLinks.Where(x => x.SectionID == section).OrderBy(x => x.Position).Select(x => x);
            query = Pager.Setup<NavigationLink>(this, query.AsQueryable(), page, resultsPerPage);
            return View(query);
        }

        /// <summary>
        /// Load View (ADD/EDIT)
        /// </summary>
        /// <param name="tab">Tab ID</param>
        /// <param name="section">Section ID</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Manage(string tab, string section,  int id = 0)
        {
            ViewData["TabID"] = tab;
            ViewData["SectionID"] = section;

            if (id > 0)
            {
                //edit
                DBDataContext db = Utils.DB.GetContext();
                NavigationLink l = db.NavigationLinks.SingleOrDefault(x => x.ID == id);
                if (l != null)
                {
                    ViewData["Title"] = "Edit Link";
                    ViewData["Action"] = "Update";
                    l.SectionID = Convert.ToInt32(section);
                    ViewData["TemplateID"] = l.WebPage.TemplateID.ToString();
                    return View("Manage", l);
                }
                else
                {
                    //cannot find model in database
                    return RedirectToAction("Index", "Link", new{ controller = "Link", tab = tab, section = section});
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Link";
                ViewData["Action"] = "Add";
                ViewData["TemplateID"] = null;
                return View("Manage");
            }
        }

        /// <summary>
        /// Add Link
        /// </summary>
        /// <param name="l"></param>
        /// <param name="webPageId"></param>
        /// <param name="tabId"></param>
        /// <param name="sectionId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(NavigationLink l, string webPageId, string tabId, string sectionId)
        {
            if (ModelState.IsValid)
            {
                DBDataContext db = Utils.DB.GetContext();

                l.Position = db.NavigationLinks.Where(x=>x.SectionID == Convert.ToInt32(sectionId)).Count() + 1;
                Section s = db.Sections.SingleOrDefault(x => x.ID == Convert.ToInt32(sectionId));
                if (s != null)
                {
                    s.NavigationLinks.Add(l);
                    l.WebPageID = Convert.ToInt32(webPageId);
                    

                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Link", new { controller = "Link", tab = tabId, section = sectionId });
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Report.Exception(ex, "Links/Add");
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Section does not exist in the database");
                }
            }

            ViewData["Title"] = "Add Link";
            ViewData["Action"] = "Add";
            ViewData["TabID"] = tabId;
            ViewData["SectionID"] = sectionId;
            return View("Manage");
        }

        /// <summary>
        /// Update Link
        /// </summary>
        /// <param name="id">SectionID</param>
        /// <param name="webPageId"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(int id, string webPageId, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            NavigationLink l = db.NavigationLinks.SingleOrDefault(x => x.ID == id);
            if (l != null)
            {

                if (ModelState.IsValid)
                {
                    TryUpdateModel(l);
                    l.WebPageID = Convert.ToInt32(webPageId);

                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Link", new { controller = "Link", tab = l.Section.TabID.ToString(), section = l.SectionID.ToString() });
                    }
                    catch (Exception ex)
                    {
                        ErrorHandler.Report.Exception(ex, "Link/Update");
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    }
                }

                ViewData["Title"] = "Edit Link";
                ViewData["Action"] = "Update";
                ViewData["TabID"] = l.Section.TabID.ToString();
                ViewData["SectionID"] = l.SectionID.ToString();
                return View("Manage", l);
            }

            return RedirectToAction("Index", "Link", new { controller = "Link", tab = l.Section.TabID.ToString(), section = l.SectionID.ToString() });
        }

        /// <summary>
        /// Delete Link
        /// </summary>
        /// <param name="id">Link ID</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            NavigationLink l = db.NavigationLinks.SingleOrDefault(x => x.ID == id);
            if (l != null)
            {                
                //delete link
                db.NavigationLinks.DeleteOnSubmit(l);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Link/Delete");
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Section does not exist in the database");
            }

            return RedirectToAction("Index", "Link", new { controller = "Link", tab = l.Section.TabID.ToString(), section = l.SectionID.ToString() });
        }

        /////////////////
        // POSITIONING //
        /////////////////

        /// <summary>
        /// update section order
        /// </summary>
        /// <param name="sectionId"></param>
        /// <param name="order"></param>
        /// <param name="delim"></param>
        /// <returns>JSON object { Success : true }</returns>
        public ActionResult SaveLinkOrder(int sectionId, string order, string delim)
        {
            string[] arr = Utils.Array.FromString(order, delim);
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<NavigationLink> data = db.NavigationLinks.Where(x => x.SectionID == sectionId).OrderBy(x => x.Position).Select(x => x);
            int ctr = 1;
            foreach (string id in arr)
            {
                try
                {
                    data.Single(x => x.ID == Convert.ToInt32(id)).Position = ctr;
                    ctr++;
                }
                catch (Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Section/SaveLinkOrder SectionID: " + sectionId.ToString() + " ID: " + id);
                }
            }
            bool bSuccess = true;
            string error = "";
            try
            {
                db.SubmitChanges();
            }
            catch (Exception exc)
            {
                ErrorHandler.Report.Exception(exc, "Section/SaveLinkOrder");
                ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                bSuccess = false;
                error = exc.Message;
            }

            return Json(new { Success = bSuccess, Error = error }, JsonRequestBehavior.AllowGet);
        }
    }
}
