using DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Services.Implements
{
    public class PlatformUserCountService : IPlatformUserCountService
    {
        private readonly IPlatformUserCountRepository _platformUserCountRepository;

        public PlatformUserCountService(IPlatformUserCountRepository platformUserCountRepository)
        {
            _platformUserCountRepository = platformUserCountRepository;
        }

        public int IncreaseUserCountByPlatformName(string platformName)
        {
            return _platformUserCountRepository.IncreaseUserCountByPlatformName(platformName);
        }
    }
}
