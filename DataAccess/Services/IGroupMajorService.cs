using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IGroupMajorService
    {
        Guid CreateGroupMajors(Guid userId, GroupMajorsDTOForRecruiting groupMajorsDTOForRecruiting);

        List<GroupMajor> GetRecruitingGroupMajorsByGroupId(Guid groupId);
    }
}
