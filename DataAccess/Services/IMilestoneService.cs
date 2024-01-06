using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IMilestoneService
    {
        public IEnumerable<Milestone> GetAllMilestone();

        public Guid CreateMilestone(MilestoneDTOForCreating milestone);

        public Guid UpdateMilestone(MilestoneDTOForUpdating milestione);

        public Milestone FindMilestone(Guid id);

        public int DeleteMilestone(Guid id);

        MilestonesDTO GetMilestonesByGroupId(Guid userId, Guid groupId);

        public Guid? UpdateCurrentMilestone(MilestoneDTOForUpdatingCurrentOrder milestone, Guid userId);
    }
}
