using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Framework.Security;

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

        public void CreateCourse(Course course)
        {
            course.Maintainers.Add(_securityContext.User!);

            _database.Courses.Add(course);

            _database.SaveChanges();
        }

        public ICollection<Course> GetCourses()
        {
            return _database.Courses.ToList();
        }
    }
}