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
    public class BodyController : Controller
    {
        private const int resultsPerPage = 25;
        private const int empResultsPerPage = 5;

        public ActionResult Index(int webpage, string tmpl)
        {
            ViewData["WebPageID"] = webpage.ToString();
            ViewData["Template"] = tmpl;
            DBDataContext db = Utils.DB.GetContext();
            Body b = db.Bodies.SingleOrDefault(x => x.WebPageID == webpage);
            if (b != null)
            {
                return View(b);
            }
            return View(new Body());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Index(string webPageId, string rte, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webPageId));
            if (pg != null)
            {
                Body b = pg.Bodies.FirstOrDefault();
                if (b != null)
                {
                    //update
                    b.HTML = HttpUtility.HtmlEncode(rte);
                }
                else
                {
                    pg.Bodies.Add(new Body() { HTML = HttpUtility.HtmlEncode(rte) });
                }

                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "WebPage", new { controller = "WebPage", action = "Index", webpage = webPageId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ErrorHandler.Report.Exception(ex, "Body/Index [POST]");
                }
            }
            else
            {
                ModelState.AddModelError("", "Web Page does not exist");
            }

            return View();
        }

        /* IFRAME */
        public ActionResult IFrame(string webpage, string tmpl)
        {
            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            DBDataContext db = Utils.DB.GetContext();
            Body b = db.Bodies.SingleOrDefault(x => x.WebPageID == Convert.ToInt32(webpage));
            if (b != null)
            {
                return View(b);
            }
            return View(new Body());
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult IFrame(string webPageId, string code, FormCollection form)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webPageId));
            if (pg != null)
            {
                Body b = pg.Bodies.FirstOrDefault();
                if (b != null)
                {
                    //update
                    b.HTML = HttpUtility.HtmlEncode(code);
                }
                else
                {
                    //insert
                    pg.Bodies.Add(new Body() { HTML = HttpUtility.HtmlEncode(code) });
                }
                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "WebPage", new { controller = "WebPage", action = "Index", webpage = webPageId });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ErrorHandler.Report.Exception(ex, "Body/IFrame [POST]");
                }
            }
            else
            {
                ModelState.AddModelError("", "Web Page does not exist");
            }
            return View();
        }

        /* PDF PREVIEW */
        public virtual ActionResult PDFPrev(string webpage, string tmpl)
        {
            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            DBDataContext db = Utils.DB.GetContext();
            Body b = db.Bodies.SingleOrDefault(x => x.WebPageID == Convert.ToInt32(webpage));
            if (b != null)
            {
                return View(b);
            }
            return View(new Body());
        }

        [HttpPost]
        public ActionResult PDFPrev(string webPageId, DocumentFile doc)
        {
            string code = "";

            if (doc != null)
            {
                doc.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    if (doc.Save(doc.Name.Replace(" ", "").ToLower()))
                    {
                        code = doc.SavedName;
                        doc.Cleanup();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Document File format is not recognized");
            }

            if (!string.IsNullOrEmpty(code))
            {

                DBDataContext db = Utils.DB.GetContext();
                WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webPageId));
                if (pg != null)
                {
                    Body b = pg.Bodies.FirstOrDefault();
                    if (b != null)
                    {
                        //delete existing PDF file
                        if (System.IO.File.Exists(HttpContext.Server.MapPath(Url.Data(b.HTML))))
                        {
                            System.IO.File.Delete(HttpContext.Server.MapPath(Url.Data(b.HTML)));
                        }
                        b.HTML = HttpUtility.HtmlEncode(code);
                    }
                    else
                    {
                        pg.Bodies.Add(new Body() { HTML = HttpUtility.HtmlEncode(code) });
                    }

                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "WebPage", new { controller = "WebPage", action = "Index", webpage = webPageId });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        ErrorHandler.Report.Exception(ex, "Body/PDFPrev [POST]");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Web Page does not exist");
                }

            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your Document file. Please try again in few minutes.");
            }

            return View();
        }


        /* DOCUMENT LIBRARY */
        public ActionResult DocLib(string webpage, string tmpl, int? page)
        {
            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Document> docs = db.Documents.Where(x => x.WebPageID == Convert.ToInt32(webpage)).OrderBy(x => x.CategoryID);
            docs = Pager.Setup<Document>(this, docs.AsQueryable(), page, resultsPerPage);
            return View(docs);
        }

        [HttpPost]
        public ActionResult DeleteDocument(int id, string template)
        {
            DBDataContext db = Utils.DB.GetContext();
            Document d = db.Documents.SingleOrDefault(x => x.ID == id);
            string webpage = "0";
            string tmpl = template;
            if (d != null)
            {
                webpage = d.WebPageID.ToString();

                if (System.IO.File.Exists(HttpContext.Server.MapPath(d.Source)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(d.Source));
                }
                if (System.IO.File.Exists(HttpContext.Server.MapPath(d.Thumb)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(d.Thumb));
                }

                db.Documents.DeleteOnSubmit(d);

                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes");
                    ErrorHandler.Report.Exception(ex, "Body/DeleteDocument");
                }
            }

            return RedirectToAction("DocLib", "Body", new { controller = "Body", action = "DocLib", webpage = webpage, tmpl = tmpl });
        }

        public ActionResult ManageDocument(string webpage, string tmpl)
        {
            ViewData["WebPageID"] = webpage;
            ViewData["TemplateID"] = tmpl;
            return View("ManageDocument", new Document());
        }

        [HttpPost]
        public ActionResult ManageDocument(Document doc, DocumentFile file, ImageFile thumb, string category, string name, string webPageId, string tmpl)
        {
            string dbPath = "", dbThumbPath = "";

            DBDataContext db = Utils.DB.GetContext();

            //check for category <> 0
            int categoryId = 0;
            if (!string.IsNullOrEmpty(name))
            {
                //add category to database
                Category c = new Category()
                {
                    Name = name
                };
                try
                {
                    db.Categories.InsertOnSubmit(c);
                    db.SubmitChanges();
                    categoryId = c.ID;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred while adding category to the database.");
                    ErrorHandler.Report.Exception(ex, "Body/ManageDocument[HTTPPOST]");
                }
            }
            //no new category, try parsing existing one from dropdown list
            if (categoryId == 0)
            {
                Int32.TryParse(category, out categoryId);
            }

            //validate category
            if (categoryId > 0)
            {

                WebPage page = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webPageId));
                if (page != null)
                {

                    if (file != null)
                    {
                        file.ValidateForUpload(true);
                        if (ModelState.IsValid)
                        {
                            string documentName = "doc-lib_" + DateTime.Now.Ticks.ToString() + "_" + new Random().Next(1, 100).ToString() + System.IO.Path.GetExtension(file.Name);

                            if (file.Save(documentName))
                            {
                                dbPath = "/" + file.SavePath;
                                file.Cleanup();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Document file is not recognized");
                    }

                    if (thumb != null)
                    {
                        thumb.ValidateForUpload(true);
                        if (ModelState.IsValid)
                        {
                            string thumbName = "doc-lib_thumb_" + DateTime.Now.Ticks.ToString() + "_" + new Random().Next(1, 100).ToString();
                            if (thumb.Save(thumbName, new System.Drawing.Size(Config.DocLib.Thumbnail.Width, Config.DocLib.Thumbnail.Height), false))
                            {
                                dbThumbPath = "/" + thumb.SavePath;
                                thumb.Cleanup();
                            }
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Image file is not recognized");
                    }

                    if (!string.IsNullOrEmpty(dbPath) && !string.IsNullOrEmpty(dbThumbPath))
                    {
                        Category c = db.Categories.SingleOrDefault(x => x.ID == categoryId);
                        Document d = new Document()
                        {
                            Title = doc.Title,
                            Source = dbPath,
                            Thumb = dbThumbPath
                        };
                        d.Category = c;
                        page.Documents.Add(d);

                        try
                        {
                            db.SubmitChanges();
                            return RedirectToAction("DocLib", "Body", new { controller = "Body", action = "DocLib", webpage = webPageId.ToString(), tmpl = tmpl });

                        }
                        catch (Exception ex)
                        {
                            ModelState.AddModelError("", "An unknown error occurred. Please try again in a few minutes.");
                            ErrorHandler.Report.Exception(ex, "Body/ManageDocument[HTTPPOST]");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "There has been an issue with uploading your Image file. Please try again in few minutes.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Page cannot be found. Please try again in few minutes.");
                }
            }
            else
            {
                ModelState.AddModelError("category", "You must either add new or select valid category for this document.");
            }



            if (System.IO.File.Exists(HttpContext.Server.MapPath(dbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(dbPath));
            }
            if (System.IO.File.Exists(HttpContext.Server.MapPath(dbThumbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(dbThumbPath));
            }

            ViewData["WebPageID"] = webPageId;
            ViewData["TemplateID"] = tmpl;
            return View("ManageDocument", new Document());
            

        }

        /* EMPLOYEES */
        public ActionResult Employees(string webpage, string tmpl, int? page)
        {
            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Employee> employees = db.Employees.Where(x => x.WebPageID == Convert.ToInt32(webpage));
            employees = Pager.Setup<Employee>(this, employees.AsQueryable(), page, empResultsPerPage);
            return View(employees);
        }

        public ActionResult ManageEmployee(string webpage, string tmpl, int id = 0)
        {
            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            if (id > 0)
            {
                //modify
                //edit
                DBDataContext db = Utils.DB.GetContext();
                Employee e = db.Employees.SingleOrDefault(x => x.ID == id);
                if (e != null)
                {
                    ViewData["Title"] = "Edit Employee";
                    ViewData["Action"] = "UpdateEmployee";
                    return View(e);
                }
                else
                {
                    //cannot find account in database
                    return RedirectToAction("Employees", "Body", new { controller = "Body", action = "Employees", webpage = webpage, tmpl = tmpl });
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Employee";
                ViewData["Action"] = "AddEmployee";
                return View();
            }
        }

        [HttpPost]
        public ActionResult AddEmployee(Employee e, ImageFile photo, string webpage, string tmpl)
        {
            string dbPhotoPath = "";

            if (photo != null)
            {
                photo.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    if (photo.Save("emp", new System.Drawing.Size(Config.Employee.Photo.Width, Config.Employee.Photo.Height), false))
                    {
                        dbPhotoPath = "/" + photo.SavePath;
                    }

                }
            }
            else
            {
                ModelState.AddModelError("photo", "Photo does not exist.");
            }

            if (!string.IsNullOrEmpty(dbPhotoPath))
            {
                if(ModelState.IsValid)
                {
                    DBDataContext db = Utils.DB.GetContext();
                    WebPage page = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
                    if (page != null)
                    {
                        Employee emp = new Employee()
                        {
                            FirstName = e.FirstName,
                            LastName = e.LastName,
                            Title = e.Title,
                            Description = e.Description,
                            EmailAddress = e.EmailAddress,
                            Photo = dbPhotoPath
                        };

                        page.Employees.Add(emp);

                        try
                        {
                            db.SubmitChanges();
                            return RedirectToAction("Employees", "Body", new { controller = "Body", action = "Employees", webpage = webpage, tmpl = tmpl });
                        }
                        catch(Exception ex)
                        {
                            ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                            ErrorHandler.Report.Exception(ex, "Body/AddEmployee[HTTPPOST]");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Web Page does not exist.");
                    }

                }
            }


            if(System.IO.File.Exists(HttpContext.Server.MapPath(dbPhotoPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(dbPhotoPath));
            }

            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            ViewData["Title"] = "Add Employee";
            ViewData["Action"] = "AddEmployee";
            return View("ManageEmployee", e);

        }

        [HttpPost]
        public ActionResult UpdateEmployee(int id, ImageFile photo, string existingPhoto, string webpage, string tmpl)
        {
            DBDataContext db = Utils.DB.GetContext();

            string dbPhotoPath = existingPhoto;

            if (photo != null)
            {
                if (photo.HasFile)
                {
                    photo.ValidateForUpload(true);
                    if (ModelState.IsValid)
                    {

                        if (photo.Save("emp", new System.Drawing.Size(Config.Employee.Photo.Width, Config.Employee.Photo.Height), false))
                        {
                            dbPhotoPath = "/" + photo.SavePath;

                            if (System.IO.File.Exists(HttpContext.Server.MapPath(existingPhoto)))
                            {
                                System.IO.File.Delete(HttpContext.Server.MapPath(existingPhoto));
                            }
                        }
                    }                    
                }
            }
            


            Employee e = db.Employees.SingleOrDefault(x => x.ID == id);
            if (e != null)
            {
                
                TryUpdateModel(e);
                if (ModelState.IsValid)
                {
                    e.Photo = dbPhotoPath;

                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Employees", "Body", new { controller = "Body", action = "Employees", webpage = webpage, tmpl = tmpl });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        ErrorHandler.Report.Exception(ex, "Body/UpdateEmployee[HTTPPOST]");
                    }
                }
            }

            ViewData["WebPageID"] = webpage;
            ViewData["Template"] = tmpl;
            ViewData["Title"] = "Edit Employee";
            ViewData["Action"] = "UpdateEmployee";
            return View("ManageEmployee", e);

        }

        public ActionResult DeleteEmployee(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Employee e = db.Employees.SingleOrDefault(x => x.ID == id);
            if (e != null)
            {
                if (System.IO.File.Exists(HttpContext.Server.MapPath(e.Photo)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(e.Photo));
                }

                db.Employees.DeleteOnSubmit(e);

                try
                {
                    db.SubmitChanges();
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                    ErrorHandler.Report.Exception(ex, "Body/DeleteEmployee");
                }
            }

            return RedirectToAction("Employees", "Body", new { controller = "Body", action = "Employees", webpage = e.WebPageID.ToString(), tmpl = e.WebPage.Template.Code });
        }
    }
}
