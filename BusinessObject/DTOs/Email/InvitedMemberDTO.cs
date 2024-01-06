using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.Email
{
    public class InvitedMemberDTO
    {
        public Guid userId { get; set; }
       public  string email { get; set; }
       public string inovationLink  { get; set; }
       public  string content { get; set; }
        public string receiverName { get; set; }
        public  string senderName { get; set; }
        public  string groupName { get; set; }

        public string groupLogo { get; set; }

    }
}
