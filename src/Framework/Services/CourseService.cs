using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Identity;
using Data.Models;
using Framework.Security;
using Microsoft.EntityFrameworkCore;

namespace Framework.Services
{
    public class CourseService : ICourseService
    {
        private readonly Database _database;
        private readonly ISecurityContext _securityContext;

        public CourseService(
            Database database,
            ISecurityContext securityContext
        )
        {
            _database = database;
            _securityContext = securityContext;
        }

        public void CreateCategory(Course course, Category category, int? parentCategoryId = null)
        {
            category.Course = course;
            category.Parent = _database.Categories.Find(parentCategoryId);

            _database.Categories.Add(category);

            _database.SaveChanges();
        }

        public void CreateCourse(Course course)
        {
            course.Maintainers.Add(_securityContext.User!);

            _database.Courses.Add(course);

            _database.SaveChanges();
        }

        public ICollection<Category> GetCategories(Course course, int? categoryParentId = null)
        {
            return _database.Categories.AsQueryable()
                .Where(c => c.Course == course)
                .Where(c => c.Parent!.Id == categoryParentId).ToList();
        }

        public Category? GetCategory(int categoryId)
        {
            return _database.Categories.Include(x => x.Problems).FirstOrDefault(x => x.Id == categoryId);
        }

        public Course? GetCourse(int courseId)
        {
            return _database.Courses.Find(courseId);
        }

        public Course? GetCourseByCategory(int categoryId)
        {
            return _database.Categories.AsQueryable()
                .Where(c => c.Id == categoryId)
                .Select(c => c.Course)
                .FirstOrDefault();
        }

        public ICollection<Course> GetCourses(string? filterMaintainer = default!)
        {
            var _query = _database.Courses.AsQueryable();

            if (filterMaintainer != null)
            {
                _query = _query.Where(c => c.Maintainers.Any(m => m.UserName == filterMaintainer));
            }

            return _query.ToList();
        }

        public bool IsMaintainer(int courseId, User user)
        {
            return _database.Courses.AsQueryable()
                .Where(c => c.Id == courseId)
                .Any(c => c.Maintainers.Any(m => m.Id == user.Id));
        }
    }
}