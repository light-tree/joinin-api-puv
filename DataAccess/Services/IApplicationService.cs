using BusinessObject.DTOs;
using BusinessObject.DTOs.Common;
using BusinessObject.DTOs.Email;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IApplicationService
    {
        Guid? ConfirmApplication(Guid userId, ConfirmedApplicationDTO confirmedApplicationDTO);
        Guid? ConfirmInvitation(ConfirmedApplicationDTO confirmedInvitationDTO);
        Guid CreateApplication(Guid userId, SentApplicationDTO sentApplicationDTO);
        Task<List<InvitedMemberDTO>> CreateInvitations(Guid userId, SentInvitationsDTO sentInvitationsDTO);
        CommonResponse FilterApplications(Guid userId, Guid groupId, List<Guid>? majorIds, string? name, int? pageSize, int? page, string? orderBy, string? value);
        Application GetApplication(Guid applicationId);
        Application GetInvitationDetailByIdAndUserId(Guid applicationId, Guid userId);
    }
}
