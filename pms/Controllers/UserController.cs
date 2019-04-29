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
        public ActionResult Edit(User user)
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
            ModelState.Clear();
            var loggedUser = (User)Session["LoggedUser"];
            using (PMEntities context = new PMEntities())
            {
                if (context.Users.Any(u => u.Id == loggedUser.Id))
                {
                    try
                    {
                        User editeduser = context.Users.Find(loggedUser.Id);
                        editeduser.first_name = newdata.first_name;
                        editeduser.last_name = newdata.last_name;
                        editeduser.email = newdata.email;
                        editeduser.jop_description = newdata.jop_description;
                        editeduser.type = editeduser.type;
                        editeduser.password = editeduser.password;
                        editeduser.ConfirmPassword = editeduser.password;
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
                if (context.Users.Any(u => u.Id == loggedUser.Id))
                {
                    User userprofile = context.Users.Find(loggedUser.Id);
                    double pendingCount = context.projects.Count(p => (p.status == 0 && p.owner_id == loggedUser.Id));
                    double deliveredCount = context.projects.Count(p => (p.status == 1 && p.owner_id == loggedUser.Id));
                    double notdeliveredCount = context.projects.Count(p => (p.status == 2 && p.owner_id == loggedUser.Id));
                    double inprogress = context.projects.Count(p => (p.status == 5 && p.owner_id == loggedUser.Id));
                    double total = pendingCount + deliveredCount + notdeliveredCount + inprogress;
                    if (total == 0)
                        total = 1;
                    double[] customerDashboard = { (pendingCount / total) * 100, (deliveredCount / total) * 100, (notdeliveredCount / total) * 100 , (inprogress / total) * 100 };
                    return Json(new { status = "200", data = userprofile, dashboardData = customerDashboard, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "User Not found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpPost]
        public ActionResult ProfilePhotoHandler()
        {
            var loggedUser = (User)Session["LoggedUser"];

            if (Request.Files != null)
            {
                HttpPostedFileBase file = Request.Files[0];
                int fileSize = file.ContentLength;
                string fileName = file.FileName;
                string mimeType = file.ContentType;
                if (mimeType.Contains("image"))
                {
                    System.IO.Stream fileContent = file.InputStream;
                    string fullFileName = new Random().Next().ToString() + fileName + System.IO.Path.GetExtension(fileName);
                    file.SaveAs(Server.MapPath("~/uploads/") + fullFileName);
                    using (PMEntities context = new PMEntities())
                    {
                        User editeduser = context.Users.Find(loggedUser.Id);
                        editeduser.photo = fullFileName;
                        editeduser.first_name = editeduser.first_name;
                        editeduser.last_name = editeduser.last_name;
                        editeduser.email = editeduser.email;
                        editeduser.jop_description = editeduser.jop_description;
                        editeduser.type = editeduser.type;
                        editeduser.password = editeduser.password;
                        editeduser.ConfirmPassword = editeduser.password;
                        editeduser.mobile = editeduser.mobile;
                        context.SaveChanges();
                    }
                    return Json(new { status = "200", data = fullFileName, displaySweetAlert = true, message = "Photo Uploaded Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", displaySweetAlert = true, message = "File not allowed" }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { status = "404", data = "", displaySweetAlert = true, message = "Retry Again" }, JsonRequestBehavior.AllowGet);
        }

        
    }
}