using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class PasswordUpdateDTO
    {
       public  string password { get; set; }
       public string verifyCode { get; set; }
       
    }
}
