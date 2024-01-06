using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class MemberService : IMemberService
    {

        private readonly IMemberRepository _memberRepository; 
        private readonly IGroupRepository _groupRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly IAssignedTaskRepository _assignedTaskRepository;
        private readonly IUserRepository _userRepository;
        

        public MemberService(IMemberRepository memberRepository, IGroupRepository groupRepository, ITaskRepository taskRepository, IAssignedTaskRepository assignedTaskRepository, IUserRepository userRepository)
        {
            _memberRepository = memberRepository;
            _groupRepository = groupRepository;
            _taskRepository = taskRepository;
            _assignedTaskRepository = assignedTaskRepository;
            _userRepository = userRepository;
        }

        public Member findMemberByUserId(Guid userId, Guid groupId)
        {
            return _memberRepository.FindByUserIdAndGroupIdIncludeEmail(userId, groupId);
        }

        public MemberRole? GetRoleInThisGroup(Guid userId, Guid groupId)
        {
          
            return _memberRepository.GetRoleInThisGroup(userId, groupId);
        }

        public Member findMemberByMemberId(Guid memberId, Guid groupId)
        {
            return _memberRepository.FindByMemberIdAndGroupId(memberId, groupId);
        }

        public Member updateMemberRole(Guid memberId, MemberRole role) {

            

            return _memberRepository.UpdateMemberRole(memberId, role);
        }

        public List<Member> GetMembersByGroupId(Guid groupId, string? name, Guid userId)
        {
            if (_memberRepository.FindByUserIdAndGroupId(userId, groupId) == null)
                throw new Exception("User not belong to group or 1 between member or group is not exist.");

            return _memberRepository.GetMembersByGroupId(groupId, name);
        }

        public Member removeMemberFromGroup(Member member)
        {
           try
            {
                member.LeftDate= DateTime.Now;

                //check task được gán nhưng chưa bắt đầu, bỏ gán
                 _assignedTaskRepository.DeleteAssignedTask(member.Id, member.GroupId, BusinessObject.Enums.TaskStatus.NOT_STARTED_YET);
                _memberRepository.UpdateMember(member);
               
                _groupRepository.DecreaseCurrentMemberCount(member.GroupId, 1);

                return member;
            }
            catch
            {
                return null;
            }
        }

        public int LeaveGroup(Guid userId, LeavingGroupDTO leavingGroupDTO)
        {
            BusinessObject.Models.Group group = _groupRepository.FindById(leavingGroupDTO.GroupId);
            if (group == null)
                throw new Exception("Group does not exist.");

            Member member = _memberRepository.FindByUserIdAndGroupId(userId, leavingGroupDTO.GroupId);
            if (member == null)
                throw new Exception("User not belong to group or 1 between member or group is not exist.");
            if (member.Role == MemberRole.LEADER)
                throw new Exception($"{MemberRole.LEADER} must assign an other member become {MemberRole.LEADER} before leave.");
            if(member.Id == group.CreatedById)
                throw new Exception("Group creater can not leave the group.");

            List<AssignedTask> deletedAssignedTasks = _assignedTaskRepository.FindNotFinishedByAssignedForId(member.Id);
            List<Guid> taskIdsOfDeletedAssignedTasks = deletedAssignedTasks.Select(at => at.TaskId).ToList();
            List<string> memberEmailsToNotify = _memberRepository
                .GetMembersToTotifyByTaskIdsExceptLeftMemberId(taskIdsOfDeletedAssignedTasks, userId)
                .Select(m => m.User.Email).ToList();

            _assignedTaskRepository.Delete(deletedAssignedTasks);
            _groupRepository.DecreaseCurrentMemberCount(leavingGroupDTO.GroupId, 1);
            return _memberRepository.LeaveGroup(member);
        }
        public Member GetGroupLeader(Guid GroupId)
        {
            return _memberRepository.GetGroupLeader(GroupId);
        }

        public List<Member> GetAllMemberOfGroup(Guid groupId)
        {
            return _memberRepository.GetAllMemberOfGroup(groupId);
        }
    }
}
