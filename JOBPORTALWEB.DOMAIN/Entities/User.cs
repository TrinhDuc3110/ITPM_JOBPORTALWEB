using JOBPORTALWEB.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string FullName { get; set; } = string.Empty;
        public UserRole Role { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
        public UserProfile? UserProfile { get; set; }
        public bool IsActive { get; set; } = true;

    }
}
