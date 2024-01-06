using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupTaskRecordDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string StartDateDeadline { get; set; }

        public string EndDateDeadline { get; set; }

        public string ImpotantLevel { get; set; }

        public int EstimatedDays { get; set; }

        public string Status { get; set; }

        public UserDTOForTaskList CreatedBy { get; set; }

        public List<UserDTOForTaskList> AssignedFor { get; set; }
    }
}
