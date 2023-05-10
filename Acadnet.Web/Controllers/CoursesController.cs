using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Acadnet.Web.Models;
using Acadnet.Framework;

namespace Acadnet.Web.Controllers;

public class CoursesController : AcadnetController
{
    private readonly ILogger<HomeController> _logger;

    public CoursesController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }
}
