using BusinessObject.Data;
using BusinessObject.Enums;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class MemberRepository : IMemberRepository
    {
        private readonly Context _context;

        public MemberRepository(Context context)
        {
            _context = context;
        }

        public Member CreateMember(Guid userId, Guid groupId, MemberRole role)
        {
            Member member = new Member
            {
                UserId = userId,
                GroupId = groupId,
                JoinedDate = DateTime.Now,
                Role = role
            };

            _context.Members.Add(member);
            _context.SaveChanges();
            return member;
        }

        public Member FindByIdAndGroupId(Guid id, Guid groupId)
        {
            return _context.Members.FirstOrDefault(m => m.Id == id && m.GroupId == groupId && m.LeftDate == null);
        }

        public Member FindByUserIdAndGroupId(Guid userId, Guid groupId)
        {
            return _context.Members.FirstOrDefault(m => m.UserId == userId && m.GroupId == groupId && m.LeftDate == null);
        }

        public List<Member> GetMembersByGroupId(Guid groupId, string? name)
        {
            BusinessObject.Models.Group group = _context.Groups.FirstOrDefault(g => g.Id == groupId);

            List<Member> members = _context.Members
                .Include(m => m.User)
                .Where(m => m.GroupId == groupId && m.LeftDate == null && (name != null ? m.User.FullName.ToUpper().Contains(name) : true))
                .ToList();

            List<Member> memberDetailList = new List<Member>();
            foreach(Member member in members)
            {
                List<ApplicationMajor> applicationMajors = new List<ApplicationMajor>();
                if (member.Id == group.CreatedById)
                {
                    applicationMajors = _context.UserMajors
                        .Include(um => um.Major)
                        .Where(um => um.UserId == member.UserId)
                        .Select(um => new ApplicationMajor
                        {
                            Major = new Major
                            {
                                Id = um.Major.Id,
                                Name = um.Major.Name,
                                ShortName = um.Major.ShortName,
                            },
                        }).ToList();
                }
                else
                {
                    applicationMajors = _context.Applications
                    .Include(a => a.ApplicationMajors).ThenInclude(am => am.Major)
                    .FirstOrDefault(a =>
                        a.GroupId == group.Id &&
                        a.UserId == member.UserId &&
                        (a.Status == ApplicationStatus.APPROVED || a.Status == ApplicationStatus.INVITE_APPROVED)
                     ).ApplicationMajors.Select(am => new ApplicationMajor
                     {
                         Major = new Major
                         {
                             Id = am.Major.Id,
                             Name = am.Major.Name,
                             ShortName = am.Major.ShortName,
                         }
                     }).ToList();

                    if (applicationMajors.Count == 0)
                        applicationMajors = _context.UserMajors
                            .Include(um => um.Major)
                            .Where(um => um.UserId == member.UserId).Select(am => new ApplicationMajor
                                {
                                    Major = new Major
                                    {
                                        Id = am.Major.Id,
                                        Name = am.Major.Name,
                                        ShortName = am.Major.ShortName,
                                    }
                                }).ToList();
                }

                User tmpUser = member.User;

                memberDetailList.Add(new Member
                {
                    Id = member.Id,
                    JoinedDate = member.JoinedDate,
                    User = new User
                    {
                        Id = tmpUser.Id,
                        FullName = tmpUser.FullName,
                        Avatar = tmpUser.Avatar,
                        Email= tmpUser.Email,
                        Applications = new List<Application>
                        {
                            new Application
                            {
                                ApplicationMajors = applicationMajors,
                            }
                        }
                    },
                    Role = member.Role,
                });
            }

            return memberDetailList;
        }

        public MemberRole? GetRoleInThisGroup(Guid userId, Guid groupId)
        {
            Member member = _context.Members.FirstOrDefault(m => m.GroupId == groupId && m.UserId == userId && m.LeftDate == null);
            return member != null ? member.Role : null ;
        }

        public Member UpdateMemberRole(Guid memberId, MemberRole role)
        {
            try
            {
                // Retrieve the member from the database by ID
                var member =  _context.Members.FirstOrDefault<Member>(m => m.Id == memberId);

                if (member == null)
                {
                    return null;
                }
              
                // Update the member's role property
                member.Role = role;


                // Save the changes to the database
                _context.Entry(member).State = EntityState.Modified;
                _context.SaveChanges();

                return member;
            }
            catch
            {
                return null;
            }

        } 

        public Member UpdateMember(Member member)
        {
            try
            {
                _context.Entry(member).State = EntityState.Modified;
                _context.SaveChanges();
                return member;
            } catch
            {
                return null;
            }

        }

        public int CountJoinedWorkingGroupAsMemberByUserId(Guid userId)
        {
            return _context.Members.Include(m => m.Group).Where(m => m.UserId == userId && m.LeftDate == null && m.Id != m.Group.CreatedById).Count();
        }


        public Member GetGroupLeader(Guid groupId)
        {
            var t= _context.Members.Include(g => g.User).Where(g => g.Role == MemberRole.LEADER && g.GroupId == groupId).FirstOrDefault();
            return t;
        }

        public List<Member> GetMembersToTotifyByTaskIdsExceptLeftMemberId(List<Guid> taskIdsOfDeletedAssignedTasks, Guid userId)
        {
            return _context.AssignedTasks
                .Include(a => a.AssignedFor).ThenInclude(m => m.User)
                .Where(a => taskIdsOfDeletedAssignedTasks.Contains(a.TaskId) && a.AssignedFor.UserId != userId)
                .Select(a => a.AssignedFor)
                .ToList();
        }

        public int LeaveGroup(Member member)
        {
            member.LeftDate = DateTime.Now;
            _context.Members.Update(member);
            return _context.SaveChanges();
        }

        public Member FindByMemberIdAndGroupId(Guid id, Guid groupId)
        {
            return _context.Members.Include(m => m.User).FirstOrDefault(m => m.Id == id && m.GroupId == groupId && m.LeftDate == null);
        }

        public Member FindByUserIdAndGroupIdIncludeEmail(Guid userId, Guid groupId)
        {
            return _context.Members.Include(m => m.User).FirstOrDefault(m => m.UserId == userId && m.GroupId == groupId && m.LeftDate == null);
        }

        public int DeleteByGroupId(Guid groupId)
        {
            List<Member> members = _context.Members
                .Where(m => m.GroupId == groupId)
                .ToList();

            _context.RemoveRange(members);
            return _context.SaveChanges();
        }

        public List<Member> GetAllMemberOfGroup(Guid groupId)
        {
            return _context.Members.Include(m => m.User).Where(m => m.GroupId == groupId).ToList();
        }
    }
}
