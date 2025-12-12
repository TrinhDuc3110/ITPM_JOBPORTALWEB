using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.DTOs.User
{
    public class UpdateProfileRequest
    {
        public string? Headline { get; set; }
        public string? Bio { get; set; }
        public string? Skills { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? Location { get; set; }
    }
}