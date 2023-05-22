using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data.Identity;

namespace Data.Models
{
    public class Course : Entity
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public virtual ICollection<Category> Categories { get; set; } = new List<Category>();
        public virtual ICollection<User> Maintainers { get; set; } = new List<User>();
    }
}