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
            return View("~/Views/Customer/Index.cshtml");
        }

        public ActionResult Profile()
        {
            return View("~/Views/User/Profile.cshtml");
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
    }
}