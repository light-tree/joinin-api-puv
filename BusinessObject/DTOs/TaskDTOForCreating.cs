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
    public class TaskDTOForCreating
    {
        [Required(ErrorMessage = "Task's name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Task's name must be between {2} and {1} characters long.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Start date deadline is required.")]
        public DateTime StartDateDeadline { get; set; }

        [Required(ErrorMessage = "End date deadline is required.")]
        public DateTime EndDateDeadline { get; set; }

        [Required(ErrorMessage = "Important level is required.")]
        public ImportantLevel ImpotantLevel { get; set; }

        [Required(ErrorMessage = "Estimated date is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Estimated days must be a minimum of {1}.")]
        public int EstimatedDays { get; set; }

        public string? Description { get; set; }

        public Guid? MainTaskId { get; set; }

        [Required(ErrorMessage = "Group Id is required.")]
        public Guid GroupId { get; set; }
    }
}
