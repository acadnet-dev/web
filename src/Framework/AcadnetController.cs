using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Framework.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Framework
{
    public class AcadnetController : Controller
    {
        public AcadnetController()
        {
        }

        protected void AddError(string message)
        {
            if (TempData["Error"] == null)
            {
                TempData["Error"] = new List<string>();
            }
            if (TempData["Error"] is List<string>)
                ((List<string>)TempData["Error"]!).Add(message);
            if (TempData["Error"] is string[])
                ((string[])TempData["Error"]!).ToList().Add(message);
        }

        protected void AddSuccess(string message)
        {
            if (TempData["Success"] == null)
            {
                TempData["Success"] = new List<string>();
            }
            if (TempData["Success"] is List<string>)
                ((List<string>)TempData["Success"]!).Add(message);
            if (TempData["Success"] is string[])
                ((string[])TempData["Success"]!).ToList().Add(message);
        }

        protected ISecurityContext SecurityContext => (HttpContext.RequestServices.GetService(typeof(ISecurityContext)) as ISecurityContext)!;
        protected IMapper Mapper => (HttpContext.RequestServices.GetService(typeof(IMapper)) as IMapper)!;
    }
}