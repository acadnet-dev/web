using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;
using Data.Models.Enums;

namespace Data.Models
{
    public class Submission : UuidEntity
    {
        public string Json { get; set; } = "{}";

        [Column(TypeName = "varchar(255)")]
        public SubmissionStatus Status { get; set; } = default!;

        public Problem Problem { get; set; } = default!;

        public User User { get; set; } = default!;
    }
}