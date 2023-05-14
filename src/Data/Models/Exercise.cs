using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Models
{
    public class Exercise : Entity
    {
        public string Name { get; set; } = default!;
        public virtual Category Category { get; set; } = default!;
    }
}