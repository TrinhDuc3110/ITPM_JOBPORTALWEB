using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class UserProfile
    {
        [Key]
        public Guid UserId { get; set; }

        public string? Headline { get; set; } 
        public string? Bio { get; set; }
        public string? Skills { get; set; }
        public string? Education { get; set; }
        public string? Experience { get; set; }
        public string? ResumeUrl { get; set; }
        public string? Location { get; set; }

        // Navigation Property về User gốc
        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }
}