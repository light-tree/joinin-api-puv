using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Models;

namespace BusinessObject.DTOs.User
{
    public class UserProfileResponseDTO
    {

   
        public Guid Id { get; set; }

        public string FullName { get; set; }

        public string? Password { get; set; }

        public string Email { get; set; }

        public string? Phone { get; set; }

        public DateTime BirthDay { get; set; }

        public bool Gender { get; set; }

        public string Description { get; set; }

        public string Skill { get; set; }
        public string? Token { get; set; }

 
        public string? OtherContact { get; set; }


        public string? Avatar { get; set; }


        public string? Theme { get; set; }


        public UserStatus Status { get; set; }

        public bool IsAdmin { get; set; }

        public List<Major> majors { get; set; }
    }
}
