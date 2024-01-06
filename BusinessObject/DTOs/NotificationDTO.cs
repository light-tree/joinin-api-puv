using BusinessObject.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class NotificationDTO
    {

      
     
        public Guid Id { get; set; }

      
        public string Name { get; set; }

       
        public DateTime CreatedDate { get; set; }

       
        public NotificationType Type { get; set; }

      
        public NotificationStatus Status { get; set; }

      
        public string? Image { get; set; }

        public Guid? UserId { get; set; }

        public string message { get; set; }

        public string link { get; set; }





    }
}
