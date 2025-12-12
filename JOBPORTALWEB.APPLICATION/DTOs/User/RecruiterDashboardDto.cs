using System.Collections.Generic;

namespace JOBPORTALWEB.APPLICATION.DTOs.User
{
    public class RecruiterDashboardDto
    {
        public int TotalActiveJobs { get; set; }
        public int TotalApplications { get; set; }
        public int NewApplicationsLast7Days { get; set; }
        public int SavedCandidatesCount { get; set; } 

        public List<RecentJobDto> RecentJobs { get; set; } = new List<RecentJobDto>();
    }

    public class RecentJobDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Type { get; set; } = "Full Time";
        public DateTime ApplicationDeadline { get; set; }
        public string Status { get; set; } = "Active";
        public int ApplicationCount { get; set; }
    }
}