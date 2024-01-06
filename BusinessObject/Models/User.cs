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
    [Table("users")]
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("fullname")]
        public string FullName { get; set; }

        [Column("password")]
        public string? Password { get; set; }

        [Required]
        [Column("email")]
        public string Email { get; set; }

        [Column("phone")]
        public string? Phone { get; set; }

        [Required]
        [Column("birthday")]
        public DateTime BirthDay { get; set; }

        [Required]
        [Column("gender")]
        public bool Gender { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("skill")]
        public string Skill { get; set; }

        [Column("token")]
        public string? Token { get; set; }

        [Column("other_contact")]
        public string? OtherContact { get; set; }

        [Column("avatar")]
        public string? Avatar { get; set; }

        [Column("theme")]
        public string? Theme { get; set; }

        [Required]
        [Column("status")]
        public UserStatus Status { get; set; }

        [Required]
        [Column("is_admin")]
        public bool IsAdmin { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Column("verify_code")]
        public string? VerifyCode { get; set; }

        [Column("end_date_premium")]
        public DateTime? EndDatePremium { get; set; }

        [Column("last_login_date")]
        public DateTime? LastLoginDate { get; set; }

        [InverseProperty(nameof(Feedback.FeedbackedFor))]
        public List<Feedback> ReceivedFeedbacks { get; set; }

        [InverseProperty(nameof(Feedback.FeedbackedBy))]
        public List<Feedback> SentFeedbacks { get; set; }

        public List<Transaction> Transactions { get; set; }

        public List<UserMajor> UserMajors { get; set; }

        public List<Member> Members { get; set; }

        public List<Application> Applications { get; set; }
    }
}
