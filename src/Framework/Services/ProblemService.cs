using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;
using Data.S3;
using Markdig;

namespace Framework.Services
{
    public class ProblemService : IProblemService
    {
        private readonly Database _database;
        private readonly IFileService _fileService;

        public ProblemService(
            Database database,
            IFileService fileService
        )
        {
            _database = database;
            _fileService = fileService;
        }

        public void CreateProblem(Problem problem, Category category)
        {
            problem.Category = category;
            problem.FilesBucketName = Guid.NewGuid().ToString();

            _database.Problems.Add(problem);

            _database.SaveChanges();

            // create bucket
            _fileService.CreateBucketAsync(problem.FilesBucketName).Wait();
        }

        public Problem? GetProblem(int id)
        {
            return _database.Problems.Find(id);
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
    }
}