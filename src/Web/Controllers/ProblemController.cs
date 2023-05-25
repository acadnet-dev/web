using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Framework;
using Web.Models.Problem;
using Framework.Services;
using Data.Models;
using Data.Models.Enums;

namespace Web.Controllers;

public class ProblemController : AcadnetController
{
    private readonly IProblemService _problemService;
    private readonly ICourseService _categoryService;

    public ProblemController(
        IProblemService problemService,
        ICourseService categoryService
    )
    {
        _problemService = problemService;
        _categoryService = categoryService;
    }

    [HttpGet]
    public IActionResult Create([FromQuery] int? categoryId)
    {
        if (categoryId == null)
        {
            AddError("Category not found!");
            return RedirectToAction("Index", "Course");
        }

        return View(new CreateProblemViewModel { CategoryId = categoryId });
    }

    [HttpPost]
    public IActionResult Create(CreateProblemViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var _category = _categoryService.GetCategory(model.CategoryId!.Value);

        if (_category == null)
        {
            AddError("Category not found!");
            return RedirectToAction("Index", "Course");
        }

        var _problem = new Problem
        {
            Name = model.Name,
            // problems are incomplete after creation, because they don't have any files
            Status = ProblemStatus.Incomplete
        };

        _problemService.CreateProblem(_problem, _category);

        // get course to know where to return
        var _course = _categoryService.GetCourseByCategory(_category.Id);

        return RedirectToAction("Categories", "Course", new { courseId = _course!.Id, categoryParent = _category.Id });
    }
}
