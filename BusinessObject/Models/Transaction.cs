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
    [Table("transactions")]
    public class Transaction
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Column("transaction_date")]
        public DateTime? TransactionDate { get; set; }

        [Required]
        [Column("status")]
        public TransactionStatus Status { get; set; }

        [Required]
        [Column("type")]
        public TransactionType Type { get; set; }

        [Required]
        [ForeignKey(nameof(User))]
        [Column("user_id")]
        public Guid UserId { get; set; }

        public User User { get; set; }

        [Required]
        [Column("created_date")]
        public DateTime CreatedDate { get; set; }

        [Required]
        [Column("transaction_code")]
        public string? TransactionCode { get; set; }
    }
}
