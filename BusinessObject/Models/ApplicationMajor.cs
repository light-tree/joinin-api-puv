using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.Models
{
    [Table("application_majors")]
    public class ApplicationMajor
    {
        [Key]
        [ForeignKey(nameof(Application))]
        [Column("application_id")]
        public Guid ApplicationId { get; set; }

        [Key]
        [ForeignKey(nameof(Major))]
        [Column("major_id")]
        public Guid MajorId { get; set; }

        public Application Application { get; set; }

        public Major Major { get; set; }
    }
}
