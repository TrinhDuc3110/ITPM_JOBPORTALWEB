using JOBPORTALWEB.DOMAIN.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.DOMAIN.Entities
{
    public class Job
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool HasApplied { get; set; }
        [NotMapped]
        public bool IsSaved { get; set; } = false;


        // --- Location ---
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty; // Full address
        public bool IsRemote { get; set; } = false;

        // --- Salary ---
        public decimal SalaryMin { get; set; }
        public decimal SalaryMax { get; set; }
        public SalaryType SalaryType { get; set; } = SalaryType.Monthly;

        // --- Job Details ---
        public JobType JobType { get; set; } // Full-time, Part-time...
        public JobLevel JobLevel { get; set; } // Senior, Junior...
        public int Vacancies { get; set; } = 1; // Số lượng tuyển
        public string? Experience { get; set; } // "3+ Years"
        public string? Education { get; set; } // "Bachelor Degree"
        public string? Tags { get; set; } // Lưu dạng "C#, .NET, SQL"
        public string? Benefits { get; set; } // Lưu dạng JSON hoặc text

        // --- Status & Dates ---
        public JobStatus Status { get; set; } = JobStatus.Active;
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;
        public DateTime ApplicationDeadline { get; set; }

        // --- Relations ---
        public Guid RecruiterId { get; set; }

        // Job thuộc về Company (Hiển thị logo công ty ở trang Find Job)
        public int? CompanyId { get; set; }
        public Company? Company { get; set; }
    }
}