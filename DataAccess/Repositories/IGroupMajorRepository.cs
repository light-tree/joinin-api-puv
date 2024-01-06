using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IGroupMajorRepository
    {
        GroupMajor CreateGroupMajor(Guid id, GroupMajorDTO groupMajorDTO);
        int DecreaseCurrentNeededMemberCount(GroupMajor groupMajor, int v);
        int DeleteByGroupIdAndMajorId(Guid groupId, Guid majorId);
        List<GroupMajor> FindByGroupId(Guid groupId);
        GroupMajor FindByGroupIdAndMajorId(Guid groupId, Guid majorId);
        List<GroupMajor> GetRecruitingGroupMajorsByGroupId(Guid groupId);
        int UpdateGroupMajor(Guid groupId, GroupMajorDTO groupMajorDTO);
    }
}
