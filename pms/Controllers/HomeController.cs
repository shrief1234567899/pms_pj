using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pms.Models;
namespace pms.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var user = (User)Session["LoggedUser"];
            if (user.type == "admin")
                return RedirectToAction("AdminDashboard");
            else if(user.type == "customer")
                return View("~/Views/Customer/Index.cshtml");
            else if (user.type == "pm")
                return RedirectToAction("PmDashboard");
            // using (PMEntities context = new PMEntities())
            ///{
            /* User user = new User()
             {
                 first_name = "TMT",
                 last_name = "MEDHAT" ,
                 jop_description = "CEO OF TMT",
                 type = "Admin"

             };
             context.Users.Add(user);
             context.SaveChanges(); */

            // select data
            //User user = context.Users.FirstOrDefault(u => u.first_name == "TMT");
            // update
            //user.password = "123456";
            //remove
            //context.Users.Remove(user);
            //context.SaveChanges();
            //}
            return View();
        }

        public ActionResult AdminDashboard()
        {
            return View();
        }

        public ActionResult PmDashboard()
        {
            using (PMEntities context = new PMEntities())
            {
                var projects = context.projects.Where(c => c.status == 3).ToList();
                return View(projects);
            }

        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}