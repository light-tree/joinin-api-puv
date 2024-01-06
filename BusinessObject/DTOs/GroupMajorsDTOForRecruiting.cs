using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupMajorsDTOForRecruiting
    {
        [Required(ErrorMessage = "Group's Id is required.")]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "Recruiting list must not be null.")]
        public List<GroupMajorDTO> GroupMajorsDTO { get; set; }
    }
}
