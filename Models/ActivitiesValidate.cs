using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Belt1.Models
{
    public class ActivitiesValidate
    {
        [MinLength(2)]
        public string title { get; set; }
        [Required(ErrorMessage = "A description for the activity is required")]
        [MinLength(10)]
        public string description { get; set; }
        // [Required(ErrorMessage = "A date for the activity is required")]
        // public DateTime date { get; set; }
        // [Required(ErrorMessage = "A time for the activity is required")]
        // public int time { get; set; }
    }
}