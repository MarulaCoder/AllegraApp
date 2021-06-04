using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AllegraApi.Models
{
    public class UserResponseModel
    {
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}
