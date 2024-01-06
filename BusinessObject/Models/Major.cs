using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("majors")]
    public class Major
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("short_name")]
        public string ShortName { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        public List<ApplicationMajor> ApplicationMajors { get; set; }

        public List<GroupMajor> GroupMajors { get; set; }

        public List<UserMajor> UserMajors { get; set; }

        public static implicit operator List<object>(Major v)
        {
            throw new NotImplementedException();
        }
    }
}
