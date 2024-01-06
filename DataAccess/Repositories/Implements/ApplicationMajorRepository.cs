using BusinessObject.Data;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class ApplicationMajorRepository : IApplicationMajorRepository
    {
        private readonly Context _context;

        public ApplicationMajorRepository(Context context)
        {
            _context = context;
        }

        public List<ApplicationMajor> CreateApplicationMajors(
            Guid newApplicationId,
            List<Guid> majorIds
        )
        {
            List<ApplicationMajor> applicationMajors = new List<ApplicationMajor>();
            foreach (Guid majorId in majorIds)
            {
                applicationMajors.Add(
                    new ApplicationMajor { ApplicationId = newApplicationId, MajorId = majorId }
                );
            }
            _context.AddRange(applicationMajors);
            _context.SaveChanges();
            return applicationMajors;
        }

        public List<ApplicationMajor> CreateApplicationMajorsUsingInvitation(
            Guid userId,
            Guid invitationid
        )
        {
            return CreateApplicationMajors(
                invitationid,
                _context.UserMajors
                    .Where(um => um.UserId == userId)
                    .Select(um => um.MajorId)
                    .ToList()
            );
        }

        public List<ApplicationMajor> FindByApplicationId(Guid applicationId)
        {
            return _context.ApplicationMajors
                .Where(am => applicationId == am.ApplicationId)
                .ToList();
        }
    }
}
