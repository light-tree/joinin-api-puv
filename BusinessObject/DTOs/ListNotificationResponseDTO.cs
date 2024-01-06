using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class ListNotificationResponseDTO
    {
        public List<NotificationDTO> notificationDTOs { get; set; }

        public int HasSeenNumber { get; set; }

        public int HasUnreadNumber { get; set;}
    }
}
