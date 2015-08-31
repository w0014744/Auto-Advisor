using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMPSAdvisingDB;
using PagedList;

namespace CMPSAdvisingDB.Controllers
{
    public class BaseCoursesController : Controller
    {
        private CMPSAdvising1Entities db = new CMPSAdvising1Entities();

        [Authorize(Roles = "Admin,Professor")]
        public ActionResult AddPreReq (int? id)
        {
            BaseCourse baseCourse = db.BaseCourses.Find(id);
            return View("AddPreReq", baseCourse);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult AddPreReq (int? id, string preReqName)
        {
            BaseCourse bc = db.BaseCourses.Find(id);
            Prerequisite prereq = new Prerequisite();
            prereq.BaseCourse_PreReqIsFor = bc;
            prereq.PrereqName = preReqName;
            db.Prerequisites.Add(prereq);
            bc.Prerequisites.Add(prereq);
            db.Entry(bc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id=bc.ID });
        }

        [Authorize(Roles = "Admin,Professor")]
        public ActionResult ResetPreReqs (int? id)
        {
            BaseCourse bc = db.BaseCourses.Find(id);
            db.Prerequisites.RemoveRange(bc.Prerequisites);
            bc.Prerequisites.Clear();
            db.Entry(bc).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = bc.ID });
        }


        // GET: BaseCourses
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.DeptSortParm = String.IsNullOrEmpty(sortOrder) ? "dept_desc" : "";
            ViewBag.NumberSortParm = sortOrder == "Number" ? "number_desc" : "Number";
           
            if (searchString == null)
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var bCourses = from b in db.BaseCourses select b;

            if (!String.IsNullOrEmpty(searchString))
            {
                string[] searchTerms = { "", "" };

                string[] splitSearch = searchString.Split(' ');

                for (int i = 0; i < splitSearch.Length; i++)
                {
                    searchTerms[i] = splitSearch[i];
                }

                    if (!String.IsNullOrEmpty(searchTerms[1]))
                    {
                        string dept = searchTerms[0].ToUpper();
                        string cn = searchTerms[1];
                        bCourses = bCourses.Where(b => b.Department.Contains(dept) && b.CourseNumber.ToString().Contains(cn));
                    }
                    else
                    {
                        bCourses = bCourses.Where(b => b.Department.Contains(searchString.ToUpper())
                                                  || b.CourseNumber.ToString().Contains(searchString));
                    }
            }

            switch (sortOrder)
            {
                case "dept_desc":
                    bCourses = bCourses.OrderByDescending(b => b.Department);
                    break;
                case "Number":
                    bCourses = bCourses.OrderBy(b => b.CourseNumber);
                    break;
                case "number_desc":
                    bCourses = bCourses.OrderByDescending(b => b.CourseNumber);
                    break;
                default:
                    bCourses = bCourses.OrderBy(b => b.Department);
                    break;
            }

            int pageSize = 10;
            int pageNumber = (page ?? 1);

            return View(bCourses.ToPagedList(pageNumber, pageSize));
        }

        // GET: BaseCourses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaseCourse baseCourse = db.BaseCourses.Find(id);
            if (baseCourse == null)
            {
                return HttpNotFound();
            }
            return View(baseCourse);
        }

        // GET: BaseCourses/Create
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: BaseCourses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create([Bind(Include = "ID,Name,Department,CourseNumber,CreditHours")] BaseCourse baseCourse)
        {
            if (ModelState.IsValid)
            {
                db.BaseCourses.Add(baseCourse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(baseCourse);
        }

        // GET: BaseCourses/Edit/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaseCourse baseCourse = db.BaseCourses.Find(id);
            if (baseCourse == null)
            {
                return HttpNotFound();
            }
            return View(baseCourse);
        }

        // POST: BaseCourses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Edit([Bind(Include = "ID,Name,Department,CourseNumber,CreditHours")] BaseCourse baseCourse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(baseCourse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(baseCourse);
        }

        // GET: BaseCourses/Delete/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            BaseCourse baseCourse = db.BaseCourses.Find(id);
            if (baseCourse == null)
            {
                return HttpNotFound();
            }
            return View(baseCourse);
        }

        // POST: BaseCourses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult DeleteConfirmed(int id)
        {
            BaseCourse baseCourse = db.BaseCourses.Find(id);
            db.BaseCourses.Remove(baseCourse);
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
