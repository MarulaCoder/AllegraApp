using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class BasicParameters
    {
        public User User { get; set; }
        public Exercise Exercise { get; set; }
        public int UserId { get; set; }

        public int Limit { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
    }
}
