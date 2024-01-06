using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SentInvitationsDTO
    {
        [StringLength(1000, ErrorMessage = "Description's length must be {1} at most.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Group Id must not empty.")]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "The User's Id list must have at least one element.")]
        [MinLength(1)]
        public List<Guid> UserIds { get; set; }
    }
}
