using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class GroupMajorRepository : IGroupMajorRepository
    {
        private readonly Context _context;

        public GroupMajorRepository(Context context)
        {
            _context = context;
        }

        public GroupMajor CreateGroupMajor(Guid groudId, GroupMajorDTO groupMajorDTO)
        {
            GroupMajor groupMajor = new GroupMajor
            {
                MajorId = groupMajorDTO.MajorId,
                GroupId = groudId,
                MemberCount = groupMajorDTO.MemberCount
            };

            _context.GroupMajors.Add(groupMajor);
            if (_context.SaveChanges() != 1)
                throw new Exception("Create recruiting not success.");
            return groupMajor;
        }

        public int DecreaseCurrentNeededMemberCount(GroupMajor groupMajor, int v)
        {
            groupMajor.MemberCount -= v;
            if (groupMajor.MemberCount <= 0)
                return DeleteByGroupIdAndMajorId(groupMajor.GroupId, groupMajor.MajorId);
            else
            {
                _context.GroupMajors.Update(groupMajor);
                return _context.SaveChanges();
            }
        }

        public int DeleteByGroupIdAndMajorId(Guid groupId, Guid majorId)
        {
            GroupMajor groupMajor = FindByGroupIdAndMajorId(groupId, majorId);
            if (groupMajor == null) 
                throw new Exception("Recruiting does not exist.");
            _context.GroupMajors.Remove(groupMajor);
            return _context.SaveChanges();
        }

        public List<GroupMajor> FindByGroupId(Guid groupId)
        {
            return _context.GroupMajors.Where(gm => gm.GroupId == groupId).ToList();
        }

        public GroupMajor FindByGroupIdAndMajorId(Guid groupId, Guid majorId)
        {
            return _context.GroupMajors.FirstOrDefault(gm => gm.GroupId == groupId && gm.MajorId == majorId);
        }

        public List<GroupMajor> GetRecruitingGroupMajorsByGroupId(Guid groupId)
        {
            return _context.GroupMajors
                .Include(gm => gm.Major)
                .Where(gm => gm.GroupId == groupId)
                .Select(gm => new GroupMajor
                {
                    MemberCount = gm.MemberCount,
                    Major = new Major
                    {
                        Id = gm.Major.Id,
                        Name = gm.Major.Name,
                        ShortName = gm.Major.ShortName,
                    }
                })
                .ToList();
        }

        public int UpdateGroupMajor(Guid groupId, GroupMajorDTO groupMajorDTO)
        {
            GroupMajor groupMajor = FindByGroupIdAndMajorId(groupId, groupMajorDTO.MajorId);
            groupMajor.MemberCount = groupMajorDTO.MemberCount;
            _context.GroupMajors.Update(groupMajor);
            return _context.SaveChanges();
        }
    }
}
