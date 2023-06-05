using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Data;
using Data.Identity;
using Data.Models;
using Data.S3;
using Data.Settings;
using Framework.Services.ProblemServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Framework.Services
{
    public class CheckerService : ICheckerService
    {
        private readonly Database _database;
        private readonly IOptions<CheckerSettings> _checkerSettings;
        private static readonly HttpClient client = new HttpClient();
        private readonly ProblemServiceFactory _problemServiceFactory;


        public CheckerService(
            Database database,
            IOptions<CheckerSettings> checkerSettings,
            ProblemServiceFactory problemServiceFactory
        )
        {
            _database = database;
            _checkerSettings = checkerSettings;
            _problemServiceFactory = problemServiceFactory;
        }

        public async Task<Submission> CreateSubmissionAsync(S3Object submittedFile, Problem problem, User user)
        {
            var builder = new UriBuilder(_checkerSettings.Value.Endpoint + "/submission/create");

            var query = HttpUtility.ParseQueryString(string.Empty);
            query["type"] = problem.Type.ToString();
            query["bucket"] = problem.FilesBucketName;
            builder.Query = query.ToString();

            string url = builder.ToString();

            using (var multipartFormContent = new MultipartFormDataContent())
            {
                multipartFormContent.Add(new StreamContent(submittedFile.Content), "file", submittedFile.FileName);
                var response = await client.PostAsync(
                    url,
                    multipartFormContent
                );
                var responseString = await response.Content.ReadAsStringAsync();

                dynamic parsedJson = JsonConvert.DeserializeObject(responseString)!;

                var submission = new Submission
                {
                    Id = parsedJson["submission_id"],
                    Problem = problem,
                    User = user,
                };

                _database.Submissions.Add(submission);
                await _database.SaveChangesAsync();

                return submission;
            }
        }

        public async Task<Submission> GetSubmissionAsync(string uuid)
        {
            // get submission from database
            var submission = _database.Submissions.Include(s => s.Problem).FirstOrDefault(s => s.Id == uuid);

            if (submission == null)
            {
                throw new Exception("Submission not found!");
            }

            // get submission status from checker
            var builder = new UriBuilder(_checkerSettings.Value.Endpoint + "/submission/status");
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["submission_id"] = uuid;
            builder.Query = query.ToString();
            string url = builder.ToString();
            var response = await client.GetAsync(url);
            var responseString = await response.Content.ReadAsStringAsync();

            // update submission
            submission.Json = responseString;
            submission.Status = _problemServiceFactory.GetServiceById(submission.Problem.Id).GetSubmissionStatus(submission);
            await _database.SaveChangesAsync();

            return submission;
        }
    }
}