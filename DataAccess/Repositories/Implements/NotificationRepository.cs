using BusinessObject.Data;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DataAccess.Repositories.Implements
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly Context _context;
        public NotificationRepository(Context context) {
            this._context = context;
        }

        public Notification GetNotificationById(Guid Id)
        {
            return _context.Notifications.Where(n => n.Id == Id).FirstOrDefault();
        }

        public Notification AddNotification(Notification notification)
        {
            try
            {
                _context.Notifications.Add(notification);
                _context.SaveChanges();
                return notification;
            } catch (Exception ex)
            {
                return null;
            }
        }

        public Notification UpdateNotification(Notification notification)
        {
            try
            {
                _context.Entry(notification).State = EntityState.Modified;
                _context.SaveChanges();
                return notification;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public CommonResponse FilterNotifications(DateTime? StartDate,
                                                      Guid UserId,
                                                      NotificationType? notificationType,
                                                      NotificationStatus? notificationStatus,
                                                      int pageSize = 5,
                                                      int pageNumber = 1)
        {
            try
            {
                CommonResponse commonResponse = new CommonResponse();
                IQueryable<Notification> notifications = _context.Notifications;
                Pagination pagination = new Pagination();
                pagination.PageSize = pageSize;
                pagination.CurrentPage = pageNumber;
                pagination.Total = notifications.Count();


                if (UserId != Guid.Empty)
                {
                    notifications = notifications.Where(n => n.UserId == UserId);
                }
                if (notificationType != null)
                {
                    notifications = notifications.Where(n => n.Type == notificationType);
                }
                if (notificationStatus != null)
                {
                    notifications = notifications.Where(n => n.Status == notificationStatus);
                }
                if (StartDate != null)
                {
                    notifications = notifications.Where(n => n.CreatedDate >= StartDate);
                }
                var orderedEnumerable = notifications.Skip((pageNumber - 1) * pageSize)
                        .Take(pageSize);
                var rs = orderedEnumerable.OrderByDescending(p => p.CreatedDate).ToList();
                commonResponse.Status = 200;
                commonResponse.Pagination = pagination;
                commonResponse.Data = rs;
                return commonResponse;
                
            } catch
            {
                return null;
            }
         
        }

        public int CountNotificationByStatus(NotificationStatus notificationStatus, Guid userId)
        {
            return _context.Notifications.Count(p => p.Status == notificationStatus && p.UserId == userId);
        }

      
    }
}
