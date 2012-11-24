using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Temp.Models;
using WebIT.Lib;
using WebIT.Temp;

namespace WebIT.Temp.Areas.Admin.Controllers
{
    public class MediaLibraryController : Controller
    {
        private const int resultsPerPage = 25;

        public ActionResult Index(int? page)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Models.Media> query = db.Medias.OrderByDescending(x=>x.Type).Select(x => x);
            query = Pager.Setup<Media>(this, query.AsQueryable(), page, resultsPerPage);
            return View(query);
        }

        public ActionResult Manage()
        {
            //add
            ViewData["Title"] = "Add Media";
            ViewData["Action"] = "Add";
            return View("Manage");
        }

        [HttpPost]
        public ActionResult Add(Media m, HttpPostedFileBase file)
        {
            string dbPath = "", path = "";
            string fName = m.Name.ToLower().Replace(" ", "_");
            string ext = "";


            if (file != null)
            {
                ext = System.IO.Path.GetExtension(file.FileName);
                
            
                fName = ((m.Type == "I") ? "img_" : "fla_") + fName + ext;

                if (((m.Type == "I") ? Utils.Image.IsImage(ext) : ext.Equals(".swf")))
                {
                    path = Url.UploadPath(fName);
                    try
                    {
                        file.SaveAs(path);
                        dbPath = Url.Content(UrlHelperExtensions.ContentDirectory.data, fName);
                    }
                    catch
                    {
                        ModelState.AddModelError("", "An error occurred. Please try again in a few minutes.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "File is not recognized");
            }


            if (!string.IsNullOrEmpty(dbPath))
            {
                DBDataContext db = Utils.DB.GetContext();
                Media media = new Media()
                {
                    Name = m.Name,
                    Type = m.Type,
                    Path = dbPath
                };
                db.Medias.InsertOnSubmit(media);

                try
                {
                    db.SubmitChanges();
                    return RedirectToAction("Index", "MediaLibrary", new { controller = "MediaLibrary", action = "Index" });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.Message);
                    ErrorHandler.Report.Exception(ex, "MediaLibrary/Add");
                }
            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your Image/Video file. Please try again in few minutes.");
            }

            ViewData["Title"] = "Add Media";
            ViewData["Action"] = "Add";
            return View("Manage");




        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Media m = db.Medias.SingleOrDefault(x => x.ID == id);
            if (m != null)
            {
                if (System.IO.File.Exists(HttpContext.Server.MapPath(m.Path)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(m.Path));
                }
                

                db.Medias.DeleteOnSubmit(m);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes");
                    ErrorHandler.Report.Exception(ex, "MediaLibrary/Delete");
                }

            }
            return RedirectToAction("Index", "MediaLibrary", new { controller = "MediaLibrary", action = "Index"});

        }
    }
}
