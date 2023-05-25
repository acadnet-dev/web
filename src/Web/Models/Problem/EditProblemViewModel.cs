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
        [Required]
        [HiddenInput]
        public int Id { get; set; } = default!;

        [Required]
        [Display(Name = "Problem Name")]
        public string Name { get; set; } = default!;
    }
}