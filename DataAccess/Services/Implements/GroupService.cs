using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
using BusinessObject.Models;
using DataAccess.Repositories;
using DataAccess.Repositories.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IMemberRepository _memberRepository;
        private readonly IGroupMajorRepository _groupMajorRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IMilestoneRepository _milestoneRepository;
        private readonly IAssignedTaskRepository _assignedTaskRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ICommentRepository _commentRepository;

        public GroupService(IGroupRepository groupRepository, IMemberRepository memberRepository, IGroupMajorRepository groupMajorRepository, IUserRepository userRepository, IMajorRepository majorRepository, IMilestoneRepository milestoneRepository, IAssignedTaskRepository assignedTaskRepository, ITaskRepository taskRepository, ICommentRepository commentRepository)
        {
            _groupRepository = groupRepository;
            _memberRepository = memberRepository;
            _groupMajorRepository = groupMajorRepository;
            _userRepository = userRepository;
            _majorRepository = majorRepository;
            _milestoneRepository = milestoneRepository;
            _assignedTaskRepository = assignedTaskRepository;
            _taskRepository = taskRepository;
            _commentRepository = commentRepository;
        }

        public Guid CreateGroup(Guid createrId, GroupDTOForCreating groupDTOForCreating)
        {
            UserType createrUserType = _userRepository.GetUserTypeById(createrId);

            int maxGroup = createrUserType == UserType.FREEMIUM ? 
                BusinessRuleData.MAX_NUMBER_GROUP_FOR_FREEMIUM : 
                BusinessRuleData.MAX_NUMBER_GROUP_FOR_PREMIUM;

            if (_groupRepository.CountGroupByCreatedById(createrId) >= maxGroup)
                throw new Exception($"Number of created groups have reached the limit for this creater account ({maxGroup}).");
            
            Group group = _groupRepository.CreateGroup(createrId, groupDTOForCreating);
            Member member = _memberRepository.CreateMember(createrId, group.Id, MemberRole.LEADER);

            _groupRepository.AssignCreater(member, group);

            return group.Id;
        }

        public CommonResponse FilterGroups(Guid userId, string? name, string? type, int? pageSize, int? page, string? orderBy, string? value)
        {
            return _groupRepository.FilterGroups(userId, name, type, pageSize, page, orderBy, value);
        }

        public CommonResponse FilterGroupsToApply(Guid userId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            return _groupRepository.FilterGroupsToApply(userId, majorIds, name, pageSize, page, orderBy, value);
        }

        public Group GetDetailByIdAndUserId(Guid groupId, Guid userId)
        {
            //if(_memberRepository.FindByUserIdAndGroupId(userId, groupId) == null)
            //    throw new Exception("User not belong to group or 1 between member or group is not exist.");

            return _groupRepository.GetDetailById(groupId);
        }

        public Guid UpdateGroup(Guid userId, GroupDTOForUpdating groupDTOForUpdating)
        {
            Member member = _memberRepository.FindByUserIdAndGroupId(userId, groupDTOForUpdating.Id);

            if (member == null)
                throw new Exception("Updater not belong to group or 1 between member or group is not exist.");

            MemberRole memberRole = member.Role;
            if (memberRole != MemberRole.LEADER)
                throw new Exception($"Member must be {MemberRole.LEADER} to update task.");

            _groupRepository.UpdateGroup(groupDTOForUpdating);
            return groupDTOForUpdating.Id;
        }

        public Group GetGroupByGuid(Guid groupId)
        {
            return _groupRepository.FindById(groupId);
        }

        public int DeleteGroup(Guid userId, Guid groupId)
        {
            Member member = _memberRepository.FindByUserIdAndGroupId(userId, groupId);
            if (member == null)
                throw new Exception("User not belong to group or 1 between member or group is not exist.");

            Group group = _groupRepository.FindById(groupId);
            if (group.CreatedById != member.Id) 
                throw new Exception("User must be group creater to delete this group.");

            int rs = _groupRepository.InactivateGroup(group);
            _milestoneRepository.DeleteMilestoneByGroupId(groupId);
            _assignedTaskRepository.DeleteByGroupId(groupId);
            _commentRepository.DeleteByGroupId(groupId);
            _taskRepository.DeleteByGroupId(groupId);
            _memberRepository.DeleteByGroupId(groupId);
            return rs;
        }
    }
}
