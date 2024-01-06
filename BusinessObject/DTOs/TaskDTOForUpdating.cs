using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class TaskDTOForUpdating
    {
        [Required(ErrorMessage = "Id is required.")]
        public Guid Id { get; set; }

        [StringLength(100, MinimumLength = 3, ErrorMessage = "Task's name must be between {2} and {1} characters long.")]
        public string? Name { get; set; }

        public DateTime? StartDateDeadline { get; set; }

        public DateTime? EndDateDeadline { get; set; }

        public ImportantLevel? ImpotantLevel { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Estimated days must be a minimum of {1}.")]
        public int? EstimatedDays { get; set; }

        public string? Description { get; set; }

        public Enums.TaskStatus? Status { get; set; }
    }
}
