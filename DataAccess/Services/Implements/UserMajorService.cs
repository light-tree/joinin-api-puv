using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class UserMajorService : IUserMajorService
    {
        private readonly IUserMajorRepository userMajorRepository;
        public UserMajorService(IUserMajorRepository userMajorRepository) { 
            this.userMajorRepository = userMajorRepository; 
        }
        public async Task<bool> UpdateUserMajor(Guid majorId, Guid userId)
        {
            return await userMajorRepository.UpdateUserMajor(majorId,userId);
        }
    }
}
