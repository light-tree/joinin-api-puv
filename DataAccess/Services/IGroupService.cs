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
    public interface IGroupService
    {
        Guid CreateGroup(Guid createrId, GroupDTOForCreating groupDTOForCreating);
        int DeleteGroup(Guid userId, Guid groupId);
        CommonResponse FilterGroups(Guid userId, string? name, string? type, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterGroupsToApply(Guid userId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value);
        Group GetDetailByIdAndUserId(Guid groupId, Guid userId);
        Group GetGroupByGuid(Guid groupId);
        Guid UpdateGroup(Guid userId, GroupDTOForUpdating groupDTOForUpdating);
    }
}
