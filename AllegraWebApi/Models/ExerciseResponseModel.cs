using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllegraApi.Models
{
    public class ExerciseResponseModel
    {
        public int ExerciseId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime Date { get; set; }
    }
}
