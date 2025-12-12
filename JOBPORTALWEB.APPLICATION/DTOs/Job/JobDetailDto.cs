using System;
using System.Collections.Generic;

namespace JOBPORTALWEB.APPLICATION.DTOs.Job
{
    public class JobDetailDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal SalaryMin { get; set; }
        public decimal SalaryMax { get; set; }
        public DateTime PostedDate { get; set; }
        public DateTime ApplicationDeadline { get; set; }
        public Guid RecruiterId { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyLogo { get; set; }
        public string? CompanyWebsite { get; set; }
        public string? CompanyDescription { get; set; }
        public bool HasApplied { get; set; }
        public bool IsSaved { get; set; }
    }
}