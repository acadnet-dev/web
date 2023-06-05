using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Web.Models.Problem
{
    public class EditProblemViewModel
    {
        public int Id { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string FilesBucketName { get; set; } = default!;
        public ICollection<string> Files { get; set; } = default!;
    }

    public class CheckStructureViewModel
    {
        public bool ReadmePresent { get; set; } = default!;
        public string? ReadmeError { get; set; }

        public bool BadSourcePresent { get; set; } = default!;
        public string? BadSourceError { get; set; }

        public bool GoodSourcePresent { get; set; } = default!;
        public string? GoodSourceError { get; set; }

        public int InputTestsCount { get; set; } = default!;
        public string? InputTestsError { get; set; }

        public int RefTestsCount { get; set; } = default!;
        public string? RefTestsError { get; set; }

        public bool SolutionOk { get; set; } = default!;
    }
}