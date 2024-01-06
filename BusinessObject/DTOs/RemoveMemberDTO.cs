using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class RemoveMemberDTO
    {
        [Required]
       public Guid MemberId { get; set; }
        [Required]
       public Guid groupId { get; set; }  

       [Required]
        public string Description {get; set; }

    }
}
