using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace JOBPORTALWEB.APPLICATION.DTOs.Company
{
    public class CreateCompanyRequest
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }
    }
}