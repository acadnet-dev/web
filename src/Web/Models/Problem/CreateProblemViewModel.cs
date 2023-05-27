using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Data.Models.Enums;

namespace Web.Models.Problem
{
    public class CreateProblemViewModel
    {
        [Required]
        [Display(Name = "Problem Name")]
        public string Name { get; set; } = default!;

        [Required]
        [HiddenInput]
        public int? CategoryId { get; set; } = default!;

        [Required]
        [Display(Name = "Problem Type")]
        public ProblemType ProblemType { get; set; } = default!;
    }
}