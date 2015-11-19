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
    [Authorize(Roles ="Admin,Professor")]
    public class ConcentrationsController : Controller
    {
        private CMPSAdvising1Entities db = new CMPSAdvising1Entities();

        // GET: Concentrations
        public ActionResult Index()
        {
            return View(db.Concentrations.ToList());
        }

        // GET: Concentrations/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concentration concentration = db.Concentrations.Find(id);
            if (concentration == null)
            {
                return HttpNotFound();
            }

            List<BaseCourse> RequiredCourses = concentration.RequiredCourses.ToList();

            return View(concentration);
        }

        // GET: Concentrations/Create
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Concentrations/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create([Bind(Include = "ID,Name,HoursRequired")] Concentration concentration)
        {
            if (ModelState.IsValid)
            {
                db.Concentrations.Add(concentration);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(concentration);
        }

        // GET: Concentrations/Edit/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concentration concentration = db.Concentrations.Find(id);
            if (concentration == null)
            {
                return HttpNotFound();
            }
            return View(concentration);
        }

        // POST: Concentrations/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Edit([Bind(Include = "ID,Name,HoursRequired")] Concentration concentration)
        {
            if (ModelState.IsValid)
            {
                db.Entry(concentration).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(concentration);
        }

        // GET: Concentrations/Delete/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Concentration concentration = db.Concentrations.Find(id);
            if (concentration == null)
            {
                return HttpNotFound();
            }
            return View(concentration);
        }

        // POST: Concentrations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult DeleteConfirmed(int id)
        {
            Concentration concentration = db.Concentrations.Find(id);
            db.Concentrations.Remove(concentration);
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
