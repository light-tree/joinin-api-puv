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
    public interface IGroupRepository
    {
        int AssignCreater(Member member, Group group);
        Group CreateGroup(Guid userId, GroupDTOForCreating groupDTOForCreating);
        CommonResponse FilterGroups(Guid userId, string? name, string? type, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterGroupsToApply(Guid userId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value);
        Group FindById(Guid groupId);
        Group GetDetailById(Guid groupId);
        Group IncreaseCurrentMemberCount(Guid groupId, int v);
        Group IncreaseCurrentMemberCountByInvitation(Guid groupId, int v);
        bool IsStillJoinableWithUserType(Guid userId);
        Guid? UpdateCurrentMilestone(Guid groupID, Guid milestoneID);

        Guid? GetCurrentMilestone(Guid groupID);
        int UpdateGroup(GroupDTOForUpdating groupDTOForUpdating);
        int CountGroupByCreatedById(Guid createrId);
        Group DecreaseCurrentMemberCount(Guid groupId, int v);
        int UpdateGroupSizeAsPremiumByCreaterId(Guid userId);
        int InactivateGroup(Group group);
    }
}
