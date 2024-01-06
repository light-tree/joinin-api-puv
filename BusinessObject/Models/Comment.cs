using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObject.Enums;

namespace BusinessObject.Models
{
    [Table("comments")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("status")]
        public CommentStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(Task))]
        [Column("task_id")]
        public Guid TaskId { get; set; }

        [Required]
        [ForeignKey(nameof(Task))]
        [Column("member_id")]
        public Guid MemberId { get; set; }

        public Task Task { get;}
    }
}
