using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;

namespace Framework.Services
{
    public interface ICourseService
    {
        public ICollection<Course> GetCourses();
        public void CreateCourse(Course course);
    }
}