using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.User;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace DataAccess.Services.Implements
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _notificationRepository;
       

        public NotificationService (INotificationRepository notificationRepository, IConfiguration configuration)
        {
            _notificationRepository= notificationRepository;
          
        }


        public CommonResponse FilterNotification(DateTime StartDate,
                                                      Guid UserId,
                                                      NotificationType notificationType,
                                                      NotificationStatus notificationStatus,
                                                      int pageSize = 5,
                                                      int pageNumber = 1)
        {
          
            CommonResponse commonResponse = new CommonResponse();

            try
            {
                
                List<NotificationDTO> notificationDTOs = new List<NotificationDTO>();
                ListNotificationResponseDTO listNotificationResponseDTO = new ListNotificationResponseDTO();
                var rs = _notificationRepository.FilterNotifications(null, UserId, null, null, pageSize, pageNumber);
                List<Notification> notifications = (List<Notification>)rs.Data;
                if (rs == null || notifications.Count == 0)
                {
                    listNotificationResponseDTO.notificationDTOs = notificationDTOs;
                    commonResponse.Data = listNotificationResponseDTO;
                    commonResponse.Message = "Notification empty";
                    return commonResponse;

                }
              
                
             
                
                if (notifications != null && notifications.Count > 0)
                {
                    int hasSeenNotification = 0;
                    int unReadNotification = 0;
                    foreach (var n in notifications)
                    {
                       NotificationDTO notificationDTO =  JsonConvert.DeserializeObject<NotificationDTO>(n.Content);
                        notificationDTO.Id = n.Id;
                        notificationDTO.CreatedDate = n.CreatedDate;
                        notificationDTO.Status = n.Status;
                        
                        notificationDTOs.Add(notificationDTO);
                        
                        
                    }
                    
                    listNotificationResponseDTO.notificationDTOs = notificationDTOs;
                    
                    listNotificationResponseDTO.HasSeenNumber = _notificationRepository.CountNotificationByStatus(NotificationStatus.SEEN, UserId);
                    listNotificationResponseDTO.HasUnreadNumber = _notificationRepository.CountNotificationByStatus(NotificationStatus.NOT_SEEN_YET, UserId); ;
                    
                    commonResponse.Data = listNotificationResponseDTO;
                    commonResponse.Message = "Filter Success";
                    commonResponse.Pagination = rs.Pagination;
                    return commonResponse;  

                } else
                {
                    commonResponse.Data = "";
                    commonResponse.Message = "Notification empty";
                    return commonResponse;
                }
            }
            catch
            {
                return null;
            }
         
        
        }

        public Notification AddNotification(Notification notification)
        {
            try
            {
              
                var rs = _notificationRepository.AddNotification(notification);
                return rs;
            }
            catch
            {
                return null;
            }
        }

        public CommonResponse UpdateNotificationStatus(NotificationUpdateDTO notificationUpdateDTO)
        {
            CommonResponse commonResponse = new CommonResponse();
            try
            {
                foreach (Guid tmp in notificationUpdateDTO.NotificationUpdateDTOs)
                {
                    var n = _notificationRepository.GetNotificationById(tmp);
                    if (n == null) {
                        continue;
                    } else
                    {
                        var json = JsonConvert.DeserializeObject<NotificationDTO>(n.Content);
                        n.Status = NotificationStatus.SEEN;
                        NotificationDTO notificationDTO = new NotificationDTO();
                        notificationDTO.Id = n.Id;
                        notificationDTO.Status = n.Status;
                        notificationDTO.CreatedDate = n.CreatedDate;
                        notificationDTO.UserId = n.UserId;
                        notificationDTO.Type  = n.Type;
                        notificationDTO.Image = n.Image;
                        notificationDTO.Name = n.Name;
                        notificationDTO.message =
                        notificationDTO.message = json.message;
                        notificationDTO.link = json.link;

                        n.Content = JsonConvert.SerializeObject(notificationDTO);
                        _notificationRepository.UpdateNotification(n);
                    }

                }
                commonResponse.Status = 200;
                commonResponse.Message = "Update successfully.";
                return commonResponse;
            }
            
            catch
            {
                commonResponse.Status = 500;
                commonResponse.Message = "Internal server error.";
                return commonResponse;
            }
            
        }

            
    }
}
