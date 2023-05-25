using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models;

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
    }
}