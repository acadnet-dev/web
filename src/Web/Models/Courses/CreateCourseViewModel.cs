using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Courses
{
    public class CreateCourseViewModel
    {
        [Required]
        [Display(Name = "Course Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; } = default!;

        [Required]
        [Display(Name = "Course Description")]
        [StringLength(500, MinimumLength = 10)]
        public string Description { get; set; } = default!;
    }
}