using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface INotificationService
    {
        Notification AddNotification(Notification notification);
        CommonResponse FilterNotification(DateTime StartDate, Guid UserId, NotificationType notificationType, NotificationStatus notificationStatus, int pageSize = 5, int pageNumber = 1);
        CommonResponse UpdateNotificationStatus(NotificationUpdateDTO notificationUpdateDTO);
    }
}
