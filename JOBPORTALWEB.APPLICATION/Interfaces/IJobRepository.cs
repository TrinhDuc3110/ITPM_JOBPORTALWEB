using JOBPORTALWEB.APPLICATION.DTOs.Common;
using JOBPORTALWEB.APPLICATION.DTOs.User;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Interfaces
{
    public interface IJobRepository
    {
        Task<Job> AddJobAsync(Job job);

        Task<PaginatedList<Job>> GetPaginatedJobsAsync(
              int pageNumber,
              int pageSize,
              string? location,
              string? searchTerm,
              bool? isRemote,
              JobType? jobType,
              decimal? minSalary, 
              Guid? currentUserId
          );

        Task<Job?> GetJobByIdAsync(int id);

        Task<bool> UpdateJobAsync(Job job);
        Task<bool> DeleteJobAsync(Job job);

        Task<int> AddApplicationAsync(JobApplication application);

        Task<List<JobApplication>> GetApplicationsByRecruiterIdAsync(Guid recruiterId);
        Task<JobApplication?> GetApplicationByIdAsync(int applicationId);
        Task<RecruiterDashboardDto> GetDashboardStatsAsync(Guid recruiterId);
        Task<int> CountApplicationsAsync(int jobId, Guid jobSeekerId);
        Task<int> CountApplicationsAsync(Guid recruiterId);
        Task<bool> ToggleSavedJobAsync(int jobId, Guid jobSeekerId);
        Task<List<Job>> GetSavedJobsBySeekerIdAsync(Guid jobSeekerId);
        Task<bool> HasUserAppliedAsync(int jobId, Guid userId);
        Task MarkAsDownloadedAsync(int applicationId);
        Task<PaginatedList<Job>> GetJobsByRecruiterIdAsync(Guid recruiterId, int pageNumber, int pageSize);
        Task<bool> IsJobSavedAsync(int jobId, Guid userId);
        Task<int?> GetCompanyIdByRecruiterIdAsync(Guid recruiterId);
        Task UpdateCompanyForRecruiterJobsAsync(Guid recruiterId, int companyId);


    }
}