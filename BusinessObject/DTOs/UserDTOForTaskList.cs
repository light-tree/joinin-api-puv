using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class UserDTOForTaskList
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string FullName { get; set; }

        public string? Avatar { get; set; }
    }
}
