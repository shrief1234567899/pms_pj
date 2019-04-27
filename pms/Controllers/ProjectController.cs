using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using pms.Models;

namespace pms.Controllers
{
    public class ProjectController : Controller
    {
        public ActionResult Index()
        {
            var loggedUser = (User)Session["LoggedUser"];
            if(loggedUser!=null)
                return View();
            else
                return RedirectToAction("Login", "User");
        }

        [HttpGet]
        public ActionResult GetProjectByStatus(int Id)
        {
            if (Session["LoggedUser"] == null)
            {
                return RedirectToAction("Login", "User");
            }
            var loggedUser = (User)Session["LoggedUser"];
            using (PMEntities context = new PMEntities())
            {
                if(loggedUser.type == "admin")
                {
                    if(context.projects.Any(p => (p.status == Id)))
                    {
                        var projectsData = context.projects.Where(p => (p.status == Id)).ToList();

                        foreach (var pj in projectsData)
                        {
                            User user = context.Users.Find(pj.owner_id);
                            pj.owner = user;
                        }
                        return Json(new { status = "200", data = projectsData, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);

                    }
                    else if (Id == -1)
                    {
                        var projectsData = context.projects.ToList();
                        foreach(var pj in projectsData)
                        {
                            User user = context.Users.Find(pj.owner_id);
                            pj.owner = user;
                        }
                        //var projectsData = context.projects.ToList();

                        if (projectsData.Count == 0)
                            return Json(new { status = "404", data = "", message = "No Project Found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);

                        return Json(new { status = "200", data = projectsData, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(new { status = "404", data = "", message = "No data found with this status", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                    }

                }
                if (context.projects.Any(p => (p.status == Id && p.owner_id == loggedUser.Id)))
                {
                    var projectsData = context.projects.Where(p => (p.status == Id && p.owner_id == loggedUser.Id)).ToList();
                    return Json(new { status = "200", data = projectsData, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
                else if (Id == -1)
                {
                    var projectsData = context.projects.Where(p => (p.owner_id == loggedUser.Id)).ToList();

                    if (projectsData.Count == 0)
                        return Json(new { status = "404", data = "", message = "No Project Found", displaySweetAlert = false}, JsonRequestBehavior.AllowGet);

                    return Json(new { status = "200", data = projectsData, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "No data found with this status", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [HttpGet]
        public ActionResult GetProject(int id)
        {
            using (PMEntities context = new PMEntities())
            {
                if (context.projects.Any(p => p.Id == id))
                {
                    project pj = context.projects.Find(id);
                    return Json(new { status = "200", data = pj, displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "project not found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        [HttpPost]
        public ActionResult ChangeStatus(project editData)
        {
            using (PMEntities context = new PMEntities())
            {
                project pj = context.projects.Find(editData.Id);
                pj.status = editData.status;
                context.SaveChanges();
                return Json(new { status = "200", data = pj });
            }
        }
        [HttpPost]
        public ActionResult EditProject(project editdata)
        {
            using (PMEntities context = new PMEntities())
            {
                if (context.projects.Any(p => p.Id == editdata.Id))
                {
                    project pj = context.projects.Find(editdata.Id);
                    pj.name = editdata.name;
                    pj.description = editdata.description;
                    context.SaveChanges();
                    return Json(new { status = "200", data = pj, displaySweetAlert = true, message = "Project Edited Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "project not found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
        public ActionResult AdminAddProject()
        {
            using (PMEntities context = new PMEntities())
            {
                var users = context.Users.Where(u => u.type =="customer").ToList();
                ViewBag.users = users;
                return View();
            }
        }
        [HttpPost]
        public ActionResult AdminAddProject(project newproject)
        {
            using (PMEntities context = new PMEntities())
            {
                int maxPj = context.projects.Count();
                if (maxPj > 0)
                {
                    project last = context.projects.OrderByDescending(p => p.Id).FirstOrDefault();
                    maxPj = last.Id;
                }
                project pj = new project
                {
                    Id = maxPj + 1,
                    name = newproject.name,
                    status = 0,
                    description = newproject.description,
                    owner_id = newproject.owner_id
                };
                context.projects.Add(pj);
                context.SaveChanges();
                
                var users = context.Users.Where(u => u.type == "customer").ToList();
                ViewBag.users = users;
                    
                
                ViewBag.Message = "Project Created Successfully";
                return View();
            }

        }
        [HttpPost]
        public ActionResult AddProject(project newproject)
        {
            var loggedUser = (User)Session["LoggedUser"];
            using (PMEntities context = new PMEntities())
            {
                int maxPj = context.projects.Count();
                if (maxPj > 0)
                {
                    project last = context.projects.OrderByDescending(p => p.Id).FirstOrDefault();
                    maxPj = last.Id;
                }
                project pj = new project
                {
                    Id = maxPj + 1,
                    name = newproject.name,
                    status = 0,
                    description = newproject.description,
                    owner_id = loggedUser.Id
                };
                context.projects.Add(pj);
                context.SaveChanges();
                return Json(new { status = "200", data = pj, displaySweetAlert = true, message = "Project Added Successfully" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult DeleteProject(int id)
        {
            using (PMEntities context = new PMEntities())
            {
                if (context.projects.Any(p => p.Id == id))
                {
                    var project = new project { Id = id };
                    context.projects.Attach(project);
                    context.projects.Remove(project);
                    context.SaveChanges();
                    return Json(new { status = "200", data = " ", displaySweetAlert = true, message = "Project Deleted Successfully" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { status = "404", data = "", message = "project not found", displaySweetAlert = false }, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}