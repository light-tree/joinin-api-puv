using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class MilestonesDTO
    {
        public List<Milestone> Milestones { get; set; }

        public int? CurrentMilestoneOrder { get; set; }
    }
}
