using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("user_majors")]
    public class UserMajor
    {
        [Key]
        [ForeignKey(nameof(User))]
        [Column("user_id")]
        public Guid UserId { get; set; }

        [Key]
        [ForeignKey(nameof(Major))]
        [Column("major_id")]
        public Guid MajorId { get; set; }

        public User User { get; set; }

        public Major Major { get; set; }
    }
}
