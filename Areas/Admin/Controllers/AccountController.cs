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
    public class AccountController : Controller
    {
        private const int resultsPerPage = 25;

        public ActionResult Index(int? page)
        {
            DBDataContext db = Utils.DB.GetContext();
            IEnumerable<Models.Account> query = db.Accounts.OrderBy(x => x.LastName).ThenBy(x => x.FirstName).Select(x => x);
            query = Pager.Setup<Account>(this, query.AsQueryable(), page, resultsPerPage);
            return View(query);
        }


        public ActionResult Manage(int id = 0)
        {
            if (id > 0)
            {
                //edit
                DBDataContext db = Utils.DB.GetContext();
                Account acc = db.Accounts.SingleOrDefault(x => x.ID == id);
                if (acc != null)
                {
                    ViewData["Title"] = "Edit Account";
                    ViewData["Action"] = "Update";
                    return View(acc);
                }
                else
                {
                    //cannot find account in database
                    return RedirectToAction("Index", "Account");
                }
            }
            else
            {
                //add
                ViewData["Title"] = "Add Account";
                ViewData["Action"] = "Add";
                return View();
            }
        }

        [HttpPost]
        public ActionResult Update(int id, string[] secRoles, string newPassword, FormCollection form)
        {

            DBDataContext db = Utils.DB.GetContext();
            Account acc = db.Accounts.SingleOrDefault(x => x.ID == id);
            if (acc != null)
            {
                TryUpdateModel(acc);
                db.Roles.DeleteAllOnSubmit(acc.Roles);

                if (secRoles != null)
                {
                    foreach (string s in secRoles)
                    {
                        try
                        {
                            Role r = new Role()
                            {
                                SecurityRole = (SecurityRole)Enum.Parse(typeof(SecurityRole), s)
                            };
                            acc.Roles.Add(r);
                        }
                        catch { }
                    }
                }

                if (ModelState.IsValid)
                {
                    if (!string.IsNullOrEmpty(newPassword))
                    {
                        if (WebIT.Lib.Utils.Validate.PasswordFormat(newPassword))
                        {
                            acc.Password = Security.Password.GenerateHash(acc.Email, newPassword);
                        }
                        else
                        {
                            ModelState.AddModelError("acc.Password", "Password format is not valid. Expecting 6+ characters(1 upper & 1 lower alpha, 1 numeric)");
                        }
                    }
                    if (ModelState.IsValid)
                    {
                        if (db.Accounts.Count(x => x.Email.Equals(acc.Email) && x.ID != id) > 0)
                        {
                            ModelState.AddModelError("acc.Email", "Email address is already in use. Please choose another one.");
                        }
                        else
                        {
                            try
                            {
                                db.SubmitChanges();

                                return RedirectToAction("Index", "Account");
                            }
                            catch
                            {
                                ModelState.AddModelError("", "An unknown error occurred. Please try again in a few minutes.");
                            }
                        }
                    }
                }

                ViewData["Title"] = "Edit Account";
                ViewData["Action"] = "Update";
                return View("Manage", acc);
            }

            return RedirectToAction("Index", "Account");

        }

        [HttpPost]
        public ActionResult Add(Account acc, string[] secRoles, string newPassword)
        {
            if (secRoles != null)
            {
                foreach (string s in secRoles)
                {
                    Role r = new Role()
                    {
                        SecurityRole = (SecurityRole)Enum.Parse(typeof(SecurityRole), s)
                    };
                    acc.Roles.Add(r);
                }
            }

            if (ModelState.IsValid)
            {
                if (!string.IsNullOrEmpty(newPassword))
                {
                    if (WebIT.Lib.Utils.Validate.PasswordFormat(newPassword))
                    {
                        acc.Password = Security.Password.GenerateHash(acc.Email, newPassword);
                    }
                    else
                    {
                        ModelState.AddModelError("acc.Password", "Password format is not valid. Expecting 6+ characters(1 upper & 1 lower alpha, 1 numeric)");
                    }
                    if (ModelState.IsValid)
                    {

                        DBDataContext db = Utils.DB.GetContext();

                        if (db.Accounts.Count(x => x.Email.Equals(acc.Email)) > 0)
                        {
                            ModelState.AddModelError("acc.Email", "Email address is already in use. Please choose another one.");
                        }
                        else
                        {
                            db.Accounts.InsertOnSubmit(acc);

                            try
                            {
                                acc.Registered = DateTime.Now;
                                acc.StatusID = db.Status.Single(x => x.Value.Equals("Active")).ID;

                                db.SubmitChanges();

                                return RedirectToAction("Index", "Account");
                            }
                            catch(Exception ex)
                            {
                                ModelState.AddModelError("", "An unknown error occurred. Please try again in few minutes.");
                                ErrorHandler.Report.Exception(ex, "Account/Add");
                            }
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Password is required");
                }
            }

            ViewData["Title"] = "Add Account";
            ViewData["Action"] = "Add";

            return View("Manage", acc);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            DBDataContext db = Utils.DB.GetContext();
            Account acc = db.Accounts.SingleOrDefault(x => x.ID == id);
            if (acc != null)
            {
                db.Roles.DeleteAllOnSubmit(acc.Roles);
                db.Accounts.DeleteOnSubmit(acc);
                db.SubmitChanges();
            }

            return RedirectToAction("Index", "Account");
        }

    }
}
