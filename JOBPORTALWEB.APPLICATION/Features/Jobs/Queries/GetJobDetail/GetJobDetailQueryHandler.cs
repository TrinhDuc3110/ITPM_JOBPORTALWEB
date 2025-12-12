using JOBPORTALWEB.APPLICATION.DTOs.Job;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobDetail
{
    public class GetJobDetailQueryHandler : IRequestHandler<GetJobDetailQuery, JobDetailDto?>
    {
        private readonly IJobRepository _jobRepository;

        public GetJobDetailQueryHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<JobDetailDto?> Handle(GetJobDetailQuery request, CancellationToken cancellationToken)
        {
            var job = await _jobRepository.GetJobByIdAsync(request.JobId);
            if (job == null) return null;

            bool hasApplied = false;
            bool isSaved = false;

            if (request.CurrentUserId.HasValue)
            {
                hasApplied = await _jobRepository.HasUserAppliedAsync(job.Id, request.CurrentUserId.Value);

                isSaved = await _jobRepository.IsJobSavedAsync(job.Id, request.CurrentUserId.Value);
            }

            // Ánh xạ sang DTO
            return new JobDetailDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Location = job.Location,
                SalaryMin = job.SalaryMin,
                SalaryMax = job.SalaryMax,
                PostedDate = job.PostedDate,
                ApplicationDeadline = job.ApplicationDeadline,
                RecruiterId = job.RecruiterId,
                CompanyName = job.Company?.Name ?? "Unknown Company",
                CompanyLogo = job.Company?.LogoUrl,
                CompanyWebsite = job.Company?.Website,
                CompanyDescription = job.Company?.Description,
                HasApplied = hasApplied,
                IsSaved = isSaved,
            };
        }
    }
}