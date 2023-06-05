using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data.Models.Enums;

namespace Data.Models
{
    [Table("Problems")]
    public class Problem : Entity
    {
        public string Name { get; set; } = default!;

        public virtual Category Category { get; set; } = default!;

        [Column(TypeName = "varchar(255)")]
        public ProblemStatus Status { get; set; } = ProblemStatus.Incomplete;

        public string FilesBucketName { get; set; } = default!;

        [Column(TypeName = "varchar(255)")]
        public ProblemType Type { get; set; } = default!;

        public ICollection<Submission> Submissions { get; set; } = default!;

        public Submission? SolutionSubmission { get; set; } = default!;
    }
}