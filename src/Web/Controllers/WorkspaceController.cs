using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Framework;
using System.Web.Mvc;
using Framework.Services;
using Data;
using Data.Models;
using System.Web;

namespace Web.Controllers;

[Authorize]
public class WorkspaceController : AcadnetController
{
    private readonly IWorkspaceService _workspaceService;
    private readonly Database _database;

    public WorkspaceController(
        IWorkspaceService workspaceService,
        Database database
    )
    {
        _workspaceService = workspaceService;
        _database = database;
    }

    public IActionResult LaunchWorkspace([FromQuery] int? problemId)
    {
        if (problemId == null)
        {
            return BadRequest();
        }

        var problem = _database.Problems.Find(problemId);

        if (problem == null)
        {
            return NotFound();
        }

        var workspaceId = _workspaceService.GetWorkspaceId(problem, SecurityContext.User!);

        // set cookie
        Response.Cookies.Append(Consts.WORKSPACE_ID_COOKIE_NAME, workspaceId);

        return Redirect("/code?folder=" + _workspaceService.GetWorkspaceProblemPath(workspaceId));
    }
}
