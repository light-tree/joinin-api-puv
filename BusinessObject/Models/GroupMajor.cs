using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("group_majors")]
    public class GroupMajor
    {
        [Key]
        [ForeignKey(nameof(Group))]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Key]
        [ForeignKey(nameof(Major))]
        [Column("major_id")]
        public Guid MajorId { get; set; }

        [Required]
        [Column("member_count")]
        public int MemberCount { get; set; }

        public Group Group { get; set; }

        public Major Major { get; set; }
    }
}
