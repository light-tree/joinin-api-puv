using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IFeedbackRepository
    {
        Guid CreateFeedback(Guid userId, SentFeedbackDTO sentFeedbackDTO);
        CommonResponse FilterFeedbacks(Guid userId, int? pageSize, int? page, string? orderBy, string? value);
        Feedback FindByFeedbackedByUserIdAndFeedbackedForUserIdAndGroupId(Guid feedbackedByUserId, Guid feedbackedForUserId, Guid groupId);
        float GetAverageRatingByUserId(Guid userId);
    }
}
