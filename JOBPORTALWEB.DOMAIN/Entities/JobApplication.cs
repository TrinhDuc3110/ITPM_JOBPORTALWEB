using JOBPORTALWEB.DOMAIN.Enums;
using System;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class JobApplication
    {
        public int Id { get; set; }

        // Khóa ngoại tới Job đã ứng tuyển
        public int JobId { get; set; }
        public Guid UserId { get; set; }
        public Job Job { get; set; } = null!;

        // Khóa ngoại tới Người tìm việc
        public Guid JobSeekerId { get; set; }
        public User JobSeeker { get; set; } = null!;

        public DateTime ApplicationDate { get; set; } = DateTime.UtcNow;
        public ApplicationStatus Status { get; set; } = ApplicationStatus.Applied; 
        public string? CoverLetter { get; set; }

        public string CvPath { get; set; } = string.Empty;
        public bool IsDownloaded { get; set; } = false;
    }
}