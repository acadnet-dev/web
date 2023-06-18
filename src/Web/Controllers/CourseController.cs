using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Framework;
using Web.Models.Courses;
using Framework.Services;
using Microsoft.AspNetCore.Authorization;
using Data.Models;

namespace Web.Controllers;

public class CourseController : AcadnetController
{
    private readonly ICourseService _courseService;

    public CourseController(
        ICourseService courseService
    )
    {
        _courseService = courseService;
    }

    public IActionResult Index([FromQuery] string? filterMaintainer = default!)
    {
        var _courses = _courseService.GetCourses(filterMaintainer);

        var _output = new List<CourseViewModel>();

        foreach (var _course in _courses)
        {
            _output.Add(new CourseViewModel
            {
                Id = _course.Id,
                Name = _course.Name,
                Description = _course.Description,
                ProblemsCount = _courseService.GetProblemsCount(_course)
            });
        }

        return View(_output);
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
        var model = new CategoriesViewModel
        {
            CourseId = _course.Id,
            CourseName = _course.Name,
            Categories = Mapper.Map<List<CategoryViewModel>>(_categories),
            CategoryParent = categoryParent,
        };

        // the category has a name only if it is not the root category
        var categoryName = string.Empty;
        if (categoryParent != null)
        {
            var _categoryParent = _courseService.GetCategory(categoryParent.Value);

            if (_categoryParent == null)
            {
                return NotFound();
            }

            model.CategoryName = _categoryParent.Name;
            model.Problems = Mapper.Map<List<ProblemInCategoryViewModel>>(_categoryParent.Problems);
        }

        return View(model);
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

        _courseService.CreateCategory(_course, _category, model.CategoryParentId);

        AddSuccess("Category created successfully!");

        return RedirectToAction(nameof(Categories), new { courseId = model.CourseId, categoryParent = model.CategoryParentId });
    }
}
