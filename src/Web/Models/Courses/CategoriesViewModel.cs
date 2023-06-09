using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Enums;

namespace Web.Models.Courses
{
    public class CategoriesViewModel
    {
        public int CourseId { get; set; } = default!;
        public string CourseName { get; set; } = default!;
        public int? CategoryParent { get; set; } = default!;
        public string CategoryName { get; set; } = default!;

        public ICollection<CategoryViewModel> Categories { get; set; } = default!;
        public ICollection<ProblemInCategoryViewModel> Problems { get; set; } = new List<ProblemInCategoryViewModel>();
    }

    public class CategoryViewModel
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

    public class ProblemInCategoryViewModel
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public ProblemStatus Status { get; set; } = ProblemStatus.Hidden;
        public bool IsSolved { get; set; } = default!;
    }
}