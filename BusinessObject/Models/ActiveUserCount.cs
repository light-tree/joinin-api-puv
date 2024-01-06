using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("active_user_counts")]
    public class ActiveUserCount
    {
        [Key]
        [Column("start_date")]
        public DateTime StartDate { get; set; }

        [Key]
        [Column("end_date")]
        public DateTime EndDate { get; set; }

        [Required]
        [Column("user_count")]
        public int UserCount { get; set; }
    }
}
