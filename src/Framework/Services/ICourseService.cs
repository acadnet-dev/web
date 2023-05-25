using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models;

namespace Framework.Services
{
    public interface ICourseService
    {
        public ICollection<Course> GetCourses(string? filterMaintainer = default!);
        public Course? GetCourse(int courseId);
        public Course? GetCourseByCategory(int categoryId);
        public void CreateCourse(Course course);
        public ICollection<Category> GetCategories(Course course, int? categoryParentId = default!);
        public Category? GetCategory(int categoryId);
        public bool IsMaintainer(int courseId, User user);
        public void CreateCategory(Course course, Category category, int? parentCategoryId = default!);
    }
}