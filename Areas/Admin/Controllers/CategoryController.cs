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
    public class CategoryController : Controller
    {
        private const int resultsPerPage = 25;

        public ActionResult Index(int? page)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Models.Category> query = db.Categories.Select(x => x);
            query = Pager.Setup<Category>(this, query.AsQueryable(), page, resultsPerPage);
             try
             { 
                 ViewData["Error"] = ((!string.IsNullOrEmpty(TempData["Error"].ToString())) ? TempData["Error"].ToString() : ""); 
             } 
             catch
             {
             }
            return View(query);
        }



        [HttpPost]
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Document> query = db.Documents.Where(x => x.CategoryID == id);
            if (query.Count() > 0)
            {
                ModelState.AddModelError("", "Unable to delete because this Category is related to at least " + query.Count().ToString() + " documents under Document Library section.");
                TempData["Error"] = "Unable to delete because this Category is related to at least " + query.Count().ToString() + " documents under Document Library section.";
            }

            if (ModelState.IsValid)
            {
                Category c = db.Categories.SingleOrDefault(x => x.ID == id);
                if (c != null)
                {
                    db.Categories.DeleteOnSubmit(c);
                    try
                    {
                        db.SubmitChanges();
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in a few minutes.");
                        TempData["Error"] = "An unknow error occurred. Please try again in few minutes.";
                        ErrorHandler.Report.Exception(ex, "Category/Delete");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Unable to find Category");
                }
            }

            return RedirectToAction("Index", "Category");
        }

        public ActionResult Manage(int id = 0)
        {
            if (id > 0)
            {
                //edit
                DBDataContext db = Utils.DB.GetContext();
                Category c = db.Categories.SingleOrDefault(x => x.ID == id);
                if (c != null)
                {
                    ViewData["Title"] = "Edit Category";
                    ViewData["Action"] = "Update";
                    return View(c);
                }
                else
                {
                    //cannot find category in database
                    return RedirectToAction("Index", "Category");
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Category";
                ViewData["Action"] = "Add";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Add(Category c, string webpage, string tmpl)
        {
            DBDataContext db = Utils.DB.GetContext();
            if (ModelState.IsValid)
            {
                db.Categories.InsertOnSubmit(c);
                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "Category", new { controller = "Category", action = "Index", webpage = webpage, tmpl = tmpl});
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    ErrorHandler.Report.Exception(ex, "Category/Add[HTTPPOST]");
                }
            }
            ViewData["Title"] = "Add Category";
            ViewData["Action"] = "Add";
            return View("Manage", c);
        }

        [HttpPost]
        public ActionResult Update(int id, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            Category c = db.Categories.SingleOrDefault(x => x.ID == id);
            if (c != null)
            {
                TryUpdateModel(c);

                if (ModelState.IsValid)
                {
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Category");
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        ErrorHandler.Report.Exception(ex, "Category/Update[HTTPPOST]");
                    }
                }
            }

            ViewData["Title"] = "Edit Category";
            ViewData["Action"] = "Update";
            return View("Manage", c);
        }

    }
}
