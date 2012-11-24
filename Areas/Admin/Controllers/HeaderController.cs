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
    public class HeaderController : Controller
    {
        private const int resultsPerPage = 25;

        public ActionResult Index(int webpage, int? page)
        {
            ViewData["WebPageID"] = webpage.ToString();
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Header> query = db.Headers.Where(x=>x.WebPageID == webpage).Select(x => x);
            query = Pager.Setup<Header>(this, query.AsQueryable(), page, resultsPerPage);
            return View(query);
        }

        /// <summary>
        /// Load View (ADD/EDIT)
        /// </summary>
        /// <param name="webpage">WebPage ID</param>
        /// <returns></returns>
        public ActionResult Manage(string webpage)
        {
            ViewData["WebPageID"] = webpage;

            //add
            ViewData["Title"] = "Add Header";
            ViewData["Action"] = "Add";
            return View("Manage");
        }

        [HttpPost]
        public ActionResult Add(ImageFile image, VideoFile video, string webPageId)
        {
            string dbImagePath = "";
            string dbVideoPath = "";
            string videoFileName = "";
            string imageFileName = "";

            if (image != null)
            {
                image.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    if (image.Save("header_image", new System.Drawing.Size(Config.Header.Image.Width, Config.Header.Image.Height), false))
                    {
                        dbImagePath = "/" + image.SavePath;
                        imageFileName = image.SavedName;
                        image.Cleanup();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Image File is not recognized");
            }

            if (video != null)
            {
                video.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    if (!System.IO.Path.GetExtension(video.Name).Equals(".swf"))
                    {
                        ModelState.AddModelError("", "Invalid file type. Expected: .swf");
                    }

                    if (ModelState.IsValid)
                    {
                        if (video.Save("header_flash" + System.IO.Path.GetExtension(video.Name)))
                        {
                            dbVideoPath = "/" + video.SavePath;
                            videoFileName = video.SavedName;
                            video.Cleanup();
                        }
                    }
                    
                }
            }
            else
            {
                ModelState.AddModelError("", "Video File is not recognized");
            }

            bool removeFiles = false;

            if(!string.IsNullOrEmpty(dbImagePath) && !string.IsNullOrEmpty(dbVideoPath))
            {
                DBDataContext db = Utils.DB.GetContext();
                WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webPageId));
                if (pg != null)
                {
                    Header h = new Header()
                    {
                        ImagePath = dbImagePath,
                        MoviePath = dbVideoPath
                    };
                    pg.Headers.Add(h);

                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Header", new { controller = "Header", action = "Index", webpage = webPageId });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", ex.Message);
                        ErrorHandler.Report.Exception(ex, "Header/Add");
                        removeFiles = true;
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Web Page does not exist");
                    removeFiles = true;
                }
            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your Image/Video file. Please try again in few minutes.");
                removeFiles = true;
            }

            if (removeFiles)
            {
                if (System.IO.File.Exists(Url.UploadPath(imageFileName)))
                {
                    System.IO.File.Delete(Url.UploadPath(imageFileName));
                }

                if(System.IO.File.Exists(Url.UploadPath(videoFileName)))
                {
                    System.IO.File.Delete(Url.UploadPath(videoFileName));
                }
            }

            ViewData["Title"] = "Add Header";
            ViewData["Action"] = "Add";
            return View("Manage");

        }

        [HttpPost]
        public ActionResult Delete(int id, string webPageId)
        {
            DBDataContext db = Utils.DB.GetContext();
            Header h = db.Headers.SingleOrDefault(x => x.ID == id);
            if (h != null)
            {
                if (System.IO.File.Exists(HttpContext.Server.MapPath(h.ImagePath)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(h.ImagePath));
                }
                if (System.IO.File.Exists(HttpContext.Server.MapPath(h.MoviePath)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(h.MoviePath));
                }

                db.Headers.DeleteOnSubmit(h);
                try
                {
                    db.SubmitChanges();
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes");
                    ErrorHandler.Report.Exception(ex, "Header/Delete");
                }

            }
            return RedirectToAction("Index", "Header", new { controller = "Header", action = "Index", webpage = webPageId });

        }
    }
}
