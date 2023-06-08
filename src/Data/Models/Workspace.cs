using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;

namespace Data.Models
{
    public class Workspace : UuidEntity
    {
        public User User { get; set; } = default!;
        public Problem Problem { get; set; } = default!;
    }
}