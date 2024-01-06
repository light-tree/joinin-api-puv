using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupMajorDTO
    {
        public Guid MajorId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Number of member needed must be at least 1.")]
        public int MemberCount { get; set; }
    }
}
