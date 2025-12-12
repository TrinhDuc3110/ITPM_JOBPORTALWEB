using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class SavedCandidate
    {
        public int Id { get; set; }

        public Guid RecruiterId { get; set; } // Người lưu
        public User Recruiter { get; set; } = null!;

        public Guid CandidateId { get; set; } // Ứng viên được lưu
        public User Candidate { get; set; } = null!;

        public DateTime SavedDate { get; set; } = DateTime.UtcNow;
        public string? Note { get; set; } // Ghi chú của Recruiter về ứng viên này
    }
}