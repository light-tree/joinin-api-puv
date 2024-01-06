using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class FeedbackService : IFeedbackService
    {
        private readonly IFeedbackRepository _feedbackRepository;
        private readonly IMemberRepository _memberRepository;

        public FeedbackService(IFeedbackRepository feedbackRepository, IMemberRepository memberRepository)
        {
            _feedbackRepository = feedbackRepository;
            _memberRepository = memberRepository;
        }

        public Guid CreateFeedback(Guid userId, SentFeedbackDTO sentFeedbackDTO)
        {
            Member feedbackedBy = _memberRepository.FindByUserIdAndGroupId(userId, sentFeedbackDTO.GroupId);
            if (feedbackedBy == null)
                throw new Exception("User who feedback not belong to group or 1 between member or group is not exist.");

            Member feedbackedFor = _memberRepository.FindByIdAndGroupId(sentFeedbackDTO.MemberId, sentFeedbackDTO.GroupId);
            if (feedbackedFor == null)
                throw new Exception("User who is feedbacked not belong to group or 1 between member or group is not exist.");

            double joinedDaysOfFeedbackedBy = (DateTime.Now - feedbackedBy.JoinedDate).TotalDays;
            if (joinedDaysOfFeedbackedBy < 30)
                throw new Exception("User who feedback must joined group at least 30 days to feedback other members");

            double joinedDaysOfFeedbackedFor = (DateTime.Now - feedbackedFor.JoinedDate).TotalDays;
            if (joinedDaysOfFeedbackedFor < 30)
                throw new Exception("User who is feedbacked must joined group at least 30 days to feedback other members");

            if (feedbackedBy.Role == MemberRole.LEADER)
            {
                if (feedbackedFor.Role != MemberRole.LEADER)
                {
                    if (_feedbackRepository.FindByFeedbackedByUserIdAndFeedbackedForUserIdAndGroupId(feedbackedBy.UserId, feedbackedFor.UserId, sentFeedbackDTO.GroupId) != null)
                        throw new Exception("Feedback is already exist.");
                    return _feedbackRepository.CreateFeedback(userId, sentFeedbackDTO);
                }
                else throw new Exception($"{MemberRole.LEADER} can only feedback {MemberRole.SUB_LEADER} and {MemberRole.MEMBER}.");
            }
            else
            {
                if (feedbackedFor.Role == MemberRole.LEADER)
                {
                    if (_feedbackRepository.FindByFeedbackedByUserIdAndFeedbackedForUserIdAndGroupId(feedbackedBy.UserId, feedbackedFor.UserId, sentFeedbackDTO.GroupId) != null)
                        throw new Exception("Feedback is already exist.");
                    return _feedbackRepository.CreateFeedback(userId, sentFeedbackDTO);
                }
                else throw new Exception($"{MemberRole.SUB_LEADER} and {MemberRole.MEMBER} can only feedback {MemberRole.LEADER}.");
            }
        }

        public CommonResponse FilterFeedbacks(Guid userId, int? pageSize, int? page, string? orderBy, string? value)
        {
            return _feedbackRepository.FilterFeedbacks(userId, pageSize, page, orderBy, value);
        }

        public float GetAverageRatingByUserId(Guid userId)
        {
            return _feedbackRepository.GetAverageRatingByUserId(userId);
        }
    }
}
