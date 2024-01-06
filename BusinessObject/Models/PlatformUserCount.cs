using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("platform_user_counts")]
    public class PlatformUserCount
    {
        [Key]
        [Column("platform_name")]
        public string PlatformName { get; set; }

        [Required]
        [Column("user_count")]
        public int UserCount { get; set; }
    }
}
