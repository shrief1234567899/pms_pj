using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
            return View("~/Views/Customer/Index.cshtml");
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