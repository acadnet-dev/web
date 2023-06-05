using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models;
using Data.S3;

namespace Framework.Services
{
    public interface ICheckerService
    {
        Task<Submission> CreateSubmissionAsync(S3Object submission, Problem problem, User user);
        Task<Submission> GetSubmissionAsync(string uuid);
    }
}