using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class Company
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string? Website { get; set; }
        public string? Description { get; set; }
        public string? Location { get; set; }

        // Liên kết với các Jobs
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}