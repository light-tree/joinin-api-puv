using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IMilestoneRepository
    {
        public IEnumerable<Milestone> GetAllMilestone();

        public Guid CreateMilestone(MilestoneDTOForCreating milestone, int order);

        public int GetLastOrder(Guid groupID);

        public Guid UpdateMilestone(MilestoneDTOForUpdating milestione);

        public Milestone FindMilestone(Guid id);

        public int DeleteMilestone(Guid id);
        MilestonesDTO GetMilestonesByGroupId(Guid groupId);

        Milestone GetNextMilestoneByWishType(Guid id,Guid groupID, WishType wishType);

        int CountMilestone();

        public int SortOrderMilestone(Guid groupID, int startOrder);
        int DeleteMilestoneByGroupId(Guid groupId);
    }
}
