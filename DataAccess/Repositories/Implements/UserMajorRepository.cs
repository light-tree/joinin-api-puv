using BusinessObject.Data;
using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class UserMajorRepository : IUserMajorRepository
    {
        Context _context;
        public UserMajorRepository(Context context)
        {
            this._context = context;
        }
        public Task<bool> assignMajorToUser(List<UserMajor> userMajors)
        {
            return null;
        }

        public List<UserMajor> FindByUserId(Guid userId)
        {
            return _context.UserMajors.Where(um => um.UserId == userId).ToList();
        }

        public async Task<bool> UpdateUserMajor(Guid majorId, Guid userId)
        {
            try
            {
                var userMajor = await _context.UserMajors.FirstOrDefaultAsync(um => um.MajorId == majorId && um.UserId == userId);

                if (userMajor == null)
                {
                    _context.UserMajors.Add(new UserMajor { MajorId = majorId, UserId = userId });
                    _context.SaveChanges();
                    return true;
                }
                return false;
               
            }
            catch
            {
                return false;
            }
        }
    }
}
