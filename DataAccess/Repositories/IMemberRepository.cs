using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IMemberRepository
    {
        Member CreateMember(Guid userId, Guid groupId, MemberRole role);
        Member FindByUserIdAndGroupId(Guid userId, Guid groupId);
        Member FindByIdAndGroupId(Guid id, Guid groupId);
        MemberRole? GetRoleInThisGroup(Guid userId, Guid groupId);
        Member UpdateMemberRole(Guid memberId, MemberRole role);
        List<Member> GetMembersByGroupId(Guid groupId, string? name);
        Member UpdateMember(Member member);
        int CountJoinedWorkingGroupAsMemberByUserId(Guid userId);
        Member GetGroupLeader(Guid groupId);
        List<Member> GetMembersToTotifyByTaskIdsExceptLeftMemberId(List<Guid> taskIdsOfDeletedAssignedTasks, Guid userId);
        int LeaveGroup(Member member);
        Member FindByMemberIdAndGroupId(Guid id, Guid groupId);
        Member FindByUserIdAndGroupIdIncludeEmail(Guid userId, Guid groupId);
        int DeleteByGroupId(Guid groupId);
        List<Member> GetAllMemberOfGroup(Guid groupId);
    }
}
