using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class UserFilterDTO
    {

        public string name { get; set; }
        public string email { get; set; }

        public int pageSize { get; set; }

        public int currentPage { get; set; }

    }
}
