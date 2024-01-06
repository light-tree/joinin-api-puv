using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class AssignedTasksDTO
    {
        [Required(ErrorMessage = "Task's Id is required.")]
        public Guid TaskId { get; set; }

        [Required(ErrorMessage = "Assignee list must not null.")]
        public List<Guid> AssignedForIds { get; set; }
    }
}
