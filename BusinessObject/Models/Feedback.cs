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
    [Table("feedbacks")]
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; }

        [Required]
        [Column("rating")]
        public float Rating { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("status")]
        public FeedbackStatus Status { get; set; }

        [Required]
        [ForeignKey(nameof(FeedbackedBy))]
        [Column("feedbacked_by_id")]
        public Guid FeedbackedById { get; set; }

        [Required]
        [ForeignKey(nameof(FeedbackedFor))]
        [Column("feedbacked_for_id")]
        public Guid FeedbackedForId { get; set; }

        [Required]
        [ForeignKey(nameof(Group))]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        public User FeedbackedBy { get; set; }

        public User FeedbackedFor { get; set; }

        public Group Group { get; set; }
    }
}
