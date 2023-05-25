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

        public ProblemService(
            Database database
        )
        {
            _database = database;
        }

        public void CreateProblem(Problem problem, Category category)
        {
            problem.Category = category;

            _database.Problems.Add(problem);

            _database.SaveChanges();
        }
    }
}