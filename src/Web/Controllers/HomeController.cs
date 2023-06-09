﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Framework;

namespace Web.Controllers;

public class HomeController : AcadnetController
{
    public HomeController()
    {
    }

    public IActionResult Index()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
