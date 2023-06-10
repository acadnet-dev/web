using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models;

namespace Framework.Services
{
    public interface IWorkspaceService
    {
        string GetWorkspaceId(Problem problem, User user);
        Task<string> GetWorkspaceUrlAsync(string workspaceId);
        string GetWorkspaceProblemPath(string workspaceId);
        Problem GetProblem(string workspaceId);
    }
}