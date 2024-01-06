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
    public class TaskDetailDTO
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string StartDateDeadline { get; set; }

        public string EndDateDeadline { get; set; }

        public string? FinishedDate { get; set; }

        public string ImpotantLevel { get; set; }

        public int EstimatedDays { get; set; }

        public string? Description { get; set; }

        public string Status { get; set; }

        public Guid? MainTaskId { get; set; }

        public GroupDTOForToDoTaskRecord Group { get; set; }

        public UserDTOForTaskList CreatedBy { get; set; }

        public List<GroupTaskRecordDTO>? SubTasks { get; set; }

        public List<UserDTOForTaskList>? AssignedFor { get; set; }

        public List<CommentRecordDTO>? Comments { get; set; }
    }
}
