using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories
{
    public interface IUserMajorRepository
    {
        public Task<bool> assignMajorToUser(List<UserMajor> userMajors);
        List<UserMajor> FindByUserId(Guid userId);
        public Task<bool> UpdateUserMajor(Guid majorId, Guid userId);
    }
}
