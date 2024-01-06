using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("assigned_tasks")]
    public class AssignedTask
    {
        [Key]
        [ForeignKey(nameof(Task))]
        [Column("task_id")]
        public Guid TaskId { get; set; }

        [Key]
        [ForeignKey(nameof(AssignedFor))]
        [Column("assigned_for_id")]
        public Guid AssignedForId { get; set; }

        [Key]
        [ForeignKey(nameof(AssignedBy))]
        [Column("assigned_by_id")]
        public Guid AssignedById { get; set; }

        [Required]
        [Column("assigned_date")]
        public DateTime AssignedDate { get; set; }

        public Task Task { get; set; }

        public Member AssignedFor { get; set; }

        public Member AssignedBy { get; set; }
    }
}
