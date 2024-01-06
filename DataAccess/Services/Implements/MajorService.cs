using BusinessObject.DTOs.Common;
using BusinessObject.DTOs;
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
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;

        public MajorService(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }

        public CommonResponse FilterMajors(Guid? userId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            return _majorRepository.FilterMajors(userId, name, pageSize, page, orderBy, value);
        }

        public CommonResponse FilterMajorsForApplicationViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            return _majorRepository.FilterMajorsForApplicationViewForLeader(groupId, name, pageSize, page, orderBy, value);
        }

        public CommonResponse FilterMajorsForGroupViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value)
        {
            return _majorRepository.FilterMajorsForGroupViewForLeader(groupId, name, pageSize, page, orderBy, value);
        }

        public Major FindMajorById(Guid id)
        {
            var rs = _majorRepository.FindByID(id); ;
            return rs;

        }

        public IEnumerable<Major> GetAllMajors()
        {
            return _majorRepository.FindAll();
        }

        public Guid UpdateMajor(MajorDTOForUpdate major)
        {
            if (FindMajorById(major.Id) == null) throw new Exception("This major doesn't exist in application");
            return _majorRepository.UpdateMajor(major);
        }

        public Guid CreateMajor(MajorDTOForCreate major)
        {           
            return _majorRepository.CreateMajor(major);
        }

       
    }
}
