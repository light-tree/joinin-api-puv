using BusinessObject.Enums;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RoleAssignDTO
    {
      
        [Required]
        [Range(0, 2, ErrorMessage = "Role Assign must be Member or SubLeader or Leader")]
        public MemberRole Role { get; set; }
        [Required]
        public Guid GroupId  { get; set; }
        [Required]
        public Guid MemberId { get; set; }
    }
}
