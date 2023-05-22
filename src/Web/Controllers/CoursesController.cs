using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Framework;
using Web.Models.Courses;
using Framework.Services;
using Microsoft.AspNetCore.Authorization;
using Data.Models;

namespace Web.Controllers;

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
        var _courses = _courseService.GetCourses();

        return View(Mapper.Map<List<CourseViewModel>>(_courses));
    }

    [HttpGet]
    [Authorize(Roles = "ProblemAuthor")]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "ProblemAuthor")]
    [ValidateAntiForgeryToken]
    public IActionResult Create(CreateCourseViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var _course = Mapper.Map<Course>(model);

        _courseService.CreateCourse(_course);

        AddSuccess("Course created successfully!");

        return RedirectToAction(nameof(Index));
    }
}
