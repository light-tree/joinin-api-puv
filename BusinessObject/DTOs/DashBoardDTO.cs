using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class DashBoardDTO
    {
        public long TotalRevenue { get; set; }

        public long TotalUser { get; set;}

        public long TotalFreemiumUser { get; set; }

        public long TotalPremiumUser { get; set; }

        public float TotalUserGrownPercentLastWeek { get; set; }

        public List<int> FreeUserCount { get; set; }

        public List<int> PreUserCount { get; set; }

        public List<int> ActiveUserCount { get; set; }

        public int FacebookUser { get; set; }

        public int TiktokUser { get; set; }
        
        public int UnknownUser { get; set; }

        public List<int> GroupCount { get; set; }
    }
}
