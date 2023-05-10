using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Acadnet.Data.Models
{
    public class Category : Entity
    {
        public string Name { get; set; } = default!;
        public Course? Course { get; set; }
        public Category? Parent { get; set; }
        public virtual ICollection<Category> Children { get; set; } = new List<Category>();
        public virtual ICollection<Exercise> Exercises { get; set; } = new List<Exercise>();
    }
}