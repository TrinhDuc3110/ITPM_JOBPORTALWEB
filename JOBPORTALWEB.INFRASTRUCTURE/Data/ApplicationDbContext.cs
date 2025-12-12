using JOBPORTALWEB.DOMAIN.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

namespace JOBPORTALWEB.INFRASTRUCTURE.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // Các DbSet
        public DbSet<Job> Jobs { get; set; }
        public DbSet<JobApplication> JobApplications { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }
        public DbSet<SavedCandidate> SavedCandidates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 1. Cấu hình Identity (Bắt buộc)
            base.OnModelCreating(modelBuilder);

            // 2. Cấu hình Job (Decimal Salary)
            modelBuilder.Entity<Job>()
                .Property(j => j.SalaryMin)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Job>()
                .Property(j => j.SalaryMax)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<UserProfile>()
                .HasOne(p => p.User)         
                .WithOne(u => u.UserProfile)  
                .HasForeignKey<UserProfile>(p => p.UserId)
                .IsRequired();

            // 4. Cấu hình Company (One-to-Many)
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Company)
                .WithMany(c => c.Jobs)
                .HasForeignKey(j => j.CompanyId)
                .OnDelete(DeleteBehavior.Restrict); // Tránh xóa công ty làm mất Job

            // 5. Cấu hình SavedJob (Khắc phục lỗi Cycle)
            modelBuilder.Entity<SavedJob>()
                .HasIndex(s => new { s.JobId, s.JobSeekerId }).IsUnique();

            modelBuilder.Entity<SavedJob>()
                .HasOne(s => s.Job)
                .WithMany()
                .HasForeignKey(s => s.JobId)
                .OnDelete(DeleteBehavior.Restrict); // QUAN TRỌNG: Tắt Cascade Delete

            modelBuilder.Entity<SavedJob>()
                .HasOne(s => s.JobSeeker)
                .WithMany()
                .HasForeignKey(s => s.JobSeekerId)
                .OnDelete(DeleteBehavior.Restrict); // QUAN TRỌNG: Tắt Cascade Delete

            // 6. Cấu hình SavedCandidate (Khắc phục lỗi Cycle)
            modelBuilder.Entity<SavedCandidate>()
                .HasIndex(s => new { s.RecruiterId, s.CandidateId }).IsUnique();

            modelBuilder.Entity<SavedCandidate>()
                .HasOne(s => s.Recruiter)
                .WithMany()
                .HasForeignKey(s => s.RecruiterId)
                .OnDelete(DeleteBehavior.Restrict); // QUAN TRỌNG: Tắt Cascade Delete

            modelBuilder.Entity<SavedCandidate>()
                .HasOne(s => s.Candidate)
                .WithMany()
                .HasForeignKey(s => s.CandidateId)
                .OnDelete(DeleteBehavior.Restrict); // QUAN TRỌNG: Tắt Cascade Delete

            // 7. Cấu hình JobApplication (An toàn)
            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.Job)
                .WithMany()
                .HasForeignKey(ja => ja.JobId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<JobApplication>()
                .HasOne(ja => ja.JobSeeker)
                .WithMany()
                .HasForeignKey(ja => ja.JobSeekerId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}