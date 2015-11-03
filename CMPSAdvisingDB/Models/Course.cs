using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMPSAdvisingDB
{
    public partial class Course
    {
        public String toLetter (int numberGrade)
        {
            String letterGrade = "";
            if (numberGrade == 0)
            {
                letterGrade = "F";
            }
            else if (numberGrade == 1)
            {
                letterGrade = "D";
            }
            else if (numberGrade == 2)
            {
                letterGrade = "C";
            }
            else if (numberGrade == 3)
            {
                letterGrade = "B";
            }
            else if (numberGrade == 4)
            {
                letterGrade = "A";
            }

            return letterGrade;
        }
    }
}
