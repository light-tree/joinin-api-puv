using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IMemberService
    {
        public Member findMemberByUserId(Guid userId, Guid groupId);

        public MemberRole? GetRoleInThisGroup(Guid userId, Guid groupId);

        public Member findMemberByMemberId(Guid memberId, Guid groupId);

        public Member updateMemberRole(Guid memberId, MemberRole role);

        List<Member> GetMembersByGroupId(Guid groupId, string? name, Guid userId);

        Member removeMemberFromGroup(Member member);

        int LeaveGroup(Guid userId, LeavingGroupDTO leavingGroupDTO);

        Member GetGroupLeader(Guid GroupId);
        List<Member> GetAllMemberOfGroup(Guid groupId);
    }
}
