using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Acadnet.Data.Models;

namespace Acadnet.Framework.Services
{
    public interface ICourseService
    {
        public ICollection<Course> GetCourses();
    }
}