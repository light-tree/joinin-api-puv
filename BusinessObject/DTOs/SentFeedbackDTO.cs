using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class SentFeedbackDTO
    {
        [Required(ErrorMessage = "Id of feedbacked member must not be null.")]
        public Guid MemberId { get; set; }

        [Required(ErrorMessage = "Group's Id must not be null.")]
        public Guid GroupId { get; set; }

        [Required(ErrorMessage = "Content must not be null.")]
        public string Content { get; set; }

        [Required(ErrorMessage = "Rating must not be null.")]
        [Range(0.5, 5, ErrorMessage = "Rating must between {1} and {2}.")]
        public float Rating { get; set; }
    }
}
