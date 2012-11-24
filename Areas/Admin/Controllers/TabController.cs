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
    public class TabController : Controller
    {
        private const int resultsPerPage = 25;

        //////////
        // TABS //
        //////////

        /// <summary>
        /// Load Tab Listing
        /// </summary>
        /// <returns></returns>
        public ActionResult Index(int? page)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Tab> query = db.Tabs.OrderBy(x => x.Position).Select(x => x);
            query = Pager.Setup<Tab>(this, query.AsQueryable(), page, resultsPerPage);
            return View(query);
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
                Tab t = db.Tabs.SingleOrDefault(x => x.ID == id);
                if (t != null)
                {
                    ViewData["Title"] = "Edit Tab";
                    ViewData["Action"] = "Update";
                    return View("Manage", t);
                }
                else
                {
                    //cannot find account in database
                    return RedirectToAction("Index", "Tab");
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Tab";
                ViewData["Action"] = "Add";
                return View("Manage");
            }
        }

        /// <summary>
        /// Add Tab
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Add(Tab t)
        {
            if (ModelState.IsValid)
            {
                DBDataContext db = Utils.DB.GetContext();
                db.Tabs.InsertOnSubmit(t);
                t.Position = db.Tabs.Count() + 1;
                t.FAQPageID = null;

                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "Tab");
                }
                catch(Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Tab/Add");
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                }
            }

            ViewData["Title"] = "Add Tab";
            ViewData["Action"] = "Add";

            return View("Manage", t);
        }

        /// <summary>
        /// Update Tab
        /// </summary>
        /// <param name="id"></param>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Update(int id, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            Tab t = db.Tabs.SingleOrDefault(x => x.ID == id);
            if (t != null)
            {

                if (ModelState.IsValid)
                {
                    TryUpdateModel(t);
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Tab");
                    }
                    catch(Exception ex)
                    {
                        ErrorHandler.Report.Exception(ex, "Tab/Update");
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    }
                }


                ViewData["Title"] = "Edit Tab";
                ViewData["Action"] = "Update";
                return View("Manage", t);
            }

            return RedirectToAction("Index", "Tab");
        }

        /// <summary>
        /// Delete Tab
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Tab t = db.Tabs.SingleOrDefault(x => x.ID == id);
            if (t != null)
            {
                //get all sections
                IEnumerable<Section> sections = t.Sections.Where(x => x.TabID == t.ID);

                foreach (var s in sections)
                {
                    //delete all navigation links for each section
                    db.NavigationLinks.DeleteAllOnSubmit(s.NavigationLinks);
                }

                //delete all sections
                db.Sections.DeleteAllOnSubmit(sections);
                //delete tab
                db.Tabs.DeleteOnSubmit(t);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Tab/Delete");
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Tab does not exist in the database");
            }

            return RedirectToAction("Index", "Tab");
        }

        /////////////////
        // POSITIONING //
        /////////////////

        /// <summary>
        /// update tab order
        /// </summary>
        /// <param name="order"></param>
        /// <param name="delim"></param>
        /// <returns>JSON object { Success : true }</returns>
        public ActionResult SaveTabOrder(string order, string delim)
        {
            string[] arr = Utils.Array.FromString(order, delim);
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Tab> data = db.Tabs.OrderBy(x => x.Position).Select(x => x);
            int ctr = 1;
            foreach (string id in arr)
            {
                try
                {
                    data.Single(x => x.ID == Convert.ToInt32(id)).Position = ctr;
                    ctr++;
                }
                catch(Exception ex)
                {
                    ErrorHandler.Report.Exception(ex, "Tab/SaveTabOrder ID: " + id);
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
                ErrorHandler.Report.Exception(exc, "Tab/SaveTabOrder");
                ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                bSuccess = false;
                error = exc.Message;
            }

            return Json(new { Success = bSuccess, Error = error}, JsonRequestBehavior.AllowGet);
        }

        //////////////
        // FAQ PAGE //
        /////////////

        /// <summary>
        /// Load FAQ Page View
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult FAQPage(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Tab t = db.Tabs.SingleOrDefault(x => x.ID == id);
            if (t != null)
            {
                ViewData["TabID"] = id.ToString();
                if (t.FAQPageID != null)
                {
                    ViewData["FAQPageID"] = t.FAQPageID.ToString();
                }
                return View(t);
            }

            ModelState.AddModelError("", "Tab does not exist in the database");
            return RedirectToAction("Index", "Tab");
        }

        [HttpPost]
        public ActionResult FAQPage(int id, int faqId)
        {
            DBDataContext db = Utils.DB.GetContext();
            Tab t = db.Tabs.SingleOrDefault(x => x.ID == id);
            if (t != null)
            {
                TryUpdateModel(t);
                t.FAQPageID = faqId;

                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "Tab", new { controller = "Tab", action = "Index", tab = id.ToString() });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    ErrorHandler.Report.Exception(ex, "Tab/FAQPage[POST]");
                }
            }

            return View(t);

        }
    }
}