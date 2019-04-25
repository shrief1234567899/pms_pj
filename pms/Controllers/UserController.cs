using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}