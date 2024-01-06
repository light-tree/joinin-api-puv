using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface INotificationRepository
    {
        Notification AddNotification(Notification notification);
        int CountNotificationByStatus(NotificationStatus notificationStatus, Guid userId);
        CommonResponse FilterNotifications(DateTime? StartDate, Guid UserId, NotificationType? notificationType, NotificationStatus? notificationStatus, int pageSize = 5, int pageNumber = 1);
        Notification GetNotificationById(Guid Id);
        Notification UpdateNotification(Notification notification);
    }
}
