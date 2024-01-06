using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SentApplicationDTO
    {
        [StringLength(1000, ErrorMessage = "Description's length must be {1} at most.")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Group Id must not be empty.")]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "The MajorIds list must have at least one element.")]
        [MinLength(1)]
        public List<Guid> MajorIds { get; set; }
    }
}
