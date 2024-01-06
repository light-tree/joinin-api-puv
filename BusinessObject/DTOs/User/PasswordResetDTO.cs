using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class PasswordResetDTO
    {
        public string password { get; set; }
        public string verifyToken { get; set; }
    }
}
