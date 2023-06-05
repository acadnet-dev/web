using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Models
{
    public class SubmissionError
    {
        public string Type { get; set; } = default!;
        public string Message { get; set; } = default!;
    }
}