using BusinessObject.Data;
using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using Google.Protobuf.WellKnownTypes;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class MajorRepository : IMajorRepository
    {
        Context _context;
        public MajorRepository(Context context)
        {
            _context = context;
        }

        public IEnumerable<Major> FindAll()
        {
            return _context.Majors.ToList();
        }

        public Major FindByID(Guid id)
        {
            try
            {
                return _context.Majors.FirstOrDefault(m => m.Id == id);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        static object GetPropertyValue(object obj, string propertyName)
        {
            var propertyInfo = obj.GetType().GetProperty(propertyName);
            return propertyInfo.GetValue(obj);
        }

        public CommonResponse FilterMajors(Guid? userId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            List<Major> majors = new List<Major>();
            if (userId == null)
            {
                majors = _context.Majors.Where(um => name != null ? (um.Name.ToUpper().Contains(name.ToUpper()) || um.ShortName.ToUpper().Contains(name.ToUpper())) : true).ToList();
            }
            else
            {
                majors = _context.UserMajors.Include(um => um.Major).Where(um => um.UserId == userId && (name != null ? um.Major.Name.ToUpper().Contains(name.ToUpper()) : true)).Select(um => um.Major).ToList();
            }

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    majors = majors.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    majors = majors.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }
            else
                majors = majors.OrderBy(t => t.Name).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            if (pageSize != null && page != null)
            {
                pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
                pagination.CurrentPage = page == null ? 1 : page.Value;
                pagination.Total = majors.Count;
                majors = majors.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            }

            List<MajorRecordDTO> majorRecordDTOs = majors.Select(m => new MajorRecordDTO
            {
                Id = m.Id,
                ShortName = m.ShortName,
                Name = m.Name,
            }).ToList();

            response.Data = majorRecordDTOs;
            response.Pagination = pagination;
            response.Message = "Filter major list success.";
            response.Status = 200;

            return response;
        }

        public CommonResponse FilterMajorsForApplicationViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            List<Major> majorsInWaitingApplications = _context.ApplicationMajors
                .Include(am => am.Major)
                .Include(am => am.Application)
                .Where(am => 
                    am.Application.GroupId == groupId && 
                    am.Application.Status == BusinessObject.Enums.ApplicationStatus.WAITING && 
                    (name != null ? (am.Major.Name.ToUpper().Contains(name.ToUpper()) || am.Major.ShortName.ToUpper().Contains(name.ToUpper())) : true))
                .Select(um => um.Major).ToList();

            List<Major> majorsInGroupMajors = _context.GroupMajors.Include(gm => gm.Major)
                .Where(um => 
                    um.GroupId == groupId && 
                    (name != null ? um.Major.Name.ToUpper().Contains(name.ToUpper()) : true))
                .Select(um => um.Major).ToList();

            List<Major> majors = majorsInWaitingApplications.Concat(majorsInGroupMajors)
                                       .GroupBy(m => m.Id)
                                       .Select(m => m.First())
                                       .ToList();

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    majors = majors.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    majors = majors.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }
            else
                majors = majors.OrderBy(t => t.Name).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            if (pageSize != null && page != null)
            {
                pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
                pagination.CurrentPage = page == null ? 1 : page.Value;
                pagination.Total = majors.Count;
                majors = majors.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            }

            List<MajorRecordDTO> majorRecordDTOs = majors.Select(m => new MajorRecordDTO
            {
                Id = m.Id,
                ShortName = m.ShortName,
                Name = m.Name,
            }).ToList();

            response.Data = majorRecordDTOs;
            response.Pagination = pagination;
            response.Message = "Filter major list success.";
            response.Status = 200;

            return response;
        }


        public CommonResponse FilterMajorsForGroupViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            List<Application> approvedApplicationsAndInvitations = _context.Applications
                .Include(a => a.ApplicationMajors).ThenInclude(am => am.Major)
                .Where(a =>
                    a.GroupId == groupId &&
                    a.Status == BusinessObject.Enums.ApplicationStatus.APPROVED)
                .ToList();

            List<Major> majorsInApprovedApplicationsAndInvitations = new List<Major>();
            foreach (Application application in approvedApplicationsAndInvitations)
            {
                if(application.ApplicationMajors.Count() == 0)
                {
                    //application is an Invitation
                    majorsInApprovedApplicationsAndInvitations.AddRange
                    (
                        _context.UserMajors
                        .Include(um => um.Major)
                        .Where(um => um.UserId == application.UserId)
                        .Select(um => um.Major).ToList()
                    );
                }
                else
                {
                    //application is an Application
                    majorsInApprovedApplicationsAndInvitations.AddRange(application.ApplicationMajors.Select(am => am.Major));
                }
            }

            majorsInApprovedApplicationsAndInvitations = majorsInApprovedApplicationsAndInvitations
                .Where(m => name != null ? (m.Name.ToUpper().Contains(name.ToUpper()) || m.ShortName.ToUpper().Contains(name.ToUpper())) : true).ToList();

            //List<Major> majorsInApprovedApplications = _context.ApplicationMajors
            //    .Include(am => am.Major)
            //    .Include(am => am.Application)
            //    .Where(am =>
            //        am.Application.GroupId == groupId &&
            //        am.Application.Status == BusinessObject.Enums.ApplicationStatus.APPROVED &&
            //        (name != null ? am.Major.Name.ToUpper().Contains(name.ToUpper()) : true))
            //    .Select(um => um.Major).ToList();

            List<Major> majorsInGroupMajors = _context.GroupMajors.Include(gm => gm.Major)
                .Where(um =>
                    um.GroupId == groupId &&
                    (name != null ? (um.Major.Name.ToUpper().Contains(name.ToUpper()) || um.Major.ShortName.ToUpper().Contains(name.ToUpper())) : true))
                .Select(um => um.Major).ToList();

            List<Major> majors = majorsInApprovedApplicationsAndInvitations.Concat(majorsInGroupMajors)
                .GroupBy(m => m.Id)
                .Select(m => m.First())
                .ToList();

            Guid? userIdOfGroupCreater = _context.Members
                .Include(m => m.Group)
                .FirstOrDefault(m =>
                    m.GroupId == groupId &&
                    m.LeftDate == null &&
                    m.Group.CreatedById == m.Id).UserId;

            if (userIdOfGroupCreater != null)
            {
                majors = majors.Concat(_context.UserMajors
                    .Include(um => um.Major)
                    .Where(um => um.UserId == userIdOfGroupCreater)
                    .Select(um => um.Major))
                    .GroupBy(m => m.Id)
                    .Select(m => m.First())
                    .ToList();
            }

            if (orderBy != null && value != null && (value.ToLower().Equals("asc") || value.ToLower().Equals("des")))
            {
                if (value.ToLower().Equals("asc"))
                    majors = majors.OrderBy(t => GetPropertyValue(t, orderBy)).ToList();
                else
                    majors = majors.OrderByDescending(t => GetPropertyValue(t, orderBy)).ToList();
            }
            else
                majors = majors.OrderBy(t => t.Name).ToList();

            CommonResponse response = new CommonResponse();
            Pagination pagination = new Pagination();
            if (pageSize != null && page != null)
            {
                pagination.PageSize = pageSize == null ? 10 : pageSize.Value;
                pagination.CurrentPage = page == null ? 1 : page.Value;
                pagination.Total = majors.Count;
                majors = majors.Skip((pagination.CurrentPage - 1) * pagination.PageSize).Take(pagination.PageSize).ToList();
            }

            List<MajorRecordDTO> majorRecordDTOs = majors.Select(m => new MajorRecordDTO
            {
                Id = m.Id,
                ShortName = m.ShortName,
                Name = m.Name,
            }).ToList();

            response.Data = majorRecordDTOs;
            response.Pagination = pagination;
            response.Message = "Filter major list success.";
            response.Status = 200;

            return response;
        }

        public Guid UpdateMajor(MajorDTOForUpdate major)
        {
            var majorFound = FindByID(major.Id);
            majorFound.Name = major.Name;
            majorFound.ShortName = major.ShortName;
            _context.Majors.Update(majorFound);
            _context.SaveChanges();
            return majorFound.Id;
        }

        public Guid CreateMajor(MajorDTOForCreate major)
        {
            BusinessObject.Models.Major MajorCreate = new BusinessObject.Models.Major();
            MajorCreate.Name = major.Name;
            MajorCreate.ShortName = major.ShortName;
            _context.Majors.Add(MajorCreate);
            _context.SaveChanges();
            return MajorCreate.Id;
        }

      
    }
}
