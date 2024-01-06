using BusinessObject.DTOs.Common;
using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IMajorRepository
    {
        CommonResponse FilterMajors(Guid? userId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterMajorsForApplicationViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterMajorsForGroupViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        Major FindByID(Guid id);

        IEnumerable<Major> FindAll();

        Guid UpdateMajor(MajorDTOForUpdate major);

        Guid CreateMajor(MajorDTOForCreate major);
      
    }
}
