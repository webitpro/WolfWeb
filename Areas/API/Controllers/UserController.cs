using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Lib;
using System.IO;
using System.Net;

namespace WebIT.Temp.Areas.API.Controllers
{
    public class UserController : Controller
    {
        [HttpPost]
        public ActionResult Login(string email, string password, string jsonp = "")
        {
            APIResponse api = new APIResponse();
            List<string> error = new List<string>();

            bool bPsw = false, bUid = false;
            if (!string.IsNullOrEmpty(email))
            {
                if (Utils.Validate.EmailAddress(email))
                {
                    bUid = true;
                }
                else
                {
                    error.Add("Email format is not valid");
                }
            }
            else
            {
                error.Add("Email is required");
            }

            if (!string.IsNullOrEmpty(password))
            {
                if (Utils.Validate.PasswordFormat(password))
                {
                    bPsw = true;
                }
                else
                {
                    error.Add("Password format is not valid");
                }

            }
            else
            {
                error.Add("Password is required");
            }

            if (bUid && bPsw)
            {
                if (Security.Authenticate(email, password, false))
                {
                    api.Success = true;
                }
                else
                {
                    error.Add("Invalid authentication credentials. Please try again.");
                }
            }

            api.Errors = error;

            if (!string.IsNullOrEmpty(jsonp))
            {
                return new JsonpResponse(jsonp, api);
            }
            else
            {
                return Json(api, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public ActionResult Logout(string jsonp = "")
        {
            APIResponse api = new APIResponse();

            Security.ClearCredentials();

            api.Success = true;

            if (!string.IsNullOrEmpty(jsonp))
            {
                return new JsonpResponse(jsonp, api);
            }
            else
            {
                return Json(api, JsonRequestBehavior.DenyGet);
            }
        }

        public DownloadableFile Shortcut(string name, string url, string jsonp = "")
        {
         
            string path = HttpContext.Server.MapPath("/Content/data/");

            //write file to disk
            StreamWriter sw = new StreamWriter(path + name);
            sw.WriteLine("[InternetShortcut]");
            sw.WriteLine("URL=" + url);
            sw.Close();

            return new DownloadableFile()
            {
                FileName = name,
                Path = "/Content/data/" + name
            };






        }
    }
}
