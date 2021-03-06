﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using CMPSAdvisingDB.Models;

namespace CMPSAdvisingDB.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        
        private CMPSAdvising1Entities db = new CMPSAdvising1Entities();
        private ApplicationDbContext _db = new ApplicationDbContext();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }


        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                string userID = User.Identity.GetUserId();
                string userW = _db.Users.Find(userID).wNumber;
                ViewBag.IsStudent = false;
                ViewBag.IsProf = false;
                if (User.IsInRole("Student") || User.IsInRole("Professor") && userW != null)
                {
                    Student studentLoggedIn = db.Students.Where(i => i.WNumber == userW).FirstOrDefault();
                    Professor profLoggedIn = db.Professors.Where(i => i.WNumber == userW).FirstOrDefault();
                    if (studentLoggedIn != null)
                    {
                        ViewBag.UserID = studentLoggedIn.ID;
                        ViewBag.IsStudent = true;
                        if (User.IsInRole("Student") && (!User.IsInRole("Admin")) && (!User.IsInRole("Professor")))
                        {
                            return RedirectToAction("Details", "Students", new { id = studentLoggedIn.ID });
                        }
                    }
                    if (profLoggedIn != null)
                    {
                        ViewBag.UserID = profLoggedIn.ID;
                        ViewBag.IsProf = true;
                    }
                    else
                    {
                        ViewBag.UserID = userID;
                    }
                }
            }
            return View();
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