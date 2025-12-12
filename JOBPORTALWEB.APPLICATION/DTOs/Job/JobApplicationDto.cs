using System;

namespace JOBPORTALWEB.APPLICATION.DTOs.Job
{
    public class JobApplicationDto
    {
        public int ApplicationId { get; set; }
        public int JobId { get; set; }
        public Guid JobSeekerId { get; set; }
        public string JobSeekerEmail { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string CvPath { get; set; } = string.Empty;
        public bool IsDownloaded { get; set; }
    }
}