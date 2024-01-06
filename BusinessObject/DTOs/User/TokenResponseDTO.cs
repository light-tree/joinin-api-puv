using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs.User
{
    public class TokenResponseDTO
    {
        string token { get; set; }
        public TokenResponseDTO(string token) {
            this.token = token;
        }
       
    }
}
