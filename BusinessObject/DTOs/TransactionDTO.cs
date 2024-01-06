using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class TransactionDTO
    {
        
        public string Code  { get; set; }

        [Required]
        [Range(0, 1, ErrorMessage = "Transaction must be has status  WAITING,SUCCESS")]
        public TransactionStatus Status { get; set; }
    }
}
