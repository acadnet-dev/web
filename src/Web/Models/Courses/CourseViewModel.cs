using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Courses
{
    public class CourseViewModel
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int ProblemsCount { get; set; } = default!;
    }
}