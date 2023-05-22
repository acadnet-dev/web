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

    public IActionResult Index([FromQuery] string? filterMaintainer = default!)
    {
        var _courses = _courseService.GetCourses(filterMaintainer);

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

    [HttpGet]
    public IActionResult Categories([FromQuery] int courseId, [FromQuery] int? categoryParent = default!)
    {
        var _course = _courseService.GetCourse(courseId);

        if (_course == null)
        {
            return NotFound();
        }

        var _categories = _courseService.GetCategories(_course, categoryParent);

        return View(new CategoriesViewModel
        {
            CourseId = _course.Id,
            CourseName = _course.Name,
            Categories = Mapper.Map<List<CategoryViewModel>>(_categories),
            CategoryParent = categoryParent
        });
    }

    [HttpGet]
    public IActionResult CreateCategory([FromQuery] int courseId, [FromQuery] int? categoryParent = default!)
    {
        var _course = _courseService.GetCourse(courseId);

        if (_course == null)
        {
            return NotFound();
        }

        var _categories = _courseService.GetCategories(_course, categoryParent);

        return View(new CreateCategoryViewModel
        {
            CourseId = _course.Id,
            CourseName = _course.Name,
            CategoryParentId = categoryParent
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "ProblemAuthor")]
    public IActionResult CreateCategory(CreateCategoryViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var _course = _courseService.GetCourse(model.CourseId);

        if (_course == null)
        {
            return NotFound();
        }

        var _category = new Category { Name = model.Name };

        _courseService.CreateCategory(_course, _category);

        AddSuccess("Category created successfully!");

        return RedirectToAction(nameof(Categories), new { courseId = model.CourseId, categoryParent = model.CategoryParentId });
    }
}
