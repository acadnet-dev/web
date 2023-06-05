using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Problem
{
    public class SolveProblemViewModel
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string StatementHtml { get; set; } = default!;
        public bool IsSolved { get; set; } = default!;
    }
}