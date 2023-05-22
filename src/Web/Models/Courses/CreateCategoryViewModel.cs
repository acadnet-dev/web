using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Models.Courses
{
    public class CreateCategoryViewModel
    {
        [Required]
        [HiddenInput]
        public int CourseId { get; set; } = default!;

        [Required]
        [HiddenInput]
        public string CourseName { get; set; } = default!;

        [HiddenInput]
        public int? CategoryParentId { get; set; } = default!;

        [Required]
        [Display(Name = "Category Name")]
        public string Name { get; set; } = default!;
    }
}