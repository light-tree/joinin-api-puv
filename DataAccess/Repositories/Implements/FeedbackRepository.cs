using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class FeedbackRepository : IFeedbackRepository
    {
        private readonly Context _context;

        public FeedbackRepository(Context context)
        {
            _context = context;
        }

        public Guid CreateFeedback(Guid userId, SentFeedbackDTO sentFeedbackDTO)
        {
            Feedback feedback = new Feedback
            {
                Content = sentFeedbackDTO.Content,
                Rating = sentFeedbackDTO.Rating,
                CreatedDate = DateTime.Now,
                Status = BusinessObject.Enums.FeedbackStatus.ACTIVE,
                FeedbackedById = userId,
                FeedbackedForId = _context.Members.FirstOrDefault(m => m.Id == sentFeedbackDTO.MemberId).UserId
            };

            _context.Add(feedback);
            _context.SaveChanges();
            return feedback.Id;
        }

        public CommonResponse FilterFeedbacks(Guid userId, int? pageSize, int? page, string? orderBy, string? value)
        {
            List<Feedback> feedbacks = _context.Feedbacks
                .Where(f => f.FeedbackedForId == userId && f.Status == BusinessObject.Enums.FeedbackStatus.ACTIVE)
                .Select(f => new Feedback
                {
                    Id = f.Id,
                    Rating = f.Rating,
                    CreatedDate = f.CreatedDate,
                    Content = f.Content,
                })
                .ToList();

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    feedbacks = feedbacks.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    feedbacks = feedbacks.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }
            else
                feedbacks = feedbacks.OrderByDescending(t => t.CreatedDate).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = feedbacks.Count;

            response.Data = feedbacks;
            response.Pagination = pagination;
            response.Message = "Filter feedback list success.";
            response.Status = 200;

            return response;
        }

        public Feedback FindByFeedbackedByUserIdAndFeedbackedForUserIdAndGroupId(Guid feedbackedByUserId, Guid feedbackedForUserId, Guid groupId)
        {
            return _context.Feedbacks.FirstOrDefault(f => f.FeedbackedById == feedbackedByUserId && f.FeedbackedForId == feedbackedForUserId && f.GroupId == groupId);
        }

        public float GetAverageRatingByUserId(Guid userId)
        {
            return _context.Feedbacks
                .Where(f => f.FeedbackedForId == userId && f.Status == BusinessObject.Enums.FeedbackStatus.ACTIVE)
                .Select(f => f.Rating).Average();
        }

        static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj);
        }
    }
}
