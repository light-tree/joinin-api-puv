using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BusinessObject.Enums;

namespace BusinessObject.Models
{
    [Table("groups")]
    public class Group
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("group_size")]
        public int GroupSize { get; set; }

        [Required]
        [Column("member_count")]
        public int MemberCount { get; set; }

        [Column("school_name")]
        public string? SchoolName { get; set; }

        [Column("class_name")]
        public string? ClassName { get; set; }

        [Column("subject_name")]
        public string? SubjectName { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("skill")]
        public string? Skill { get; set; }

        [Column("avatar")]
        public string? Avatar { get; set; }


        [Required]
        [Column("status")]
        public GroupStatus Status { get; set; }

        [ForeignKey(nameof(CurrentMilestone))]
        [Column("current_milestone_id")]
        public Guid? CurrentMilestoneId { get; set; }

        public Milestone? CurrentMilestone { get; set; }

        [ForeignKey(nameof(CreatedBy))]
        [Column("created_by_id")]
        public Guid? CreatedById { get; set; }

        public Member? CreatedBy { get; set; }

        [Column("theme")]
        public string? Theme { get; set; }


        [InverseProperty(nameof(Milestone.Group))]
        public List<Milestone> Milestones { get; set; }

        [InverseProperty(nameof(Member.Group))]
        public List<Member> Members { get; set; }

        public List<Task> Tasks { get; set; }

        public List<GroupMajor> GroupMajors { get; set; }

        public List<Application> Applications { get; set; }

        public List<Feedback> Feedbacks { get; set; }
    }
}
