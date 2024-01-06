using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IApplicationRepository
    {
        Application CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO);
        Application FindById(Guid applicationId);
        Guid? ConfirmApplication(ConfirmedApplicationDTO confirmedApplicationDTO);
        Application CreateInvitation(Guid groupId, string? description, Guid invitedUserId);
        Application FindWaitingOrInvitingByUserIdAndGroupId(Guid userId, Guid groupId);
        CommonResponse FilterApplications(Guid groupId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value);
        Application FindWaitingInvitationByIdAndUserId(Guid applicationId, Guid userId);
    }
}
