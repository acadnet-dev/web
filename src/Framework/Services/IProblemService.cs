using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Models;
using Data.Models.Enums;
using Data.S3;

namespace Framework.Services
{
    public interface IProblemService
    {
        public void CreateProblem(Problem problem);
        public Problem? GetProblem(int id);
        public string GetProblemStatementHtml(Problem problem);
        public S3Object GetProblemSource(Problem problem);
        public S3Object GetProblemSolution(Problem problem);
        public void AddSolutionSubmission(Problem problem, Submission submission);
        public void MakeProblemReady(Problem problem);

        // submission management
        public SubmissionStatus GetSubmissionStatus(Submission submission);
        public ICollection<SubmissionError> GetSubmissionErrors(Submission submission);
    }
}