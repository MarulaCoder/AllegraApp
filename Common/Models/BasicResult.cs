using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Models
{
    public class BasicResult
    {
        public bool Successful { get; set; }
        public string Reason { get; set; }
        public bool NotFound { get; set; }
        public bool EntityExists { get; set; }
        public bool ValidationError { get; set; }

        public List<User> UserList { get; set; }
        public List<Exercise> ExerciseList { get; set; }
        public int UserId { get; set; }
        public int ExerciseId { get; set; }

        public BasicResult SetFailure(string reason)
        {
            Successful = false;
            Reason = reason;
            ValidationError = false;
            return this;
        }

        public BasicResult SetSuccess()
        {
            Successful = true;
            Reason = string.Empty;
            ValidationError = false;
            return this;
        }
    }
}
