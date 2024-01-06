using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class GroupDTOForToDoTaskRecord
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Avatar { get; set; }
    }
}
