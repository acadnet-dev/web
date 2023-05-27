using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.S3;

namespace Framework.Services
{
    public interface IProblemService
    {
        public void CreateProblem(Problem problem);
        public Problem? GetProblem(int id);
        public string GetProblemStatementHtml(Problem problem);
        public S3Object GetProblemSource(Problem problem);
    }
}