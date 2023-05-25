using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;

namespace Framework.Services
{
    public interface IProblemService
    {
        public void CreateProblem(Problem problem, Category category);
        public Problem? GetProblem(int id);
    }
}