using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Models;
using Web.Models.Courses;
using Web.Models.Problem;

namespace Web.AutoMapper
{
    public class AutoMapperConfigProfile : Profile
    {
        public AutoMapperConfigProfile()
        {
            CoursesConfig();
            ProblemsConfig();
        }

        public void CoursesConfig()
        {
            CreateMap<Course, CourseViewModel>();
            CreateMap<CreateCourseViewModel, Course>();

            CreateMap<Category, CategoryViewModel>();
            CreateMap<CreateCategoryViewModel, Category>();
        }

        public void ProblemsConfig()
        {
            CreateMap<Problem, ProblemInCategoryViewModel>();
            CreateMap<CreateProblemViewModel, Problem>();
        }
    }
}