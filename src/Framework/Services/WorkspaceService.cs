using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Data;
using Data.Identity;
using Data.Models;
using Data.Settings;
using Framework.Services.ProblemServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Framework.Services
{
    public class WorkspaceService : IWorkspaceService
    {
        private readonly Database _database;
        private static readonly HttpClient client = new HttpClient();
        private readonly IOptions<WorkspacesSettings> _workspacesSettings;
        private readonly ProblemServiceFactory _problemServiceFactory;

        public WorkspaceService(
            Database database,
            IOptions<WorkspacesSettings> workspacesSettings,
            ProblemServiceFactory problemServiceFactory
        )
        {
            _database = database;
            _workspacesSettings = workspacesSettings;
            _problemServiceFactory = problemServiceFactory;
        }

        /// <summary>
        /// Retrieves or creates a workspace for a user and a problem
        /// </summary>
        /// <param name="problem"></param>
        /// <param name="user"></param>
        /// <returns>Workspace id</returns>
        public string GetWorkspaceId(Problem problem, User user)
        {
            var _workspace = _database.Workspaces.FirstOrDefault(x => x.Problem == problem && x.User == user);

            if (_workspace != null)
                return _workspace.Id;

            _workspace = new Workspace
            {
                Problem = problem,
                User = user
            };

            _database.Workspaces.Add(_workspace);
            _database.SaveChanges();

            return _workspace.Id;
        }

        public async Task<string> GetWorkspaceUrlAsync(string workspaceId)
        {
            // get workspace endpoint if it exists
            var builder = new UriBuilder(_workspacesSettings.Value.Endpoint + "/workspace/get");

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["id"] = workspaceId;
            builder.Query = query.ToString();

            string url = builder.ToString();

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic parsedJson = JsonConvert.DeserializeObject(responseString)!;

                return parsedJson["endpoint"];
            }

            // create workspace, because it doesn't exist
            var workspace = _database.Workspaces.Include(x => x.Problem).FirstOrDefault(x => x.Id == workspaceId);
            if (workspace == null)
                throw new Exception("Workspace doesn't exist");

            var problemService = _problemServiceFactory.GetServiceByType(workspace.Problem.Type);

            var files = problemService.GetWorkspaceFiles(workspace.Problem);

            builder = new UriBuilder(_workspacesSettings.Value.Endpoint + "/workspace/create");

            query = HttpUtility.ParseQueryString(string.Empty);
            query["id"] = workspaceId;
            query["problem_name"] = workspace.Problem.Name;
            builder.Query = query.ToString();

            url = builder.ToString();

            using (var multipartFormContent = new MultipartFormDataContent())
            {
                foreach (var file in files)
                {
                    multipartFormContent.Add(new StreamContent(file.Content), "files", file.FileName);
                }
                response = await client.PostAsync(
                    url,
                    multipartFormContent
                );
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic parsedJson = JsonConvert.DeserializeObject(responseString)!;

                return parsedJson["endpoint"];
            }
        }

        public string GetWorkspaceProblemPath(string workspaceId)
        {
            var workspace = _database.Workspaces.Include(x => x.Problem).FirstOrDefault(x => x.Id == workspaceId);
            if (workspace == null)
                throw new Exception("Workspace doesn't exist");


            return "/home/workspace/" + workspace.Problem.Name;
        }
    }
}