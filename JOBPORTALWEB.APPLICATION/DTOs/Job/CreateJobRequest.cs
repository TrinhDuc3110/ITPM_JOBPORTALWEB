using System.ComponentModel.DataAnnotations;
using JOBPORTALWEB.DOMAIN.Enums;


namespace JOBPORTALWEB.APPLICATION.DTOs.Job
{
    public class CreateJobRequest
    {
        [Required] public string Title { get; set; } = string.Empty;
        [Required] public string Description { get; set; } = string.Empty;

        // Location
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsRemote { get; set; }

        // Salary
        public decimal SalaryMin { get; set; }
        public decimal SalaryMax { get; set; }
        public SalaryType SalaryType { get; set; }

        // Details
        public JobType JobType { get; set; }
        public JobLevel JobLevel { get; set; }
        public int Vacancies { get; set; }
        public string? Experience { get; set; }
        public string? Education { get; set; }
        public string? Tags { get; set; } // "Tag1, Tag2"
        public string? Benefits { get; set; }

        public DateTime ApplicationDeadline { get; set; }
    }
}