using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BusinessObject.Enums;
using System.Text.Json.Serialization;

namespace BusinessObject.Models
{
    [Table("tasks")]
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("start_date_deadline")]
        public DateTime StartDateDeadline { get; set; }

        [Required]
        [Column("end_date_deadline")]
        public DateTime EndDateDeadline { get; set; }

        [Column("finished_date")]
        public DateTime? FinishedDate { get; set; }

        [Required]
        [Column("important_level")]
        public ImportantLevel ImportantLevel { get; set; }

        [Required]
        [Column("estimated_days")]
        public int EstimatedDays { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("status")]
        public Enums.TaskStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(Group))]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Required]
        [ForeignKey(nameof(CreatedBy))]
        [Column("created_by_id")]
        public Guid CreatedById { get; set; }

        [ForeignKey(nameof(MainTask))]
        [Column("main_task_id")]
        public Guid? MainTaskId { get; set; }

        public Group Group { get; set; }

        public Member CreatedBy { get; set; }

        public Task? MainTask { get; set; }

        [InverseProperty(nameof(MainTask))]
        public List<Task> SubTasks { get; set;}

        public List<AssignedTask> AssignedTasks { get; set; }
        [JsonIgnore]
        public List<Comment> Comments { get; set; }
    }
}
