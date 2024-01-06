using BusinessObject.DTOs;
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
    public class GroupMajorService : IGroupMajorService
    {
        private readonly IGroupMajorRepository _groupMajorRepository;
        private readonly IGroupRepository _groupRepository;
        private readonly IMajorRepository _majorRepository;
        private readonly IMemberRepository _memberRepository;

        public GroupMajorService(IGroupMajorRepository groupMajorRepository, IGroupRepository groupRepository, IMajorRepository majorRepository, IMemberRepository memberRepository)
        {
            _groupMajorRepository = groupMajorRepository;
            _groupRepository = groupRepository;
            _majorRepository = majorRepository;
            _memberRepository = memberRepository;
        }

        public Guid CreateGroupMajors(Guid userId, GroupMajorsDTOForRecruiting groupMajorsDTOForRecruiting)
        {
            Group group = _groupRepository.FindById(groupMajorsDTOForRecruiting.GroupId);

            if (group == null)
                throw new Exception("Group does not exist.");

            MemberRole? memberRole = _memberRepository.GetRoleInThisGroup(userId, groupMajorsDTOForRecruiting.GroupId);
            if (memberRole != MemberRole.LEADER)
                throw new Exception($"Member must be {MemberRole.LEADER} to recruit members.");
            int totalMemberNeeded = groupMajorsDTOForRecruiting.GroupMajorsDTO.Select(gm => gm.MemberCount).Sum();
            int totalMemberCanBeRecruiting = group.GroupSize - group.MemberCount;
            if (totalMemberNeeded > totalMemberCanBeRecruiting)
                throw new Exception($"Total member needed must not higher than the team's size minus current number of member ({totalMemberCanBeRecruiting}).");

            List<GroupMajor> currentRecruitingMajors = _groupMajorRepository.FindByGroupId(group.Id).ToList();
            List<Guid> currentRecruitingMajorIds = currentRecruitingMajors.Select(gm => gm.MajorId).ToList();
            List<Guid> newRecruitingMajorIds = groupMajorsDTOForRecruiting.GroupMajorsDTO.Select(gm => gm.MajorId).ToList();
            List<Guid> lostRecruitingMajorIds = currentRecruitingMajorIds.Except(newRecruitingMajorIds).ToList();
            foreach (Guid majorId in lostRecruitingMajorIds)
            {
                _groupMajorRepository.DeleteByGroupIdAndMajorId(group.Id, majorId);
            }

            List<Guid> addedRecruitingMajorIds = newRecruitingMajorIds.Except(currentRecruitingMajorIds).ToList();
            List<GroupMajorDTO> addedRecruitingMajors = groupMajorsDTOForRecruiting.GroupMajorsDTO
                .Where(gm => addedRecruitingMajorIds.Contains(gm.MajorId)).ToList();
            foreach (GroupMajorDTO groupMajorDTO in addedRecruitingMajors)
            {
                _groupMajorRepository.CreateGroupMajor(group.Id, groupMajorDTO);
            }

            List<GroupMajorDTO> remainingRecruitingMajors = groupMajorsDTOForRecruiting.GroupMajorsDTO
                .Where(gm => currentRecruitingMajorIds.Contains(gm.MajorId)).ToList();
            foreach (GroupMajorDTO groupMajorDTO in remainingRecruitingMajors)
            {
                foreach (GroupMajor groupMajor in currentRecruitingMajors)
                {
                    if (groupMajor.MajorId == groupMajorDTO.MajorId && groupMajor.MemberCount != groupMajorDTO.MemberCount)
                    {
                        if (groupMajorDTO.MemberCount > 0)
                            _groupMajorRepository.UpdateGroupMajor(group.Id, groupMajorDTO);
                        else
                            _groupMajorRepository.DeleteByGroupIdAndMajorId(group.Id, groupMajorDTO.MajorId);
                        break;
                    }
                }
            }

            return group.Id;
        }

        public List<GroupMajor> GetRecruitingGroupMajorsByGroupId(Guid groupId)
        {
            return _groupMajorRepository.GetRecruitingGroupMajorsByGroupId(groupId);
        }
    }
}