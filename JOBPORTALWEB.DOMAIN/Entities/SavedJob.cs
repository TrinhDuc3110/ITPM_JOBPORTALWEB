using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class SavedJob
    {
        public int Id { get; set; }

        public int JobId { get; set; }
        public Job Job { get; set; } = null!;

        public Guid JobSeekerId { get; set; }
        public User JobSeeker { get; set; } = null!;

        public DateTime SavedDate { get; set; } = DateTime.UtcNow;
    }
}