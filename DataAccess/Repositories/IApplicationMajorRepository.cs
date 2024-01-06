using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IApplicationMajorRepository
    {
        List<ApplicationMajor> CreateApplicationMajors(Guid newApplicationId, List<Guid> majorIds);
        List<ApplicationMajor> CreateApplicationMajorsUsingInvitation(Guid userId, Guid invitationid);
        List<ApplicationMajor> FindByApplicationId(Guid applicationId);
    }
}
