using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Enums;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class MilestoneRepository : IMilestoneRepository
    {
        Context _context; 
        public MilestoneRepository(Context context)
        {
            _context = context;
        }

        public Guid CreateMilestone(MilestoneDTOForCreating milestoneCreate, int order)
        {
            BusinessObject.Models.Milestone Milestone = new BusinessObject.Models.Milestone();
            Milestone.Name = milestoneCreate.Name;
            Milestone.Description = milestoneCreate.Description;
            Milestone.GroupId = milestoneCreate.GroupId;
            Milestone.Order = order;
            _context.Milestones.Add(Milestone);
            _context.SaveChanges();
            return Milestone.Id;
        }

        public Milestone FindMilestone(Guid id)
        {
            return _context.Milestones.Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Milestone> GetAllMilestone() => _context.Milestones.ToList();

        public int GetLastOrder(Guid groupID)
        {
            if(_context.Milestones.Where(m => m.GroupId == groupID).Count() > 0)
            {
                return _context.Milestones.Where(m => m.GroupId == groupID).Max(m => m.Order);
            }
            return 0;
        }

        public Guid UpdateMilestone(MilestoneDTOForUpdating milestone)
        {
            var milestoneUpdate = FindMilestone(milestone.Id);
            milestoneUpdate.Name = milestone.Name;
            milestoneUpdate.Description = milestone.Description;
            _context.Milestones.Update(milestoneUpdate);
            _context.SaveChanges();
            return milestone.Id;
        }

        public int DeleteMilestone(Guid id)
        {
            var milestone = FindMilestone(id);
            _context.Milestones.Remove(milestone);
            return _context.SaveChanges();
        }

        public MilestonesDTO GetMilestonesByGroupId(Guid groupId)
        {
            List<Milestone> milestones = _context.Milestones.Where(m => m.GroupId == groupId).OrderBy(m => m.Order).ToList();
            Group group = _context.Groups
                .Include(g => g.CurrentMilestone)
                .FirstOrDefault(g => g.Id == groupId);
            int? currentMilestoneOrder = group.CurrentMilestone == null ? null : group.CurrentMilestone.Order;

            return new MilestonesDTO
            {
                Milestones = milestones,
                CurrentMilestoneOrder = currentMilestoneOrder,
            };
        }

        public Milestone GetNextMilestoneByWishType(Guid id, Guid groupID, WishType wishType)
        {
            var currentMilestone = _context.Milestones.Where(m => m.Id == id).FirstOrDefault();
            Milestone nextMilestone = null;
            if (wishType == WishType.INCREASE)
            {
                //get milestone to increase
                nextMilestone = _context.Milestones.Where(m => m.GroupId == groupID && m.Order == (currentMilestone.Order + 1)).FirstOrDefault();
                if (nextMilestone == null) throw new Exception("Currently this group does not have a milestone to increase");
                
            }
            else if(wishType == WishType.DECREASE)
            {
                nextMilestone = _context.Milestones.Where(m => m.GroupId == groupID && m.Order == (currentMilestone.Order - 1)).FirstOrDefault();
                if (nextMilestone == null) throw new Exception("Currently this group does not have a milestone to decrease");
            }
            return nextMilestone;
        }

        public int CountMilestone()
        {
            return _context.Milestones.Count();
        }

        public int SortOrderMilestone(Guid groupID, int startOrder)
        {
            var listMilestone = _context.Milestones.Where(m => m.GroupId == groupID && m.Order > startOrder).ToList();
                foreach (var item in listMilestone)
                {
                    item.Order = startOrder++;
                }
                return _context.SaveChanges();         
        }

        public int DeleteMilestoneByGroupId(Guid groupId)
        {
            List<Milestone> milestones = _context.Milestones.Where(m => m.GroupId == groupId).ToList();
            _context.Milestones.RemoveRange(milestones);
            return _context.SaveChanges();
        }
    }
}
