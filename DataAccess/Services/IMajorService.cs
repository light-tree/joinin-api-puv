using BusinessObject.DTOs.Common;
﻿using BusinessObject.DTOs;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IMajorService
    {
        CommonResponse FilterMajors(Guid? userId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterMajorsForApplicationViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        CommonResponse FilterMajorsForGroupViewForLeader(Guid groupId, string? name, int? pageSize, int? page, string? orderBy, string? value);
        Major FindMajorById(Guid id);

        IEnumerable<Major> GetAllMajors();

        Guid UpdateMajor(MajorDTOForUpdate major);

        Guid CreateMajor(MajorDTOForCreate major);
     
    }
}
