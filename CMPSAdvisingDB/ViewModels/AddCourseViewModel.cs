using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMPSAdvisingDB.Models;

namespace CMPSAdvisingDB.ViewModels
{
    public class AddCourseViewModel
    {
        public Student Student { get; set; }
        public List<BaseCourse> AvailCourses { get; set; }
        public List<Course> PossibleCourses { get; set; }

        public AddCourseViewModel(Student s, List<BaseCourse> c)
        {
            Student = s;
            AvailCourses = c;
            PossibleCourses = new List<Course>();
        }

        public AddCourseViewModel()
        {
            Student = new Student();
            AvailCourses = new List<BaseCourse>();
            PossibleCourses = new List<Course>();
        }
    }
}