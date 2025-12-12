using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.DTOs.Auth
{
    public class ForgotPasswordRequest
    {
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Base URL của Frontend nơi người dùng sẽ được chuyển hướng
        [Required]
        public string ResetUrlBase { get; set; } = string.Empty;
    }
}
