using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pms.Models;
using pms;
namespace pms.Controllers
{
    public class UserController : Controller
    {
        public ActionResult CustomerHome()
        {
            var loggedUser = (User)Session["LoggedUser"];
            if (loggedUser != null)
                return View("~/Views/Customer/Index.cshtml");
            else
                return RedirectToAction("Login", "User");
        }

        public ActionResult Profile()
        {
            var loggedUser = (User)Session["LoggedUser"];
            if (loggedUser != null)
                return View("~/Views/User/Profile.cshtml");
            else
                return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public ActionResult Registration()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration(User user)
        {
            try
            {
                bool Status = false;
                string message = "";
                //
                // Model Validation 
                if (ModelState.IsValid)
                {

                    #region //Email is already Exist 
                    var isExist = IsEmailExist(user.email);
                    if (isExist)
                    {
                        ModelState.AddModelError("EmailExist", "Email already exist");
                        return View(user);
                    }
                    #endregion
                    #region to encrypt the password
                    // user.password = Crypto.Hash(user.password);
                    #endregion
                    #region Save to Database
                    using (PMEntities dc = new PMEntities())
                    {
                        dc.Users.Add(user);
                        dc.SaveChanges();
                        Status = true;
                    }
                    #endregion
                    Session["LoggedUserID"] = user.Id.ToString();
                    Session["LoggedUser"] = user;
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    message = "Invalid Request";
                }

                ViewBag.Message = message;
                ViewBag.Status = Status;
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Invalid Request";
            }

            return View(user);


        }

        [NonAction]
        public bool IsEmailExist(string target_email)
        {
            using (PMEntities dc = new PMEntities())
            {
                var v = dc.Users.Where(a => a.email == target_email).FirstOrDefault();
                return v != null;
            }
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(User u)
        {
            // this action is for handle post (login)
            //if (ModelState.IsValid) // this is check validity
            //{
            using (PMEntities dc = new PMEntities())
            {
                var v = dc.Users.Where(a => a.email.Equals(u.email) && a.password.Equals(u.password)).FirstOrDefault();
                if (v != null)
                {
                    Session["LogedUserID"] = v.Id.ToString();
                    Session["LoggedUser"] = v;
                    return RedirectToAction("Index", "Home");
                }
            }
            //}
            ViewBag.Error = "Invalid User Name Or password";
            return View(u);
        }

        [HttpPost]
        public ActionResult LogOut()
        {
            Session["LoggedUser"] = null;

            return RedirectToAction("Login", "User");
        }

        [HttpPost]
        public ActionResult EditProfile(User newdata)
        {
            var loggedUser = (User)Session["LoggedUser"];
            using (PMEntities context = new PMEntities())
            {
                if (context.Users.Any(u => u.Id == newdata.Id))
                {
                    try
                    {
                        User editeduser = context.Users.Find(loggedUser.Id);
                        editeduser.first_name = newdata.first_name;
                        editeduser.last_name = newdata.last_name;
                        editeduser.email = newdata.email;
                        editeduser.jop_description = newdata.jop_description;
                        editeduser.mobile = newdata.mobile;
                        context.SaveChanges();
                        return Json(new { status = "200", data = editeduser, displaySweetAlert = true, message = "User Edited Successfully" }, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {
                        return Json(new { status = "400", data = " ", displaySweetAlert = true, message = ex.Message }, JsonRequestBehavior.AllowGet);
                    }
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "User Not found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public ActionResult GetUserProfile(int id)
        {
            var loggedUser = (User)Session["LoggedUser"];
            using (PMEntities context = new PMEntities())
            {
                if (context.Users.Any(u => u.Id == id))
                {
                    User userprofile = context.Users.Find(loggedUser.Id);
                    double pendingCount = context.projects.Count(p => p.status == 0);
                    double deliveredCount = context.projects.Count(p => p.status == 1);
                    double notdeliveredCount = context.projects.Count(p => p.status == 2);
                    double total = pendingCount + deliveredCount + notdeliveredCount;
                    double[] customerDashboard = { (pendingCount / total) * 100, (deliveredCount / total) * 100, (notdeliveredCount / total) * 100 };
                    return Json(new { status = "200", data = userprofile, dashboardData = customerDashboard, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "User Not found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}