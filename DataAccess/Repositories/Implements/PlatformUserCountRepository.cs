using BusinessObject.Data;
using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Repositories.Implements
{
    public class PlatformUserCountRepository : IPlatformUserCountRepository
    {
        private readonly Context _context;

        public PlatformUserCountRepository(Context context)
        {
            _context = context;
        }

        public int IncreaseUserCountByPlatformName(string platformName)
        {
            PlatformUserCount? platformUserCount = _context.PlatformUserCounts
                .FirstOrDefault(puc => puc.PlatformName.ToUpper() == platformName.ToUpper());
            if (platformUserCount == null)
                throw new Exception("Platform not found");
            platformUserCount.UserCount += 1;
            _context.PlatformUserCounts.Update(platformUserCount);
            _context.SaveChanges();
            return platformUserCount.UserCount;
        }
    }
}
