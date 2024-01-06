using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObject.DTOs
{
    public class MilestoneDTOForUpdating
    {
        [Required(ErrorMessage = "Milestone ID is required.")]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Milestone name is required.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        public string Description { get; set; }
    }
}
