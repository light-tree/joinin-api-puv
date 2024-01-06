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
    [Table("applications")]
    public class Application
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("status")]
        public ApplicationStatus Status { get; set; }

        [Column("confirmed_date")]
        public DateTime? ConfirmedDate { get; set;}

        [Required]
        [Column("description")]
        public string? Description { get; set;}

        [Required]
        [ForeignKey(nameof(User))]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Required]
        [ForeignKey(nameof(Group))]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        public User User { get; set; }

        public Group Group { get; set;}

        public List<ApplicationMajor> ApplicationMajors { get; set;}
    }
}
