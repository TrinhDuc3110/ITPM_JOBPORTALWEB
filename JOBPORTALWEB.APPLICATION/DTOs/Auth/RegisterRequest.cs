using JOBPORTALWEB.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.DTOs.Auth
{
    public class RegisterRequest
    {
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string FullName { set; get; } = string.Empty;
        [Required]
        public UserRole Role { get; set; }

    }
}
