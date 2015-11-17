using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CMPSAdvisingDB;
using CMPSAdvisingDB.Models;
using CMPSAdvisingDB.ViewModels;

namespace CMPSAdvisingDB.Controllers
{
    // [Authorize]
    public class StudentsController : Controller
    {
       
        private CMPSAdvising1Entities db = new CMPSAdvising1Entities();

        public ActionResult ResetRecomendations (int? id)
        {
            Student student = db.Students.Find(id);
            db.Courses.RemoveRange(student.CoursesRecommended);
            student.CoursesRecommended.Clear();
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = student.ID });
        }

        public ActionResult ResetTaken (int? id)
        {
            Student student = db.Students.Find(id);
            db.Courses.RemoveRange(student.CoursesTaken);
            student.CoursesTaken.Clear();
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Details", new { id = student.ID });
        }

        public ActionResult RecommendCourses(int? id)
        {
            Student student = db.Students.Find(id);
            Concentration conc = student.Concentration;
            List<Course> possibleRecommendations = new List<Course>();
            foreach (BaseCourse b in conc.RequiredCourses)
            {
                Course c = new Course();
                c.BaseCourse = b;
                possibleRecommendations.Add(c);
            }

            ICollection<Course> recCollection = possibleRecommendations.Where(p => !student.CoursesTaken.Any(p2 => p2.BaseCourse.Name == p.BaseCourse.Name)).ToList();
            List<Course> toRemove = new List<Course>();

            foreach (Course c in recCollection)
            {
               bool hasIt = false;
               foreach (Course c2 in student.CoursesTaken)
               {
                   String pName = c2.BaseCourse.Department + c2.BaseCourse.CourseNumber;
                   foreach (Prerequisite p in c.BaseCourse.Prerequisites)
                   {
                       if (p.PrereqName == pName)
                       {
                           hasIt = true;
                       }
                   }
               }
                if (c.BaseCourse.Prerequisites.Count() == 0)
                {
                    hasIt = true;
                }

                if (!hasIt)
                {
                    toRemove.Add(c);
                }
            }

            foreach(Course c in toRemove)
            {
                recCollection.Remove(c);
            }

            student.CoursesRecommended = recCollection;
            
            db.Entry(student).State = EntityState.Modified;
            db.SaveChanges();

            return View("RecommendCourses", student);
        }

        public ActionResult AddCourseVM(int? id)
        {
            Student student = db.Students.Find(id);
            List<Course> alreadyTaken = student.CoursesTaken.ToList();
            Concentration conc = db.Concentrations.Find(student.Concentration.ID);
            List<BaseCourse> potentialCourses = conc.RequiredCourses.ToList();
            AddCourseViewModel vModel = new AddCourseViewModel(student, potentialCourses);

            List<Course> listCourses = new List<Course>();

            foreach (BaseCourse baseC in potentialCourses)
            {
                Course c = new Course();
                c.BaseCourse = baseC;
                listCourses.Add(c);
            }

            foreach (Course c in alreadyTaken)
            {
                listCourses.RemoveAll(s => s.BaseCourse.ID == c.BaseCourseID);
            }

            vModel.PossibleCourses = listCourses;
            
            for (int i = 0; i < listCourses.Count; i++)
            {
                var selectList = new SelectList(db.Semesters, "Sname", "Sname", "1");
                this.ViewData[string.Format("PossibleCourses[{0}].Semester", i)] = selectList;

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem { Text = "A", Value = "4" });
                items.Add(new SelectListItem { Text = "B", Value = "3" });
                items.Add(new SelectListItem { Text = "C", Value = "2" });
                items.Add(new SelectListItem { Text = "D", Value = "1" });
                items.Add(new SelectListItem { Text = "F", Value = "0" });
                this.ViewData[string.Format("PossibleCourses[{0}].Grade", i)] = items;
                this.ViewData[string.Format("selected[{0}]", i)] = 1;
            }

            return View("AddCourseVM", vModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCourseVM(AddCourseViewModel vModel)
        {
            Student stu = db.Students.Find(vModel.Student.ID);
            foreach (Course c in vModel.PossibleCourses)
            {
                if (c.Selected)
                {
                    BaseCourse bc = db.BaseCourses.Find(c.BaseCourse.ID);
                    c.BaseCourse = bc;
                    ViewData["grade"] = new SelectList(vModel.PossibleCourses); 
                    stu.CoursesTaken.Add(c);
                    
                    db.Entry(c).State = EntityState.Added;
                }
            }

            if (stu != null)
            {
                db.Entry(stu).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("ListTakenCourses", new { id=stu.ID});
        }

        public ActionResult ListTakenCourses(int? id)
        {
            Student stu = db.Students.Find(id);
            List<Course> taken = stu.CoursesTaken.ToList();
            ViewBag.CoursesTaken = taken;

            return View(stu);
        }

        public ActionResult ClearCourses(int? id)
        {
            Student stu = db.Students.Find(id);
            int count = stu.CoursesTaken.Count;
            for (int i = 0; i < count; i++)
            {
                Course c = stu.CoursesTaken.First();
                stu.CoursesTaken.Remove(c);
                db.Courses.Remove(c);
            }
            db.Entry(stu).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Students
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Index()
        {
            var students = db.Students.Include(s => s.Concentration);
            return View(students.ToList());
        }

        // GET: Students/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            List<Course> mathCourses = student.CoursesTaken.Where(c => c.BaseCourse.Department == "MATH").ToList();
            List<Course> cmpsCourses = student.CoursesTaken.Where(c => c.BaseCourse.Department == "CMPS").ToList();
            List<Course> sciCourses = student.CoursesTaken.Where(c => (c.BaseCourse.Department == "PHYS" || c.BaseCourse.Department == "CHEM" || c.BaseCourse.Department == "GBIO")).ToList();
            List<Course> englishCourses = student.CoursesTaken.Where(c => (c.BaseCourse.Department == "ENGL")).ToList();
            ViewData["mathCourses"] = mathCourses;
            ViewData["cmpsCourses"] = cmpsCourses;
            ViewData["scienceCourses"] = sciCourses;
            ViewData["englishCourses"] = englishCourses;

            return View(student);
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create()
        {
            ViewBag.StudentConcentration_ID = new SelectList(db.Concentrations, "ID", "Name");
            return View();
        }

        // POST: Students/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Create([Bind(Include = "ID,FirstName,LastName,WNumber,HoursCompleted,GPA,StudentConcentration_ID")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Students.Add(student);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.StudentConcentration_ID = new SelectList(db.Concentrations, "ID", "Name", student.StudentConcentration_ID);
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Student,Admin,Professor")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            ViewBag.StudentConcentration_ID = new SelectList(db.Concentrations, "ID", "Name", student.StudentConcentration_ID);
            ViewBag.Professor_ID = new SelectList(db.Professors, "ID", "LastName", student.Professor_ID);
            return View(student);
        }

        // POST: Students/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Student,Admin,Professor")]
        public ActionResult Edit([Bind(Include = "ID,FirstName,LastName,WNumber,HoursCompleted,GPA,StudentConcentration_ID,Professor_ID")] Student student)
        {
            if (ModelState.IsValid)
            {
                db.Entry(student).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.StudentConcentration_ID = new SelectList(db.Concentrations, "ID", "Name", student.StudentConcentration_ID);
            ViewBag.Professors_ID = new SelectList(db.Professors, "ID", "LastName", student.Professor_ID);
            return View(student);
        }

        // GET: Students/Delete/5
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Professor")]
        public ActionResult DeleteConfirmed(int id)
        {
            Student student = db.Students.Find(id);
            db.Students.Remove(student);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult GeneratePDF(int? id)
        {
            return new Rotativa.ActionAsPdf("PDFDetails", new { id = id });
        }

        public ActionResult PDFDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Student student = db.Students.Find(id);
            if (student == null)
            {
                return HttpNotFound();
            }
            List<Course> mathCourses = student.CoursesTaken.Where(c => c.BaseCourse.Department == "MATH").ToList();
            List<Course> cmpsCourses = student.CoursesTaken.Where(c => c.BaseCourse.Department == "CMPS").ToList();
            List<Course> sciCourses = student.CoursesTaken.Where(c => (c.BaseCourse.Department == "PHYS" || c.BaseCourse.Department == "CHEM" || c.BaseCourse.Department == "GBIO")).ToList();
            List<Course> englishCourses = student.CoursesTaken.Where(c => (c.BaseCourse.Department == "ENGL")).ToList();
            ViewData["mathCourses"] = mathCourses;
            ViewData["cmpsCourses"] = cmpsCourses;
            ViewData["scienceCourses"] = sciCourses;
            ViewData["englishCourses"] = englishCourses;

            return View(student);

        }

        public ActionResult MasterList()
        {
            return new Rotativa.ActionAsPdf("GenerateMasterList");
        }

        public ActionResult GenerateMasterList()
        {
            var students = db.Students.Include(s => s.Concentration);
            return View(students.ToList());
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
