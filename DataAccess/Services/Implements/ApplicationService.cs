using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.Email;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.Implements;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class ApplicationService : IApplicationService
    {
        private readonly IApplicationRepository _applicationRepository;
        private readonly IApplicationMajorRepository _applicationMajorRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IGroupMajorRepository _groupMajorRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IUserMajorRepository _userMajorRepository;
        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        
       

        public ApplicationService(IApplicationRepository applicationRepository, IApplicationMajorRepository applicationMajorRepository, IGroupRepository groupRepository, IMemberRepository memberRepository, IGroupMajorRepository groupMajorRepository, IMajorRepository majorRepository, IUserMajorRepository userMajorRepository, IConfiguration configuration, IUserRepository userRepository)
        {
            _applicationRepository = applicationRepository;
            _applicationMajorRepository = applicationMajorRepository;
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
            _groupMajorRepository = groupMajorRepository;
            _majorRepository = majorRepository;
            _userMajorRepository = userMajorRepository;
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public Guid? ConfirmApplication(Guid leaderId, ConfirmedApplicationDTO confirmedApplicationDTO)
        {
            if (confirmedApplicationDTO.Status != ApplicationStatus.APPROVED && confirmedApplicationDTO.Status != ApplicationStatus.DISAPPROVED)
                throw new Exception($"Confirmed status must be {ApplicationStatus.APPROVED} or {ApplicationStatus.DISAPPROVED}.");

            Application application = _applicationRepository.FindById(confirmedApplicationDTO.ApplicationId);
            if (application == null)
                throw new Exception("Applcation does not exist.");

            if (application.Status != ApplicationStatus.WAITING)
                throw new Exception($"Applcation's status must be {ApplicationStatus.WAITING} to be confirmed by group's leader.");

            if (_memberRepository.GetRoleInThisGroup(leaderId, application.GroupId) != MemberRole.LEADER)
                throw new Exception("This user is not leader in this group.");

            if (_memberRepository.FindByUserIdAndGroupId(application.UserId, application.GroupId) != null)
                throw new Exception("Applied user is already a member of this group.");

            BusinessObject.Models.Group group = _groupRepository.FindById(application.GroupId);
            UserType createrUserType = _userRepository.GetUserTypeById(group.CreatedBy.UserId);
            int maxJoinedMember = createrUserType == UserType.FREEMIUM ?
                BusinessRuleData.GROUP_SIZE_FOR_FREEMIUM :
                BusinessRuleData.GROUP_SIZE_FOR_PREMIUM;
            if (group.MemberCount >= maxJoinedMember)
                throw new Exception($"Number of joined member of this groups have reached the limit for this group's creater account ({maxJoinedMember}).");

            Guid? resultId = _applicationRepository.ConfirmApplication(confirmedApplicationDTO);
            if (resultId == null) throw new Exception("Confirm application fail.");

            if (confirmedApplicationDTO.Status == ApplicationStatus.APPROVED)
            {
                _groupRepository.IncreaseCurrentMemberCount(application.GroupId, 1);

                List<GroupMajor> groupMajors = _groupMajorRepository.FindByGroupId(application.GroupId);
                List<ApplicationMajor> applicationMajors = _applicationMajorRepository.FindByApplicationId(resultId.Value);
                if (applicationMajors.GroupBy(x => x.MajorId).Any(g => g.Count() > 1))
                    throw new Exception("Exist duplicated major Id.");
                bool IsMatch;
                foreach (ApplicationMajor appliedMajor in applicationMajors)
                {
                    if (_majorRepository.FindByID(appliedMajor.MajorId) == null)
                        throw new Exception("Major with Id: " + appliedMajor.MajorId + " is not exist.");
                    IsMatch = false;
                    foreach (GroupMajor groupMajor in groupMajors)
                    {
                        if (appliedMajor.MajorId == groupMajor.MajorId)
                        {
                            if (!(groupMajor.MemberCount > 0))
                                throw new Exception("This group no longer need member has major with Id: " + appliedMajor.MajorId + ".");
                            IsMatch = true;
                            _groupMajorRepository.DecreaseCurrentNeededMemberCount(groupMajor, 1);
                            break;
                        }
                    }
                    if (!IsMatch)
                        throw new Exception("Applied major with Id: " + appliedMajor.MajorId + " does not match with group's application needs.");
                }
                _memberRepository.CreateMember(application.UserId, application.GroupId, MemberRole.MEMBER);
            }
            return resultId.Value;
        }

        public Guid? ConfirmInvitation(ConfirmedApplicationDTO confirmedInvitationDTO)
        {
            if (confirmedInvitationDTO.Status != ApplicationStatus.INVITE_APPROVED && confirmedInvitationDTO.Status != ApplicationStatus.INVITE_DISAPPROVED)
                throw new Exception($"Confirmed status must be {ApplicationStatus.INVITE_APPROVED} or {ApplicationStatus.INVITE_DISAPPROVED}.");

            Application application = _applicationRepository.FindById(confirmedInvitationDTO.ApplicationId);
            if (application == null)
                throw new Exception("Invitation does not exist.");

            if (application.Status != ApplicationStatus.INVITING)
                throw new Exception($"Invitation's status must be {ApplicationStatus.INVITING} to be confirmed by invited user.");

            if (_memberRepository.FindByUserIdAndGroupId(application.UserId, application.GroupId) != null)
                throw new Exception("Invited user is already a member of this group.");

            BusinessObject.Models.Group group = _groupRepository.FindById(application.GroupId);
            UserType createrUserType = _userRepository.GetUserTypeById(application.UserId);
            //int maxJoinedGroup = createrUserType == UserType.FREEMIUM ?
            //    BusinessRuleData.MAX_NUMBER_GROUP_FOR_FREEMIUM :
            //    BusinessRuleData.MAX_NUMBER_GROUP_FOR_PREMIUM;
            //if (_memberRepository.CountJoinedWorkingGroupAsMemberByUserId(application.UserId) >= maxJoinedGroup)
            //    throw new Exception($"Number of joined groups have reached the limit for this candidate account ({maxJoinedGroup}).");
            int maxJoinedMember = createrUserType == UserType.FREEMIUM ?
                BusinessRuleData.GROUP_SIZE_FOR_FREEMIUM :
                BusinessRuleData.GROUP_SIZE_FOR_PREMIUM;
            if (group.MemberCount >= maxJoinedMember)
                throw new Exception($"Number of joined member of this groups have reached the limit for this group's creater account ({maxJoinedMember}).");

            Guid? resultId = _applicationRepository.ConfirmApplication(confirmedInvitationDTO);
            if (resultId == null) throw new Exception("Confirm Invitation fail.");

            if(confirmedInvitationDTO.Status == ApplicationStatus.INVITE_APPROVED)
            {
                _groupRepository.IncreaseCurrentMemberCountByInvitation(application.GroupId, 1);
                _memberRepository.CreateMember(application.UserId, application.GroupId, MemberRole.MEMBER);
            }

            return resultId.Value;
        }

        public Guid CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO)
        {
            //if (!_groupRepository.IsStillJoinableWithUserType(userId))
            //    throw new Exception($"User has reach maximum number of group that can join as {_userRepository.GetUserTypeById(userId)}");

            if(sentApplicationDTO.MajorIds.GroupBy(x => x).Any(g => g.Count() > 1))
                throw new Exception("Exist duplicated major Id.");

            BusinessObject.Models.Group group = _groupRepository.FindById(sentApplicationDTO.GroupId);
            if (group == null)
                throw new Exception("Group does not exist.");

            if (_memberRepository.FindByUserIdAndGroupId(userId, sentApplicationDTO.GroupId) != null)
                throw new Exception("User is already a member of this group.");

            if (_applicationRepository.FindWaitingOrInvitingByUserIdAndGroupId(userId, sentApplicationDTO.GroupId) != null)
                throw new Exception("There are already an waiting-to-confirm application which sent by this user to this group.");

            UserType createrUserType = _userRepository.GetUserTypeById(userId);
            //int maxJoinedGroup = createrUserType == UserType.FREEMIUM ?
            //    BusinessRuleData.MAX_NUMBER_GROUP_FOR_FREEMIUM :
            //    BusinessRuleData.MAX_NUMBER_GROUP_FOR_PREMIUM;
            //if (_memberRepository.CountJoinedWorkingGroupAsMemberByUserId(userId) >= maxJoinedGroup)
            //    throw new Exception($"Number of joined groups have reached the limit for this account ({maxJoinedGroup}).");
            int maxJoinedMember = createrUserType == UserType.FREEMIUM ?
                BusinessRuleData.GROUP_SIZE_FOR_FREEMIUM :
                BusinessRuleData.GROUP_SIZE_FOR_PREMIUM;
            if (group.MemberCount >= maxJoinedMember)
                throw new Exception($"Number of joined member of this groups have reached the limit for this group's creater account ({maxJoinedMember}).");

            List<GroupMajor> groupMajors = _groupMajorRepository.FindByGroupId(sentApplicationDTO.GroupId);
            List<UserMajor> userMajors = _userMajorRepository.FindByUserId(userId);
            bool IsMatch;
            foreach (Guid majorAppliedId in sentApplicationDTO.MajorIds)
            {
                if (_majorRepository.FindByID(majorAppliedId) == null)
                    throw new Exception("Major with Id: " + majorAppliedId + " is not exist.");
                IsMatch = false;
                foreach (GroupMajor groupMajor in groupMajors)
                {
                    if (majorAppliedId == groupMajor.MajorId)
                    {
                        if(!userMajors.Select(um => um.MajorId).Contains(majorAppliedId))
                            throw new Exception("Your application's major: " + _majorRepository.FindByID(majorAppliedId).Name + " does not match with your major.");
                        if (!(groupMajor.MemberCount > 0))
                            throw new Exception("This group no longer need member has major: " + _majorRepository.FindByID(majorAppliedId).Name + ".");
                        IsMatch = true;
                        break;
                    }
                }
                if (!IsMatch)
                    throw new Exception("Applied major: " + _majorRepository.FindByID(majorAppliedId).Name + " does not match with group's application needs.");
            }
            Guid newApplicationId = _applicationRepository.CreateApplication(userId, sentApplicationDTO).Id;
            _applicationMajorRepository.CreateApplicationMajors(newApplicationId, sentApplicationDTO.MajorIds);
            return newApplicationId;
        }

        public async Task<List<InvitedMemberDTO>> CreateInvitations(Guid userId, SentInvitationsDTO sentInvitationsDTO)
        {
            if (sentInvitationsDTO.UserIds.GroupBy(x => x).Any(g => g.Count() > 1))
                throw new Exception("Exist duplicated invited user Id.");

            BusinessObject.Models.Group group = _groupRepository.FindById(sentInvitationsDTO.GroupId);
            if (group == null)
                throw new Exception("Group does not exist.");

            foreach(Guid invitedUserId  in sentInvitationsDTO.UserIds)
            {
                if (_applicationRepository.FindWaitingOrInvitingByUserIdAndGroupId(invitedUserId, sentInvitationsDTO.GroupId) != null)
                    throw new Exception("There are already an waiting-to-confirm invitation which sent by this group to this user.");
            }
            
            Member member = _memberRepository.FindByUserIdAndGroupId(userId, sentInvitationsDTO.GroupId);
            if (member == null)
                throw new Exception("Inviter not belong to group or 1 between member or group is not exist.");
            if (member.Role != MemberRole.LEADER)
                throw new Exception($"Member must be {MemberRole.LEADER} to invite.");

            UserType createrUserType = _userRepository.GetUserTypeById(group.CreatedBy.UserId);
            int maxJoinedMember = createrUserType == UserType.FREEMIUM ?
                BusinessRuleData.GROUP_SIZE_FOR_FREEMIUM :
                BusinessRuleData.GROUP_SIZE_FOR_PREMIUM;
            if ((group.MemberCount + sentInvitationsDTO.UserIds.Count) > maxJoinedMember)
                throw new Exception($"Total invited member and current group's member count must not higher than the group's size limit of this group's creater account ({maxJoinedMember}).");

            List<Guid> invitaionIds = new List<Guid>();
            int rs = 0;
            List<InvitedMemberDTO> listInvitedEmail = new List<InvitedMemberDTO>();
            string invatedLink = _configuration["BaseInvitationLink"];
            foreach (Guid invitedUserId in sentInvitationsDTO.UserIds)
            {
                if(_memberRepository.FindByUserIdAndGroupId(invitedUserId, sentInvitationsDTO.GroupId) != null)
                    throw new Exception($"User with Id: {invitedUserId} is already a member of this group.");
                var tmp = _applicationRepository.CreateInvitation(sentInvitationsDTO.GroupId, sentInvitationsDTO.Description, invitedUserId);
                // config for send email
                var g = _groupRepository.FindById(sentInvitationsDTO.GroupId);
                invatedLink = invatedLink + tmp.Id;
                User m = await _userRepository.FindAccountByGUID(invitedUserId);
                InvitedMemberDTO invitedMemberDTO = new InvitedMemberDTO();
                invitedMemberDTO.email = m.Email;
                invitedMemberDTO.receiverName = m.FullName;
                invitedMemberDTO.groupName = g.Name;
                invitedMemberDTO.inovationLink = invatedLink;
                invitedMemberDTO.content = sentInvitationsDTO.Description;
                invitedMemberDTO.senderName = _userRepository.FindAccountByGUID(userId).Result.FullName;
                invitedMemberDTO.userId = invitedUserId;
                invitedMemberDTO.groupLogo = g.Avatar;
                listInvitedEmail.Add(invitedMemberDTO);


                invitaionIds.Add(tmp.Id);
               
                rs++;
            }

            if (rs != sentInvitationsDTO.UserIds.Count)
                throw new Exception("Create invitation not success.");

           
            //for (int i = 0; i < invitaionIds.Count; i++)
            //{
            //    _applicationMajorRepository.CreateApplicationMajorsUsingInvitation(sentInvitationsDTO.UserIds[i], invitaionIds[i]);
            //}

            return listInvitedEmail;
        }

        public CommonResponse FilterApplications(Guid userId, Guid groupId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            Member member = _memberRepository.FindByUserIdAndGroupId(userId, groupId);
            if (member == null)
                throw new Exception("User not belong to group or 1 between member or group is not exist.");
            if (member.Role != MemberRole.LEADER)
                throw new Exception($"Member must be {MemberRole.LEADER} to see the list of applications which were sent to this group.");
            return _applicationRepository.FilterApplications(groupId, majorIds, name, pageSize, page, orderBy, value);
        }

       
        public Application GetApplication(Guid applicationId)
        {
            return _applicationRepository.FindById(applicationId);
        }

        public Application GetInvitationDetailByIdAndUserId(Guid applicationId, Guid userId)
        {
            Application invitation = _applicationRepository.FindWaitingInvitationByIdAndUserId(applicationId, userId);
            if (invitation == null)
                throw new Exception("Invitation does not exist.");
            return invitation;
        }
    }
}
