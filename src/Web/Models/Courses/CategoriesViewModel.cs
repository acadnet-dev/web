using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;

namespace Web.Models.Courses
{
    public class CategoriesViewModel
    {
        public int CourseId { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public int? CategoryParent { get; set; } = default!;

        public ICollection<CategoryViewModel> Categories { get; set; } = default!;
    }

    public class CategoryViewModel
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }
}