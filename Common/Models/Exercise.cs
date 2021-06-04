using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class Exercise
    {
        public int ExerciseId { get; set; }
        public int UserId { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public DateTime Date { get; set; }
    }
}
