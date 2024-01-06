using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BusinessObject.Models
{
    [Table("milestones")]
    public class Milestone
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [Column("id", TypeName = "char(36)")]
        public Guid Id { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Required]
        [Column("description")]
        public string Description { get; set; }

        [Required]
        [Column("order")]
        public int Order { get; set; }

        [Required]
        [ForeignKey(nameof(Group))]
        [Column("group_id")]
        public Guid GroupId { get; set; }

        [JsonIgnore]
        public Group Group { get; set; }

        [JsonIgnore]
        public Group? GroupForCurrent { get; set; }
    }
}
