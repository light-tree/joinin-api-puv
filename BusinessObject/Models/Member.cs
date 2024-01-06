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
    [Table("members")]
    public class Member
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [ForeignKey(nameof(User))]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [ForeignKey(nameof(Group))]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Required]
        [Column("joined_date")]
        public DateTime JoinedDate { get; set; }

        [Column("left_date")]
        public DateTime? LeftDate { get; set; }

        [Required]
        [Column("role")]
        public MemberRole Role { get; set; }

        public User User { get; set; }

        public Group Group { get; set; }
        public Group? CreatedGroup { get; set; }

        [InverseProperty(nameof(AssignedTask.AssignedFor))]
        public List<AssignedTask> AssignedTasksFor { get; set; }

        [InverseProperty(nameof(AssignedTask.AssignedBy))]
        public List<AssignedTask> AssignedTasksBy { get; set; }

        public List<Task> Tasks { get; set; }

    }
}
