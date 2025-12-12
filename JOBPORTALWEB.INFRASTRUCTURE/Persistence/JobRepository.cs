using JOBPORTALWEB.APPLICATION.DTOs.Common;
using JOBPORTALWEB.APPLICATION.DTOs.User;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using JOBPORTALWEB.INFRASTRUCTURE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JOBPORTALWEB.INFRASTRUCTURE.Persistence
{
    public class JobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public JobRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Thêm Job
        public async Task<Job> AddJobAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
            return job;
        }

        // 2. Lấy Job theo ID
        public async Task<Job?> GetJobByIdAsync(int id)
        {
            return await _context.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == id);
        }

        // 3. Cập nhật Job
        public async Task<bool> UpdateJobAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
            return true;
        }

        // 4. Xóa Job
        public async Task<bool> DeleteJobAsync(Job job)
        {
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
            return true;
        }

        // 5. Thêm Đơn Ứng Tuyển
        public async Task<int> AddApplicationAsync(JobApplication application)
        {
            _context.JobApplications.Add(application);
            await _context.SaveChangesAsync();
            return application.Id;
        }

        // 6. Lấy Đơn Ứng Tuyển theo Recruiter
        public async Task<List<JobApplication>> GetApplicationsByRecruiterIdAsync(Guid recruiterId)
        {
            return await _context.JobApplications
                .Where(ja => ja.Job.RecruiterId == recruiterId)
                .Include(ja => ja.Job)
                .Include(ja => ja.JobSeeker).ThenInclude(u => u.UserProfile)
                .OrderByDescending(ja => ja.ApplicationDate)
                .ToListAsync();
        }

        // 7. Lấy Đơn Ứng Tuyển theo ID
        public async Task<JobApplication?> GetApplicationByIdAsync(int applicationId)
        {
            return await _context.JobApplications
                .Include(ja => ja.Job)
                .FirstOrDefaultAsync(ja => ja.Id == applicationId);
        }

        // 8. Dashboard Stats
        public async Task<RecruiterDashboardDto> GetDashboardStatsAsync(Guid recruiterId)
        {
            var jobsQuery = _context.Jobs.Where(j => j.RecruiterId == recruiterId);

            var totalActiveJobs = await jobsQuery.CountAsync(j => j.Status == JobStatus.Active);
            var totalApplications = await _context.JobApplications.CountAsync(ja => ja.Job.RecruiterId == recruiterId);

            var dateThreshold = DateTime.UtcNow.AddDays(-7);
            var newApplications = await _context.JobApplications.CountAsync(ja => ja.Job.RecruiterId == recruiterId && ja.ApplicationDate >= dateThreshold);

            // Lấy danh sách job gần đây
            var recentJobs = await jobsQuery
               .OrderByDescending(j => j.PostedDate)
               .Take(5)
               .Select(j => new RecentJobDto
               {
                   Id = j.Id,
                   Title = j.Title,
                   ApplicationDeadline = j.ApplicationDeadline,
                   Status = j.Status.ToString(),
                   ApplicationCount = _context.JobApplications.Count(ja => ja.JobId == j.Id)
               })
               .ToListAsync();

            return new RecruiterDashboardDto
            {
                TotalActiveJobs = totalActiveJobs,
                TotalApplications = totalApplications,
                NewApplicationsLast7Days = newApplications,
                RecentJobs = recentJobs
            };
        }

        // 9. Đếm số lần nộp đơn (Chặn Spam)
        public async Task<int> CountApplicationsAsync(int jobId, Guid jobSeekerId)
        {
            return await _context.JobApplications
                .CountAsync(ja => ja.JobId == jobId && ja.JobSeekerId == jobSeekerId);
        }

        // 10. Đếm tổng số đơn của Recruiter (Fix lỗi NotImplemented)
        public async Task<int> CountApplicationsAsync(Guid recruiterId)
        {
            return await _context.JobApplications
                .CountAsync(ja => ja.Job.RecruiterId == recruiterId);
        }

        // 11. Lưu / Bỏ lưu Job
        public async Task<bool> ToggleSavedJobAsync(int jobId, Guid jobSeekerId)
        {
            var existing = await _context.SavedJobs.FirstOrDefaultAsync(s => s.JobId == jobId && s.JobSeekerId == jobSeekerId);
            if (existing != null) { _context.SavedJobs.Remove(existing); await _context.SaveChangesAsync(); return false; }

            _context.SavedJobs.Add(new SavedJob { JobId = jobId, JobSeekerId = jobSeekerId });
            await _context.SaveChangesAsync(); return true;
        }

        // 12. Lấy Job đã lưu
        public async Task<List<Job>> GetSavedJobsBySeekerIdAsync(Guid jobSeekerId)
        {
            return await _context.SavedJobs.Where(s => s.JobSeekerId == jobSeekerId)
                .Include(s => s.Job).ThenInclude(j => j.Company).Select(s => s.Job).ToListAsync();
        }

        // 13. Kiểm tra đã nộp chưa
        public async Task<bool> HasUserAppliedAsync(int jobId, Guid userId)
        {
            return await _context.JobApplications.AnyAsync(ja => ja.JobId == jobId && ja.JobSeekerId == userId);
        }

        // 14. Lấy danh sách Job (Phân trang & Lọc - Đã fix đủ 7 tham số)
        public async Task<PaginatedList<Job>> GetPaginatedJobsAsync(
            int pageNumber, int pageSize, string? location, string? searchTerm,
            bool? isRemote, JobType? jobType, decimal? minSalary, Guid? currentUserId)
        {
            var query = _context.Jobs
                .Include(j => j.Company)
                .AsQueryable();

            // Chỉ lấy Job đang Active (Chưa đóng)
            query = query.Where(j => j.Status == JobStatus.Active);

            if (!string.IsNullOrEmpty(location))
            {
                var loc = location.ToLower();
                query = query.Where(j => j.Location.ToLower().Contains(loc) || j.City.ToLower().Contains(loc));
            }
            if (!string.IsNullOrEmpty(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(j => j.Title.ToLower().Contains(term) || j.Description.ToLower().Contains(term));
            }

            if (isRemote.HasValue) query = query.Where(j => j.IsRemote == isRemote.Value);
            if (jobType.HasValue) query = query.Where(j => j.JobType == jobType.Value);
            if (minSalary.HasValue) query = query.Where(j => j.SalaryMax >= minSalary.Value);

            query = query.OrderByDescending(j => j.PostedDate);

            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            if (currentUserId.HasValue)
            {
                var savedJobIds = await _context.SavedJobs
                    .Where(s => s.JobSeekerId == currentUserId.Value)
                    .Select(s => s.JobId)
                    .ToListAsync();

                // Duyệt qua danh sách Job vừa lấy, nếu ID nằm trong savedJobIds thì set IsSaved = true
                foreach (var job in items)
                {
                    if (savedJobIds.Contains(job.Id))
                    {
                        job.IsSaved = true;
                    }
                }
            }

            return new PaginatedList<Job>(items, count, pageNumber, pageSize);
        }

        public async Task MarkAsDownloadedAsync(int applicationId)
        {
            var app = await _context.JobApplications.FindAsync(applicationId);
            if (app != null && !app.IsDownloaded)
            {
                app.IsDownloaded = true;
                await _context.SaveChangesAsync();
            }
        }

        // Trong JobRepository.cs (Triển khai)
        public async Task<PaginatedList<Job>> GetJobsByRecruiterIdAsync(Guid recruiterId, int pageNumber, int pageSize)
        {
            var query = _context.Jobs
                .Where(j => j.RecruiterId == recruiterId) 
                .Include(j => j.Company) 
                .OrderByDescending(j => j.PostedDate); 

            var count = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();

            return new PaginatedList<Job>(items, count, pageNumber, pageSize);
        }

        public async Task<bool> IsJobSavedAsync(int jobId, Guid userId)
        {
            return await _context.SavedJobs.AnyAsync(s => s.JobId == jobId && s.JobSeekerId == userId);
        }

        public async Task<int?> GetCompanyIdByRecruiterIdAsync(Guid recruiterId)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == recruiterId);

            return user?.CompanyId;
        }
        public async Task UpdateCompanyForRecruiterJobsAsync(Guid recruiterId, int companyId)
        {
            var jobs = await _context.Jobs
                .Where(j => j.RecruiterId == recruiterId && j.CompanyId == null)
                .ToListAsync();

            foreach (var job in jobs)
            {
                job.CompanyId = companyId;
            }

            await _context.SaveChangesAsync();
        }
    }
}