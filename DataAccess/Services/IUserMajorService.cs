using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services
{
    public interface IUserMajorService
    {
        public  Task<bool> UpdateUserMajor(Guid majorId, Guid userId);
    }
}
