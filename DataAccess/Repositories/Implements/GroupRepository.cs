using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;


namespace DataAccess.Repositories.Implements
{
    public class GroupRepository : IGroupRepository
    {
        private readonly Context _context;

        public GroupRepository(Context context)
        {
            _context = context;
        }

        public int AssignCreater(Member member, Group group)
        {
            group.CreatedById = member.Id;
            _context.Update(group);
            return _context.SaveChanges();
        }

        public Group CreateGroup(Guid userId ,GroupDTOForCreating groupDTOForCreating)
        {
            User user = _context.Users.FirstOrDefault(u => u.Id == userId);
            bool isPremiumAccount = user.EndDatePremium != null ? ((user.EndDatePremium.Value > DateTime.Now) ? true : false) : false;

            Group group = new Group
            {
                Name = groupDTOForCreating.Name,
                CreatedDate = DateTime.Now,
                GroupSize = isPremiumAccount ? BusinessRuleData.GROUP_SIZE_FOR_PREMIUM : BusinessRuleData.GROUP_SIZE_FOR_FREEMIUM,
                MemberCount = 1,
                SchoolName = groupDTOForCreating.SchoolName,
                SubjectName = groupDTOForCreating.SubjectName,
                ClassName = groupDTOForCreating.ClassName,
                Description = groupDTOForCreating.Description,
                Skill = groupDTOForCreating.Skill,
                Avatar = groupDTOForCreating.Avatar,
                Theme = groupDTOForCreating.Theme,
                Status = BusinessObject.Enums.GroupStatus.ACTIVE
            };

            _context.Groups.Add(group);
            _context.SaveChanges();
            return group;
        }

        public CommonResponse FilterGroupsToApply(Guid userId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            if (!(majorIds != null && majorIds.Count > 0))
                //majorIds = _context.UserMajors.Where(um => um.UserId  == userId).Select(um => um.MajorId).ToList();
                majorIds = new List<Guid>();


            List <Group> groups = _context.Groups
                .Include(g => g.GroupMajors).ThenInclude(gm => gm.Major)
                .Include(g => g.Members)
                .Select(g => new Group
                {
                    Id = g.Id,
                    Name = g.Name,
                    SchoolName = g.SchoolName,
                    ClassName = g.ClassName,
                    SubjectName = g.SubjectName,
                    Description = g.Description,
                    CreatedDate = g.CreatedDate,
                    Theme = g.Theme,
                    Avatar = g.Avatar,
                    Status = g.Status,
                    GroupSize = g.GroupSize,
                    MemberCount = g.MemberCount,
                    Members = g.Members.Select(m => new Member
                    {
                        UserId = m.UserId
                    }).ToList(),
                    GroupMajors = g.GroupMajors.Select(gm => new GroupMajor
                    {
                        MajorId = gm.MajorId,
                        MemberCount = gm.MemberCount,
                        Major = gm.Major
                    })
                    .Where(gm => gm.MemberCount > 0)
                    .ToList(),
                })
                .Where(g => 
                    (!g.Members.Any(m => m.UserId == userId)) &&
                    g.Status == BusinessObject.Enums.GroupStatus.ACTIVE &&
                    g.GroupMajors.Any(gm => gm.MemberCount > 0) &&
                    (majorIds.Count() > 0 ? (g.GroupMajors.Any(gm => majorIds.Contains(gm.Major.Id))) : true) &&
                    (name != null ?
                        (
                            g.Name.ToUpper().Contains(name.ToUpper()) ||
                            (g.SchoolName != null ? g.SchoolName.ToUpper().Contains(name.ToUpper()) : false) ||
                            (g.ClassName != null ? g.ClassName.ToUpper().Contains(name.ToUpper()) : false) ||
                            (g.SubjectName != null ? g.SubjectName.ToUpper().Contains(name.ToUpper()) : false)
                        ) : true
                    )
                )
                .ToList();

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    groups = groups.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    groups = groups.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = groups.Count;
            groups = groups.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();

            foreach(Group group in groups)
            {
                List<Guid> userIds = group.Members.Select(m => m.UserId).ToList();

                group.Members = _context.Users
                    .Where(u => userIds.Contains(u.Id))
                    .Select(u => new Member
                    {
                        User = new User
                        {
                            FullName = u.FullName,
                            Avatar = u.Avatar,
                        }
                    }).ToList();
            }

            response.Data = groups;
            response.Pagination = pagination;
            response.Message = "Filter group list success.";
            response.Status = 200;

            return response;
        }

        public Group FindById(Guid groupId)
        {
            return _context.Groups.Include(g => g.CreatedBy).FirstOrDefault(g => g.Id == groupId && g.Status == BusinessObject.Enums.GroupStatus.ACTIVE);
        }

        public Group? IncreaseCurrentMemberCount(Guid groupId, int v)
        {
            Group group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            group.MemberCount += v;
            if (group.MemberCount > group.GroupSize)
                throw new Exception("This group already reach max group size that was set up.");
            _context.Update(group);
            if(_context.SaveChanges() != 1) throw new Exception("Increase current member count fail.");
            return group;
        }

        public Group DecreaseCurrentMemberCount(Guid groupId, int v)
        {
            Group group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            group.MemberCount -= v;
            _context.Update(group);
            if (_context.SaveChanges() != 1) throw new Exception("Decrease current member count fail.");
            return group;
        }

        public Group IncreaseCurrentMemberCountByInvitation(Guid groupId, int v)
        {
            Group group = _context.Groups.FirstOrDefault(g => g.Id == groupId);
            group.MemberCount += v;
            if (group.MemberCount > group.GroupSize)
                group.GroupSize = group.MemberCount;
            _context.Update(group);
            if (_context.SaveChanges() != 1) throw new Exception("Increase current member count fail.");
            return group;
        }

        public Guid? UpdateCurrentMilestone(Guid groupID, Guid milestoneID)
        {
            var group = FindById(groupID);
            group.CurrentMilestoneId = milestoneID;
            _context.SaveChanges();
            return group.CurrentMilestoneId;
        }

        static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj);
        }

        public Group GetDetailById(Guid groupId)
        {
            Group group = _context.Groups
                .Include(g => g.CreatedBy).ThenInclude(cb => cb.User)
                .FirstOrDefault(g => g.Id == groupId && g.Status == BusinessObject.Enums.GroupStatus.ACTIVE);

            Member leader = _context.Members.Include(m => m.User)
                .FirstOrDefault(m => m.Role == BusinessObject.Enums.MemberRole.LEADER && m.GroupId == groupId && m.LeftDate == null);

            return new Group
            {
                Id = group.Id,
                Name = group.Name,
                CreatedDate = DateTime.Now,
                CreatedById = group.CreatedById,
                CreatedBy = new Member
                {
                    Id = group.CreatedById.Value,
                    User = new User 
                    {
                        Id = group.CreatedBy.User.Id,
                        FullName = group.CreatedBy.User.FullName,
                        Avatar = group.CreatedBy.User.Avatar,
                    },
                    Role = group.CreatedBy.Role
                },
                Members = new List<Member>
                {
                    new Member
                    {
                    Id = leader.Id,
                    User = new User
                    {
                        Id = leader.User.Id,
                        FullName = leader.User.FullName,
                        Avatar = leader.User.Avatar,
                    },
                    Role = leader.Role,
                }
                },
                GroupSize = group.GroupSize,
                MemberCount = group.MemberCount,
                SchoolName = group.SchoolName,
                ClassName = group.ClassName,
                SubjectName = group.SubjectName,
                Description = group.Description,
                Skill = group.Skill,
                Avatar = group.Avatar,
                Theme = group.Theme,
                Status = group.Status,
            };
        }

        public bool IsStillJoinableWithUserType(Guid userId)
        {
            throw new NotImplementedException();
        }

        public CommonResponse FilterGroups(Guid userId, string? name, string? type, int? pageSize, int? page, string? orderBy, string? value)
        {
            if (type != null && type != "Owned" && type != "Joined")
                throw new Exception("type must be null, \"Owned\" or \"Joined\"");

            List<Group> groups = _context.Groups
                .Include(g => g.CreatedBy)
                .Include(g => g.Members)
                .Select(g => new Group
                {
                    Id = g.Id,
                    Name = g.Name,
                    SchoolName = g.SchoolName,
                    ClassName = g.ClassName,
                    CreatedDate = g.CreatedDate,
                    SubjectName = g.SubjectName,
                    Avatar = g.Avatar,
                    Status = g.Status,
                    GroupSize = g.GroupSize,
                    CreatedBy = new Member
                    {
                        UserId = g.CreatedBy.UserId
                    },
                    Members = g.Members.Select(m => new Member
                    {
                        UserId = m.UserId,
                        LeftDate = m.LeftDate
                    }).ToList(),
                    MemberCount = g.MemberCount
                    })
                .Where(g =>
                    (g.Members.Any(m => m.UserId == userId && m.LeftDate == null)) &&
                    g.Status == BusinessObject.Enums.GroupStatus.ACTIVE &&
                    (type == null ? true : (type == "Owned" ? (g.CreatedBy.UserId == userId) : (g.CreatedBy.UserId != userId))) &&
                    (name != null ?
                        (
                            g.Name.ToUpper().Contains(name.ToUpper()) ||
                            (g.SchoolName != null ? g.SchoolName.ToUpper().Contains(name.ToUpper()) : false) ||
                            (g.ClassName != null ? g.ClassName.ToUpper().Contains(name.ToUpper()) : false) ||
                            (g.SubjectName != null ? g.SubjectName.ToUpper().Contains(name.ToUpper()) : false)
                        ) : true
                    )
                ).ToList();

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    groups = groups.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    groups = groups.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = groups.Count;
            groups = groups.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();

            foreach (Group group in groups)
            {
                group.Members = new List<Member> {_context.Members
                            .Include(m => m.User)
                            .FirstOrDefault(m => m.GroupId == group.Id && m.Role == BusinessObject.Enums.MemberRole.LEADER)}
                        .Select(m => new Member
                        {
                            Id = m.Id,
                            Role = m.Role,
                            User = new User
                            {
                                Id = m.User.Id,
                                FullName = m.User.FullName,
                                Avatar = m.User.Avatar
                            }
                        }).ToList();
            }

            response.Data = groups;
            response.Pagination = pagination;
            response.Message = "Filter group list success.";
            response.Status = 200;

            return response;
        }

        public Guid? GetCurrentMilestone(Guid groupID)
        {
            return _context.Groups.Where(g => g.Id == groupID).FirstOrDefault().CurrentMilestoneId;
        }

        public int UpdateGroup(GroupDTOForUpdating groupDTOForUpdating)
        {
            Group group = _context.Groups.FirstOrDefault(g => g.Id == groupDTOForUpdating.Id);
            if (groupDTOForUpdating.Name != null) group.Name = groupDTOForUpdating.Name;
            if (groupDTOForUpdating.SchoolName != null) group.SchoolName = groupDTOForUpdating.SchoolName;
            if (groupDTOForUpdating.SubjectName != null) group.SubjectName = groupDTOForUpdating.SubjectName;
            if (groupDTOForUpdating.ClassName != null) group.ClassName = groupDTOForUpdating.ClassName;
            if (groupDTOForUpdating.Description != null) group.Description = groupDTOForUpdating.Description;
            if (groupDTOForUpdating.Skill != null) group.Skill = groupDTOForUpdating.Skill;
            if (groupDTOForUpdating.Avatar != null) group.Avatar = groupDTOForUpdating.Avatar;
            if (groupDTOForUpdating.Theme != null) group.Theme = groupDTOForUpdating.Theme;

            _context.Update(group);
            return _context.SaveChanges();
        }

        public int CountGroupByCreatedById(Guid createrId)
        {
            return _context.Groups.Include(g => g.CreatedBy).Where(g => g.CreatedBy.UserId == createrId && g.Status == BusinessObject.Enums.GroupStatus.ACTIVE).Count();
        }

        public int UpdateGroupSizeAsPremiumByCreaterId(Guid userId)
        {
            List<Group> groups = _context.Groups.Where(g => g.CreatedById == userId && g.Status == BusinessObject.Enums.GroupStatus.ACTIVE).ToList();
            foreach (Group group in groups)
            {
                group.GroupSize = BusinessRuleData.GROUP_SIZE_FOR_PREMIUM;
            }
            _context.UpdateRange(groups);
            return _context.SaveChanges();
        }

        public int InactivateGroup(Group group)
        {
            group.CreatedById = null;
            group.CurrentMilestoneId = null;
            group.Status = BusinessObject.Enums.GroupStatus.INACTIVE;

            _context.Groups.Update(group);
            return _context.SaveChanges();
        }
    }
}
