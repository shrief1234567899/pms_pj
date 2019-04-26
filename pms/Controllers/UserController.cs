using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pms.Models;
using pms;
using System.IO;
using System.Data.Entity.Migrations;

namespace pms.Controllers
{
    public class UserController : Controller
    {
        // view all users
        public ActionResult Index()
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            User current_user = (User)Session["LoggedUser"];
            int user_id = current_user.Id;
            using (PMEntities context = new PMEntities())
            {
                var users = context.Users.Where(u => u.Id != user_id).ToList();
                return View(users);
            }

        }
        
        public ActionResult Delete(int id)
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            using (PMEntities context = new PMEntities())
            {
                var users = context.Users.FirstOrDefault(u => u.Id == id);
                context.Users.Remove(users);
                context.SaveChanges();
            }

            ViewBag.Message = "Deleted Successfully";
            return RedirectToAction("Index");
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Edit(int id)
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            
            using (PMEntities context = new PMEntities())
            {
                var users = context.Users.Where(u => u.Id == id).FirstOrDefault();
                return View(users);
            }
        }
        [HttpPost]
        public ActionResult Edit (User user)
        {
            

            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            // Model Validation 
            if (ModelState.IsValid)
            {
                if(user.ImageFile != null)
                {

                string fileName = Path.GetFileNameWithoutExtension(user.ImageFile.FileName);
                string fileExt = Path.GetExtension(user.ImageFile.FileName);
                fileName = fileName + DateTime.Now.ToString("yymmssfff") + fileExt;
                user.photo = "~/Images/" + fileName;
                fileName = Path.Combine(Server.MapPath("~/Images"), fileName);
                user.ImageFile.SaveAs(fileName);
                }
                
                using (PMEntities context = new PMEntities())
                {
                    User updated_user = context.Users.FirstOrDefault(u => u.Id == user.Id);
                    updated_user.first_name = user.first_name;
                    updated_user.last_name = user.last_name;

                    updated_user.jop_description = user.jop_description;

                    updated_user.type = user.type;

                    updated_user.password = user.password;
                    updated_user.ConfirmPassword = user.ConfirmPassword;
                    updated_user.photo = user.photo;
                    updated_user.mobile = user.mobile;
                    updated_user.email = user.email;


                    context.SaveChanges();
                    ViewBag.Message = "Edit Successfully";
                    return View(user);
                }
            }
            ViewBag.Error = "Invaild Request";
            return View(user);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            // Model Validation 
            if (ModelState.IsValid)
            {
                if (user.ImageFile != null)
                {
                    string fileName = Path.GetFileNameWithoutExtension(user.ImageFile.FileName);
                    string fileExt = Path.GetExtension(user.ImageFile.FileName);
                    fileName = fileName + DateTime.Now.ToString("yymmssfff") + fileExt;
                    user.photo = "~/Images/" + fileName;
                    fileName = Path.Combine(Server.MapPath("~/Images"), fileName);
                    user.ImageFile.SaveAs(fileName);
                }
                using (PMEntities context = new PMEntities())
                {
                    context.Users.Add(user);
                    context.SaveChanges();
                }
                ViewBag.Message = "Created Successfully";
            }
            else
            {
                ViewBag.Error = "Invalid Request";
            }

            return View();
        }

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