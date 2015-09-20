using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMPSAdvisingDB;

namespace CMPSAdvisingDB.Controllers
{
    [Authorize(Roles = "Admin,Professor")]
    public class PrerequisitesController : Controller
    {
        private CMPSAdvising1Entities db = new CMPSAdvising1Entities();

        // GET: Prerequisites
        public ActionResult Index()
        {
            var prerequisites = db.Prerequisites.Include(p => p.BaseCourse_PreReqIsFor);
            return View(prerequisites.ToList());
        }

        // GET: Prerequisites/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prerequisite prerequisite = db.Prerequisites.Find(id);
            if (prerequisite == null)
            {
                return HttpNotFound();
            }
            return View(prerequisite);
        }

        // GET: Prerequisites/Create
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create()
        {
            ViewBag.BaseCourse_PreReqIsFor_ID = new SelectList(db.BaseCourses, "ID", "Name");
            return View();
        }

        // POST: Prerequisites/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create([Bind(Include = "ID,PrereqName,BaseCourse_PreReqIsFor_ID")] Prerequisite prerequisite)
        {
            if (ModelState.IsValid)
            {
                db.Prerequisites.Add(prerequisite);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BaseCourse_PreReqIsFor_ID = new SelectList(db.BaseCourses, "ID", "Name", prerequisite.BaseCourse_PreReqIsFor_ID);
            return View(prerequisite);
        }

        // GET: Prerequisites/Edit/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prerequisite prerequisite = db.Prerequisites.Find(id);
            if (prerequisite == null)
            {
                return HttpNotFound();
            }
            ViewBag.BaseCourse_PreReqIsFor_ID = new SelectList(db.BaseCourses, "ID", "Name", prerequisite.BaseCourse_PreReqIsFor_ID);
            return View(prerequisite);
        }

        // POST: Prerequisites/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Edit([Bind(Include = "ID,PrereqName,BaseCourse_PreReqIsFor_ID")] Prerequisite prerequisite)
        {
            if (ModelState.IsValid)
            {
                db.Entry(prerequisite).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BaseCourse_PreReqIsFor_ID = new SelectList(db.BaseCourses, "ID", "Name", prerequisite.BaseCourse_PreReqIsFor_ID);
            return View(prerequisite);
        }

        // GET: Prerequisites/Delete/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Prerequisite prerequisite = db.Prerequisites.Find(id);
            if (prerequisite == null)
            {
                return HttpNotFound();
            }
            return View(prerequisite);
        }

        // POST: Prerequisites/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult DeleteConfirmed(int id)
        {
            Prerequisite prerequisite = db.Prerequisites.Find(id);
            db.Prerequisites.Remove(prerequisite);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
