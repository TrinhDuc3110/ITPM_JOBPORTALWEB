using System.ComponentModel.DataAnnotations;

namespace JOBPORTALWEB.APPLICATION.DTOs.Job
{
    public class UpdateJobRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;
        public decimal SalaryMin { get; set; }
        public decimal SalaryMax { get; set; }
        public DateTime ApplicationDeadline { get; set; }
    }
}