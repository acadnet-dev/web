using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data;
using Acadnet.Data.Models;

namespace Acadnet.Framework.Services
{
    public class CourseService : ICourseService
    {
        private readonly Database _database;

        public CourseService(Database database)
        {
            _database = database;
        }

        public ICollection<Course> GetCourses()
        {
            return _database.Courses.ToList();
        }
    }
}