using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupDTOForCreating
    {
        [Required(ErrorMessage = "Group's name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Group's name must be between {2} and {1} characters long.")]
        public string Name { get; set; }

        [StringLength(100, ErrorMessage = "School's name's length must be {1} at most.")]
        public string? SchoolName { get; set; }

        [StringLength(100, ErrorMessage = "Class's name's length must be {1} at most.")]
        public string? ClassName { get; set; }

        [StringLength(100, ErrorMessage = "Subject's name's length must be {1} at most.")]
        public string? SubjectName { get; set; }

        public string? Description { get; set; }

        public string? Skill { get; set; }

        [StringLength(200, ErrorMessage = "Avatar link's length must be {1} at most.")]
        public string? Avatar { get; set; }

        [StringLength(200, ErrorMessage = "Theme link's length must be {1} at most.")]
        public string? Theme { get; set; }
    }
}
