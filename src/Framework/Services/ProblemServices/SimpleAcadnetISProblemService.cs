using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Identity;
using Data.Models;
using Data.Models.Enums;
using Data.S3;
using Framework.Services.FileServices;
using Markdig;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Framework.Services.ProblemServices
{
    public class SimpleAcadnetISProblemService : IProblemService
    {
        private readonly Database _database;
        private readonly IFileService _fileService;

        public SimpleAcadnetISProblemService(
            Database database,
            FileServiceFactory fileServiceFactory
        )
        {
            _database = database;
            _fileService = fileServiceFactory.GetFileService();
        }

        public void AddSolutionSubmission(Problem problem, Submission submission)
        {
            problem.SolutionSubmission = submission;
            problem.Submissions.Add(submission);

            _database.SaveChanges();
        }

        public void AddSubmission(Problem problem, Submission submission)
        {
            problem.Submissions.Add(submission);

            _database.SaveChanges();
        }

        public void CreateProblem(Problem problem)
        {
            problem.FilesBucketName = Guid.NewGuid().ToString();

            _database.Problems.Add(problem);

            _database.SaveChanges();

            // create bucket
            _fileService.CreateBucketAsync(problem.FilesBucketName).Wait();
        }

        public Problem? GetProblem(int id)
        {
            return _database.Problems.Include(x => x.SolutionSubmission).FirstOrDefault(x => x.Id == id);
        }

        public S3Object GetProblemSolution(Problem problem)
        {
            var s3Object = _fileService.DownloadFileAsync(problem.FilesBucketName, "solution_main.cpp").Result;

            if (s3Object == null)
            {
                throw new Exception($"Source not found - bucketName: {problem.FilesBucketName}, fileName: solution_main.cpp");
            }

            return s3Object;
        }

        public S3Object GetProblemSource(Problem problem)
        {
            var s3Object = _fileService.DownloadFileAsync(problem.FilesBucketName, "main.cpp").Result;

            if (s3Object == null)
            {
                throw new Exception($"Source not found - bucketName: {problem.FilesBucketName}, fileName: main.cpp");
            }

            return s3Object;
        }

        public string GetProblemStatementHtml(Problem problem)
        {
            var s3Object = _fileService.DownloadFileAsync(problem.FilesBucketName, "README.md").Result;

            if (s3Object == null)
            {
                throw new Exception($"Statement not found - bucketName: {problem.FilesBucketName}, fileName: README.md");
            }

            string md = (new StreamReader(s3Object.Content)).ReadToEnd();

            return Markdown.ToHtml(md);
        }

        public ICollection<SubmissionError> GetSubmissionErrors(Submission submission)
        {
            dynamic _sub = ParseSubmissionJson(submission.Json);

            // check build status
            if (_sub["build_status"] != "success")
            {
                return new List<SubmissionError> {
                    new SubmissionError {
                        Type = "Compilation error",
                        Message = _sub["build_status"]
                    }
                };
            }

            var errors = new List<SubmissionError>();

            // check test_results
            foreach (var testResult in _sub["test_results"])
            {
                if (testResult["passed"] != true)
                {
                    errors.Add(new SubmissionError
                    {
                        Type = "Test failed",
                        Message = string.Format("Expected\n<code>{0}</code>\nActual\n<code>{1}</code>", testResult["exec_result"]["ref"], testResult["exec_result"]["actual"])
                    });
                }
            }

            return errors;
        }

        public SubmissionStatus GetSubmissionStatus(Submission submission)
        {
            dynamic _sub = ParseSubmissionJson(submission.Json);

            if (_sub["status"] != "finished")
            {
                return SubmissionStatus.Pending;
            }

            var errors = GetSubmissionErrors(submission);

            if (errors.Count > 0)
            {
                return SubmissionStatus.Failed;
            }

            return SubmissionStatus.Passed;
        }

        public ICollection<S3Object> GetWorkspaceFiles(Problem problem)
        {
            // return main.cpp and README.md
            var files = new List<S3Object>();

            var main = _fileService.DownloadFileAsync(problem.FilesBucketName, "main.cpp").Result;
            if (main == null)
                throw new Exception($"Source not found - bucketName: {problem.FilesBucketName}, fileName: main.cpp");

            var readme = _fileService.DownloadFileAsync(problem.FilesBucketName, "README.md").Result;
            if (readme == null)
                throw new Exception($"Source not found - bucketName: {problem.FilesBucketName}, fileName: README.md");

            files.Add(main);
            files.Add(readme);

            return files;
        }

        public bool HasSolvedProblem(Problem problem, User user)
        {
            return _database.Submissions.Any(x => x.Problem == problem && x.User == user && x.Status == SubmissionStatus.Passed);
        }

        public void MakeProblemReady(Problem problem)
        {
            problem.Status = ProblemStatus.Ready;

            _database.SaveChanges();
        }

        private dynamic ParseSubmissionJson(string json)
        {
            return JsonConvert.DeserializeObject(json)!;
        }
    }
}