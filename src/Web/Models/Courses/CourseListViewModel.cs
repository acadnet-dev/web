using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;

namespace Web.Models.Courses
{
    public class CourseListViewModel
    {
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}