using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class NotificationUpdateDTO
    {
        [Required]
       public  List<Guid> NotificationUpdateDTOs { get; set; }
    }
}
