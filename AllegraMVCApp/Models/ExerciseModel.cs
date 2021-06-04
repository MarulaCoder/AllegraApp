using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllegraApp.Models
{
    public class ExerciseModel
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int Duration { get; set; }

        public DateTime Date { get; set; }
    }
}
