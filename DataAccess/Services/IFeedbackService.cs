using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IFeedbackService
    {
        Guid CreateFeedback(Guid userId, SentFeedbackDTO sentFeedbackDTO);
        CommonResponse FilterFeedbacks(Guid userId, int? pageSize, int? page, string? orderBy, string? value);
        float GetAverageRatingByUserId(Guid userId);
    }
}
