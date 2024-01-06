using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ConfirmedApplicationDTO
    {
        [Required(ErrorMessage = "Application's Id must not empty.")]
        public Guid ApplicationId { get; set; }

        [Required(ErrorMessage = "Application's status must not empty.")]
        public ApplicationStatus Status { get; set; }
    }
}
