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
    [Authorize]
    public class CoursesController : Controller
    {
        
        private CMPSAdvising1Entities db = new CMPSAdvising1Entities();

        // GET: Courses
        public ActionResult Index()
        {
            var courses = db.Courses.Include(c => c.BaseCourse).Include(c => c.Student_Taken).Include(c => c.Student_Recommend);
            return View(courses.ToList());
        }

        // GET: Courses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create()
        {
            ViewBag.BaseCourseID = new SelectList(db.BaseCourses, "ID", "Name");
            ViewBag.Student_ID_Taken = new SelectList(db.Students, "ID", "FirstName");
            ViewBag.Student_ID_Recommend = new SelectList(db.Students, "ID", "FirstName");
            return View();
        }

        // POST: Courses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create([Bind(Include = "ID,Semester,Grade,Selected,BaseCourseID,Student_ID_Taken,Student_ID_Recommend")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Courses.Add(course);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BaseCourseID = new SelectList(db.BaseCourses, "ID", "Name", course.BaseCourseID);
            ViewBag.Student_ID_Taken = new SelectList(db.Students, "ID", "FirstName", course.Student_ID_Taken);
            ViewBag.Student_ID_Recommend = new SelectList(db.Students, "ID", "FirstName", course.Student_ID_Recommend);
            return View(course);
        }

        // GET: Courses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID, BaseCourseID, Semester, Grade, Student_ID_Taken")] Course course)
        {
            if (ModelState.IsValid)
            {
                db.Entry(course).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details", "Students", new { id=course.Student_ID_Taken });
            }
            
            return View(course);
        }

        // GET: Courses/Delete/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Course course = db.Courses.Find(id);
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult DeleteConfirmed(int id)
        {
            Course course = db.Courses.Find(id);
            db.Courses.Remove(course);
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
