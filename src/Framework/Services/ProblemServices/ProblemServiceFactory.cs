using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using Data.Models.Enums;

namespace Framework.Services.ProblemServices
{
    public class ProblemServiceFactory
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Database _database;

        public ProblemServiceFactory(
            IServiceProvider serviceProvider,
            Database database
        )
        {
            _serviceProvider = serviceProvider;
            _database = database;
        }

        /// <summary>
        /// Gets the service for the specified problem type
        /// </summary>
        /// <param name="problemType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public IProblemService GetServiceByType(ProblemType problemType)
        {
            switch (problemType)
            {
                case ProblemType.SimpleAcadnetIS:
                    return (IProblemService)_serviceProvider.GetService(typeof(SimpleAcadnetISProblemService))!;
                default:
                    throw new ArgumentOutOfRangeException(nameof(problemType), problemType, null);
            }
        }

        public IProblemService GetServiceById(int id)
        {
            var problem = _database.Problems.Find(id);

            if (problem == null)
            {
                throw new Exception($"Problem with id {id} not found");
            }

            return GetServiceByType(problem.Type);
        }
    }
}