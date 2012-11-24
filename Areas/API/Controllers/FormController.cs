using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebIT.Lib;

namespace WebIT.Temp.Areas.API.Controllers
{
    public class FormController : Controller
    {
        
        [HttpPost]
        public ActionResult ContactUs(string name, string email, string message, string jsonp = "")
        {
            APIResponse api = new APIResponse();
            List<string> error = new List<string>();

            bool bValid = true;
            if (WebIT.Lib.Utils.Validate.ExpressionType(name) != Lib.Type.xPression.Numeric && name != "name")
            {
                bValid = bValid && true;
            }
            else
            {
                bValid = bValid && false;
                error.Add("Invalid name format");
            }

            if (WebIT.Lib.Utils.Validate.EmailAddress(email) && email != "email")
            {
                bValid = bValid && true;
            }
            else
            {
                error.Add("Invalid email format");
            }

            if (WebIT.Lib.Utils.Validate.ExpressionType(message) != Lib.Type.xPression.Numeric)
            {
                bValid = bValid && true;
            }
            else
            {
                bValid = bValid && false;
                error.Add("Message cannot be numeric value");
            }

            if (bValid)
            {
                //send email
                string body = "<table>"
                   + "<tr>"
                       + "<td>Name</td><td>" + name + "</td>"
                   + "</tr>"
                   + "<tr>"
                       + "<td>Email Address</td><td>" + email + "</td>"
                   + "</tr>"
                   + "<tr>"
                       + "<td>Date</td><td>" + message + "</td>"
                   + "</tr>"
                   + "</table>";
                try
                {
                    Utils.Email.sendEmail(Config.ActiveConfiguration.Mail.From, Config.Email.Form.ContactUs.Recipient, Config.Email.Form.ContactUs.Subject, body, true, Config.ActiveConfiguration.Mail.Host, Config.ActiveConfiguration.Mail.Port);
                    api.Success = true;
                }
                catch
                {
                    error.Add("An unknown error occurred. Please try again in a few minutes.");
                    api.Success = false;
                }
            }
            else
            {
                api.Success = false;
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
        public ActionResult WebConferenceRequest(string name, string email, string date, string time, string jsonp = "")
        {
            APIResponse api = new APIResponse();
            List<string> error = new List<string>();

            bool bValid = true;
            if (WebIT.Lib.Utils.Validate.ExpressionType(name) != Lib.Type.xPression.Numeric && name != "name")
            {
                bValid = bValid && true;
            }
            else
            {
                bValid = bValid && false;
                error.Add("Invalid name format");
            }

            if (WebIT.Lib.Utils.Validate.EmailAddress(email) && email != "email")
            {
                bValid = bValid && true;
            }
            else
            {
                error.Add("Invalid email format");
            }

            if (date != "requested date")
            {
                bValid = bValid && true;
            }
            else
            {
                bValid = bValid && false;
                error.Add("Invalid date format");
            }

            if (time != "requested time")
            {
                bValid = bValid && true;
            }
            else
            {
                bValid = bValid && false;
                error.Add("Invalid time format");
            }

            if (bValid)
            {
                //send email
                string body = "<table>"
                    + "<tr>"
                        + "<td>Name</td><td>" + name + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td>Email Address</td><td>" + email + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td>Date</td><td>" + date + "</td>"
                    + "</tr>"
                    + "<tr>"
                        + "<td>Time</td><td>" + time + "</td>"
                    + "</tr>"
                    + "</table>";
                try
                {
                    Utils.Email.sendEmail(Config.ActiveConfiguration.Mail.From, Config.Email.Form.WebConferenceRequest.Recipient, Config.Email.Form.WebConferenceRequest.Subject, body, true, Config.ActiveConfiguration.Mail.Host, Config.ActiveConfiguration.Mail.Port);
                    api.Success = true;
                }
                catch
                {
                    error = new List<string>() { "An unknown error occurred. Please try again in a few minutes." };
                    api.Success = false;
                }

            }
            else
            {
                api.Success = false;
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

    }
}
