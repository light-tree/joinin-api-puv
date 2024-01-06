using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Enums;
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
    public class ApplicationRepository : IApplicationRepository
    {
        private readonly Context _context;

        public ApplicationRepository(Context context)
        {
            _context = context;
        }

        public Application CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO)
        {
            Application application = new Application
            {
                CreatedDate = DateTime.Now,
                UserId = userId,
                Status = BusinessObject.Enums.ApplicationStatus.WAITING,
                Description = sentApplicationDTO.Description,
                GroupId = sentApplicationDTO.GroupId
            };
            _context.Applications.Add(application);
            _context.SaveChanges();
            return application;
        }

        public Application FindById(Guid id)
        {
            return _context.Applications.Include(p => p.Group).FirstOrDefault(a => a.Id == id);
        }

        public Guid? ConfirmApplication(ConfirmedApplicationDTO confirmedApplicationDTO)
        {
            Application application = FindById(confirmedApplicationDTO.ApplicationId);
            application.Status = confirmedApplicationDTO.Status;
            application.ConfirmedDate = DateTime.Now;
            _context.Update(application);
            if (_context.SaveChanges() != 1)
                return null;
            else
                return application.Id;
        }

        public Application CreateInvitation(Guid groupId, string? description, Guid invitedUserId)
        {
            Application invitation = new Application
            {
                CreatedDate = DateTime.Now,
                UserId = invitedUserId,
                Status = ApplicationStatus.INVITING,
                Description = description,
                GroupId = groupId
            };
            _context.Applications.Add(invitation);
            _context.SaveChanges();
            return invitation;
        }

        public Application FindWaitingOrInvitingByUserIdAndGroupId(Guid userId, Guid groupId)
        {
            return _context.Applications.FirstOrDefault(
                a =>
                    a.UserId == userId
                    && a.GroupId == groupId
                    && (
                        a.Status == ApplicationStatus.WAITING
                        || a.Status == ApplicationStatus.INVITING
                    )
            );
        }

        public CommonResponse FilterApplications(
            Guid groupId,
            List<Guid>? majorIds,
            string? name,
            int? pageSize,
            int? page,
            string? orderBy,
            string? value
        )
        {
            List<Application> applications = _context.Applications
                .Include(a => a.User)
                .Include(a => a.ApplicationMajors)
                .ThenInclude(am => am.Major)
                .Select(
                    a =>
                        new Application
                        {
                            Id = a.Id,
                            GroupId = a.GroupId,
                            UserId = a.UserId,
                            CreatedDate = a.CreatedDate,
                            Status = a.Status,
                            Description = a.Description,
                            ConfirmedDate = a.ConfirmedDate,
                            User = new User { FullName = a.User.FullName, Avatar = a.User.Avatar, },
                            ApplicationMajors = a.ApplicationMajors
                                .Select(
                                    am =>
                                        new ApplicationMajor
                                        {
                                            MajorId = am.MajorId,
                                            Major = new Major
                                            {
                                                Id = am.Major.Id,
                                                Name = am.Major.Name,
                                                ShortName = am.Major.ShortName,
                                            },
                                        }
                                )
                                .ToList(),
                        }
                )
                .Where(
                    a =>
                        a.GroupId == groupId
                        && (
                            name != null ? a.User.FullName.ToUpper().Contains(name.ToUpper()) : true
                        )
                        && (
                            (majorIds != null && majorIds.Count() > 0)
                                ? a.ApplicationMajors.Any(am => majorIds.Contains(am.Major.Id))
                                //(a.ApplicationMajors.Select(am => am.Major.Id).Intersect(majorIds).Count() > 0)
                                //majorIds.Any(majorId => a.ApplicationMajors.Any(am => am.MajorId == majorId))
                                : true
                        )
                        && a.Status != ApplicationStatus.INVITING
                        && a.Status != ApplicationStatus.INVITE_APPROVED
                        && a.Status != ApplicationStatus.INVITE_DISAPPROVED
                )
                .ToList();

            if (
                orderBy != null
                && value != null
                && (value.ToLower().Equals("asc") || value.ToLower().Equals("des"))
            )
            {
                if (value.ToLower().Equals("asc"))
                    applications = applications.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    applications = applications
                        .OrderByDescending(t => GetPropertyValue(t, orderBy))
                        .ToList();
            }
            else
                applications = applications.OrderByDescending(t => t.CreatedDate).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
            pagination.CurrentPage = page == null ? 1 : page.Value;
            pagination.Total = applications.Count;
            applications = applications
                .Skip((pagination.CurrentPage - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToList();

            response.Data = applications;
            response.Pagination = pagination;
            response.Message = "Filter application list success.";
            response.Status = 200;

            return response;
        }

        static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj);
        }

        public Application FindWaitingInvitationByIdAndUserId(Guid applicationId, Guid userId)
        {
            return _context.Applications
                .Select(
                    a =>
                        new Application
                        {
                            Id = a.Id,
                            Description = a.Description,
                            CreatedDate = a.CreatedDate,
                            Status = a.Status,
                            UserId = a.UserId,
                            GroupId = a.GroupId
                        }
                )
                .FirstOrDefault(
                    a =>
                        a.Id == applicationId
                        && a.UserId == userId
                        && a.Status == ApplicationStatus.INVITING
                );
        }
    }
}
