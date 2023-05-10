using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Acadnet.Web.Models;
using Acadnet.Framework;
using Acadnet.Web.Models.Courses;
using Acadnet.Framework.Services;

namespace Acadnet.Web.Controllers;

public class CoursesController : AcadnetController
{
    private readonly ILogger<HomeController> _logger;
    private readonly ICourseService _courseService;

    public CoursesController(
        ILogger<HomeController> logger,
        ICourseService courseService
    )
    {
        _logger = logger;
        _courseService = courseService;
    }

    public IActionResult Index()
    {
        return View(new CourseListViewModel { Courses = _courseService.GetCourses() });
    }
}
