using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Data.Models
{
    [Table("Problems")]
    public class Problem : Entity
    {
        public string Name { get; set; } = default!;
        public virtual Category Category { get; set; } = default!;
    }
}