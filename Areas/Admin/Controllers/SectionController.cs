using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Temp.Models;
using WebIT.Temp;
using WebIT.Lib;

namespace WebIT.Temp.Areas.Admin.Controllers
{
    [AuthorizationFilter(SecurityRole.Administrator)]
    public class SectionController : Controller
    {
         private const int resultsPerPage = 25;

        /// <summary>
        /// Load Section Listing
        /// </summary>
        /// <param name="tab">Tab ID</param>
        /// <param name="page"></param>
        /// <returns></returns>
         public ActionResult Index(int tab, int? page)
         {
             ViewData["TabID"] = tab.ToString();

             DBDataContext db = Utils.DB.GetContext();
             IEnumerable<Section> query = db.Sections.Where(x => x.TabID == tab).OrderBy(x => x.Position).Select(x => x);
             query = Pager.Setup<Section>(this, query.AsQueryable(), page, resultsPerPage);
             return View(query);
         }

        /// <summary>
        /// Load View (ADD/EDIT)
        /// </summary>
        /// <param name="tab">Tab ID</param>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Manage(string tab, int id = 0)
        {
            ViewData["TabID"] = tab;

            if (id > 0)
            {
                //edit
                DBDataContext db = Utils.DB.GetContext();
                Section s = db.Sections.SingleOrDefault(x => x.ID == id);
                if (s != null)
                {
                    ViewData["Title"] = "Edit Section";
                    ViewData["Action"] = "Update";
                    s.TabID = Convert.ToInt32(tab);
                    return View("Manage", s);
                }
                else
                {
                    //cannot find model in database
                    return RedirectToAction("Index", "Section", new { controller = "Section", tab = tab });
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Section";
                ViewData["Action"] = "Add";                
                return View("Manage");
            }
        }
        
        /// <summary>
        /// Add Section
        /// </summary>
        /// <param name="s"></param>
        /// <param name="tabId"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Section s, string tabId)
        {
            if (ModelState.IsValid)
            {
                if (s.Title.ToLower().Equals("faq"))
                {
                    ModelState.AddModelError("", "faq is protected name for FAQ pages under Tabs and cannot be used. Please try using different name.");
                }
                else
                {
                    DBDataContext db = Utils.DB.GetContext();

                    s.Position = db.Sections.Where(x => x.TabID == Convert.ToInt32(tabId)).Count() + 1;
                    Tab t = db.Tabs.SingleOrDefault(x => x.ID == Convert.ToInt32(tabId));
                    if (t != null)
                    {
                        t.Sections.Add(s);

                        try
                        {
                            db.SubmitChanges();
                            return RedirectToAction("Index", "Section", new { controller = "Section", tab = tabId });
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Report.Exception(ex, "Section/Add");
                            ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Tab does not exist in the database");
                    }
                }
            }

            ViewData["Title"] = "Add Section";
            ViewData["Action"] = "Add";
            ViewData["TabID"] = tabId;
            return View("Manage");

        }

        /// <summary>
        /// Update Section
        /// </summary>
        /// <param name="id">SectionID</param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(int id, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            Section s = db.Sections.SingleOrDefault(x=>x.ID == id);
            if(s != null)
            {
                if (ModelState.IsValid)
                {
                    if (form["Title"].ToString().ToLower().Equals("faq"))
                    {
                        ModelState.AddModelError("", "faq is protected name for FAQ pages under Tabs and cannot be used. Please try using different name.");
                    }
                    else
                    {

                        TryUpdateModel(s);

                        try
                        {
                            db.SubmitChanges();
                            return RedirectToAction("Index", "Section", new { controller = "Section", tab = s.TabID.ToString() });
                        }
                        catch (Exception ex)
                        {
                            ErrorHandler.Report.Exception(ex, "Section/Update");
                            ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        }
                    }
                }

                ViewData["Title"] = "Edit Section";
                ViewData["Action"] = "Update";
                ViewData["TabID"] = s.TabID.ToString();
                return View("Manage", s);
            }

            return RedirectToAction("Index", "Section", new { controller = "Section", tab = s.TabID.ToString() });
        }

        /// <summary>
        /// Delete Section
        /// </summary>
        /// <param name="id">SectionID</param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Section s = db.Sections.SingleOrDefault(x => x.ID == id);
            if (s != null)
            {

                //delete all navigation links for this section
                db.NavigationLinks.DeleteAllOnSubmit(s.NavigationLinks);


                //delete section
                db.Sections.DeleteOnSubmit(s);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Section/Delete");
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Section does not exist in the database");
            }

            return RedirectToAction("Index", "Section", new { controller = "Section", tab = s.TabID.ToString() });
        }

        /////////////////
        // POSITIONING //
        /////////////////

        /// <summary>
        /// update section order
        /// </summary>
        /// <param name="tabId"></param>
        /// <param name="order"></param>
        /// <param name="delim"></param>
        /// <returns>JSON object { Success : true }</returns>
        public ActionResult SaveSectionOrder(int tabId, string order, string delim)
        {
            string[] arr = Utils.Array.FromString(order, delim);
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Section> data = db.Sections.Where(x => x.TabID == tabId).OrderBy(x => x.Position).Select(x => x);
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
                    ErrorHandler.Report.Exception(ex, "Section/SaveSectionOrder TabID: " + tabId.ToString() + " ID: " + id);
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
                ErrorHandler.Report.Exception(exc, "Section/SaveSectionOrder");
                ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                bSuccess = false;
                error = exc.Message;
            }

            return Json(new { Success = bSuccess, Error = error }, JsonRequestBehavior.AllowGet);
        }


    }
}
