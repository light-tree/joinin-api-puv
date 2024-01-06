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
    public class MilestoneService : IMilestoneService
    {
        IMilestoneRepository _milestoneRepository;
        IGroupRepository _groupRepository;
        private readonly IMemberRepository _memberRepository;

        public MilestoneService(IMilestoneRepository milestoneRepository, IGroupRepository groupRepository, IMemberRepository memberRepository)
        {
            this._milestoneRepository = milestoneRepository; 
            this._groupRepository = groupRepository;
            this._memberRepository = memberRepository;
        }

        public Guid CreateMilestone(MilestoneDTOForCreating milestone)
        {
            var group = _groupRepository.FindById(milestone.GroupId);
            if (group == null) throw new Exception("Group id is not exit in application");
            var newOrder = _milestoneRepository.GetLastOrder(group.Id) + 1;
            //create milestone
            var id =  _milestoneRepository.CreateMilestone(milestone, newOrder);
            if(newOrder == 1)
            {
                _groupRepository.UpdateCurrentMilestone(group.Id, id);
            }
            return id;
        }

        public int DeleteMilestone(Guid id)
        {
            var milestone = FindMilestone(id);
            var currentMilestone = _groupRepository.GetCurrentMilestone(milestone.GroupId);
            if (currentMilestone == milestone.Id)
                throw new Exception("Cannot delete current milestone");
            if(_milestoneRepository.DeleteMilestone(id) > 0){               
                return _milestoneRepository.SortOrderMilestone(milestone.GroupId, milestone.Order);
            }
            return 0;
        }

        public Milestone FindMilestone(Guid id)
        {
            var milestone = _milestoneRepository.FindMilestone(id);
            if (milestone == null) throw new Exception("This milestone doen't exit in application");
            return milestone;
        }

        public IEnumerable<Milestone> GetAllMilestone()
        {
            return _milestoneRepository.GetAllMilestone();
        }

        public MilestonesDTO GetMilestonesByGroupId(Guid userId, Guid groupId)
        {
            if (_memberRepository.FindByUserIdAndGroupId(userId, groupId) == null)
                throw new Exception("User not belong to group or 1 between member or group is not exist.");

            return _milestoneRepository.GetMilestonesByGroupId(groupId);
        }

        public Guid UpdateMilestone(MilestoneDTOForUpdating milestione)
        {
            FindMilestone(milestione.Id);
            return _milestoneRepository.UpdateMilestone(milestione);
        }

        public Guid? UpdateCurrentMilestone(MilestoneDTOForUpdatingCurrentOrder milestone, Guid userId)
        {
            
            var member = _memberRepository.FindByUserIdAndGroupId(userId, milestone.groupId);
            if (member == null)
                throw new Exception("User not belong to group or 1 between member or group is not exist.");
            if(member.Role != MemberRole.LEADER)
                throw new Exception("Only leader can do this.");
            var currentMelistoneID = _groupRepository.GetCurrentMilestone(milestone.groupId);
            var nextMilestone = _milestoneRepository.GetNextMilestoneByWishType((Guid)currentMelistoneID, milestone.groupId, milestone.wishType);
            return _groupRepository.UpdateCurrentMilestone(milestone.groupId, nextMilestone.Id);
        }
    }
}
