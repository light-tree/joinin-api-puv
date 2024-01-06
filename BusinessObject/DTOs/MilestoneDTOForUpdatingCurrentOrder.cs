using BusinessObject.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class MilestoneDTOForUpdatingCurrentOrder
    {

        [Required(ErrorMessage = "Group ID is required.")]
        public Guid groupId { get; set; }

        [Required(ErrorMessage = "Wish type is required.")]
        public WishType wishType { get; set; }

    }
}
