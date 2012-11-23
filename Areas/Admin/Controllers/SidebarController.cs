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
    public class SidebarController : Controller
    {
        private const int resultsPerPage = 10;

        public ActionResult Index(string webpage)
        {
            ViewData["WebPageID"] = webpage;
            DBDataContext db = Utils.DB.GetContext();
            WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
            if (pg != null)
            {
                IEnumerable<Sidebar> query = pg.Sidebars;                
                return View(query);
            }

            return View();
        }

        public ActionResult Add(string webpage)
        {
            ViewData["WebPageID"] = webpage;
            return View(new Sidebar());
        }

        [HttpPost]
        public ActionResult Add(string webpage, ImageFile thumb, string link, int linkOverlay)
        {
            string dbPath = "";
            if (thumb != null)
            {
                thumb.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    thumb.Save("sidebar_link_thumb", new System.Drawing.Size(Config.Sidebar.Thumbnail.Width, Config.Sidebar.Thumbnail.Height), false);
                    dbPath = "/" + thumb.SavePath;
                    thumb.Cleanup();
                }
            }
            else
            {
                ModelState.AddModelError("", "Thumbnail is not recognized");
            }

            if (!string.IsNullOrEmpty(dbPath))
            {
                DBDataContext db = Utils.DB.GetContext();
                WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
                if (pg != null)
                {
                    pg.Sidebars.Add(new Sidebar() { TypeID = db.Types.Single(x => x.Name.Equals("Link")).ID, Source = link, Thumb = dbPath, IsOverlay = Convert.ToBoolean(linkOverlay) });
                   
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Sidebar", new { controller = "Sidebar", action = "Index", webpage = webpage });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        ErrorHandler.Report.Exception(ex, "Sidebar/Add[POST]");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Web Page is doesn't exist");
                }
            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your Thumbnail file. Please try again in few minutes.");
            }

            if (System.IO.File.Exists(HttpContext.Server.MapPath(dbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(dbPath));
            }

            ViewData["Type"] = "Link";
            ViewData["WebPageID"] = webpage;
            return View("Add");
        }

        [HttpPost]
        public ActionResult AddFlash(string webpage, ImageFile flashThumb, VideoFile flash, int flashOverlay)
        {
            string thumbPath = "";
            
            if (flashThumb != null)
            {
                flashThumb.ValidateForUpload(true);
                if (ModelState.IsValid)
                {

                    flashThumb.Save("sidebar_flash_thumb", new System.Drawing.Size(Config.Sidebar.Thumbnail.Width, Config.Sidebar.Thumbnail.Height), false);
                    thumbPath = "/" + flashThumb.SavePath;
                    flashThumb.Cleanup();
                }
            }
            else
            {
                ModelState.AddModelError("", "Thumbnail is not recognized");
            }

            string dbPath = "";
            if (flash != null)
            {
                flash.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    if (!System.IO.Path.GetExtension(flash.Name).Equals(".flv"))
                    {
                        ModelState.AddModelError("", "Invalid file type. Expected: .flv");
                    }
                    if (ModelState.IsValid)
                    {
                        flash.Save("sidebar_flash_file" + System.IO.Path.GetExtension(flash.Name));

                        dbPath = "/" + flash.SavePath;
                        flash.Cleanup();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "Video File is not recognized");
            }

           
            if (!string.IsNullOrEmpty(dbPath) && !string.IsNullOrEmpty(thumbPath))
            {
                DBDataContext db = Utils.DB.GetContext();
                WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
                if (pg != null)
                {
                    pg.Sidebars.Add(new Sidebar() { TypeID = db.Types.Single(x => x.Name.Equals("Video")).ID, Source = dbPath, Thumb = thumbPath, IsOverlay = Convert.ToBoolean(flashOverlay) });
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Sidebar", new { controller = "Sidebar", action = "Index", webpage = webpage });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        ErrorHandler.Report.Exception(ex, "Sidebar/AddFlash[POST]");
                        
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Web Page is doesn't exist");
                    
                }
            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your Video file. Please try again in few minutes.");
               
            }

           
                if (System.IO.File.Exists(HttpContext.Server.MapPath(dbPath)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(dbPath));
                }

                if (System.IO.File.Exists(HttpContext.Server.MapPath(thumbPath)))
                {
                    System.IO.File.Delete(HttpContext.Server.MapPath(thumbPath));
                }
           

            ViewData["Type"] = "Flash";
            ViewData["WebPageID"] = webpage;
            return View("Add");
        }

        [HttpPost]
        public ActionResult AddImage(string webpage, ImageFile imgThumb, ImageFile img, int imageOverlay)
        {
            string thumbPath = "";
            if (imgThumb != null)
            {
                imgThumb.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    imgThumb.Save("sidebar_image_thumb", new System.Drawing.Size(Config.Sidebar.Thumbnail.Width, Config.Sidebar.Thumbnail.Height), false);
                    thumbPath = "/" + imgThumb.SavePath;
                    imgThumb.Cleanup();
                }
            }
            else
            {
                ModelState.AddModelError("", "Thumbnail is not recognized");
            }

            string dbPath = "";
            if (img != null)
            {
                img.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    img.Save("sidebar_image_file" + System.IO.Path.GetExtension(img.Name));

                    dbPath = "/" + img.SavePath;
                    img.Cleanup();
                }
            }
            else
            {
                ModelState.AddModelError("", "Image File is not recognized");
            }

            if (!string.IsNullOrEmpty(dbPath) && !string.IsNullOrEmpty(thumbPath))
            {
                DBDataContext db = Utils.DB.GetContext();
                WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
                if (pg != null)
                {
                    pg.Sidebars.Add(new Sidebar() { TypeID = db.Types.Single(x => x.Name.Equals("Image")).ID, Source = dbPath, Thumb = thumbPath, IsOverlay = Convert.ToBoolean(imageOverlay) });
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Sidebar", new { controller = "Sidebar", action = "Index", webpage = webpage });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        ErrorHandler.Report.Exception(ex, "Sidebar/AddImage[POST]");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Web Page is doesn't exist");
                }
            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your Image file. Please try again in few minutes.");
            }

            if (System.IO.File.Exists(HttpContext.Server.MapPath(dbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(dbPath));
            }

            if (System.IO.File.Exists(HttpContext.Server.MapPath(thumbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(thumbPath));
            }

            ViewData["Type"] = "Image";
            ViewData["WebPageID"] = webpage;
            return View("Add");
        }

        [HttpPost]
        public ActionResult AddPDF(string webpage, ImageFile pdfThumb, DocumentFile pdf, int pdfOverlay)
        {
            string thumbPath = "";
            if (pdfThumb != null)
            {
                pdfThumb.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    pdfThumb.Save("sidebar_pdf_thumb", new System.Drawing.Size(Config.Sidebar.Thumbnail.Width, Config.Sidebar.Thumbnail.Height), false);

                    thumbPath = "/" + pdfThumb.SavePath;
                    pdfThumb.Cleanup();
                }
            }
            else
            {
                ModelState.AddModelError("", "PDF File is not recognized");
            }
            
            string dbPath = "";
            if (pdf != null)
            {
                pdf.ValidateForUpload(true);
                if (ModelState.IsValid)
                {
                    if (System.IO.Path.GetExtension(pdf.Name).Equals(".pdf"))
                    {
                        ModelState.AddModelError("", "Invalid file type. Expected: .pdf");
                    }
                    if (ModelState.IsValid)
                    {
                        pdf.Save("sidebar_pdf_file" + System.IO.Path.GetExtension(pdf.Name));

                        dbPath = "/" + pdf.SavePath;
                        pdf.Cleanup();
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "PDF File is not recognized");
            }

            if (!string.IsNullOrEmpty(dbPath) && !string.IsNullOrEmpty(thumbPath))
            {
                DBDataContext db = Utils.DB.GetContext();
                WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
                if (pg != null)
                {
                    pg.Sidebars.Add(new Sidebar() { TypeID = db.Types.Single(x => x.Name.Equals("PDF")).ID, Source = dbPath, Thumb = thumbPath, IsOverlay = Convert.ToBoolean(pdfOverlay) });
                    try
                    {
                        db.SubmitChanges();
                        return RedirectToAction("Index", "Sidebar", new { controller = "Sidebar", action = "Index", webpage = webpage });
                    }
                    catch (Exception ex)
                    {
                        ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                        ErrorHandler.Report.Exception(ex, "Sidebar/AddPDF[POST]");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Web Page is doesn't exist");
                }
            }
            else
            {
                ModelState.AddModelError("", "There has been an issue with uploading your PDF file. Please try again in few minutes.");
            }

            if (System.IO.File.Exists(HttpContext.Server.MapPath(dbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(dbPath));
            }

            if (System.IO.File.Exists(HttpContext.Server.MapPath(thumbPath)))
            {
                System.IO.File.Delete(HttpContext.Server.MapPath(thumbPath));
            }
            ViewData["Type"] = "PDF";
            ViewData["WebPageID"] = webpage;
            return View("Add");
        }

        [HttpPost]
        public ActionResult Delete(int id, string webpage)
        {
            DBDataContext db = Utils.DB.GetContext();
            WebPage pg = db.WebPages.SingleOrDefault(x => x.ID == Convert.ToInt32(webpage));
            if(pg != null)
            {
                Sidebar s = pg.Sidebars.SingleOrDefault(x => x.ID == id);
                if (s != null)
                {
                    if (System.IO.File.Exists(HttpContext.Server.MapPath(s.Thumb)))
                    {
                        System.IO.File.Delete(HttpContext.Server.MapPath(s.Thumb));
                    }

                    if(!s.Type.Name.Equals("Link"))
                    {
                        if(System.IO.File.Exists(HttpContext.Server.MapPath(s.Source)))
                        {
                            System.IO.File.Delete(HttpContext.Server.MapPath(s.Source));
                        }
                    }

                    db.Sidebars.DeleteOnSubmit(s);

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
            }
            return RedirectToAction("Index", "Sidebar", new { controller = "Sidebar", action = "Index", webpage = webpage });
        }
    }
}
