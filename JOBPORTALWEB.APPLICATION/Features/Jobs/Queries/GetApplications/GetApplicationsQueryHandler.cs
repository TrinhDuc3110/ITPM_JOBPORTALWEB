using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.APPLICATION.DTOs.Job;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetApplications
{
    public class GetApplicationsQueryHandler : IRequestHandler<GetApplicationsQuery, List<JobApplicationDto>>
    {
        private readonly IJobRepository _jobRepository;

        public GetApplicationsQueryHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<List<JobApplicationDto>> Handle(GetApplicationsQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy dữ liệu thực tế từ DB (đã lọc theo RecruiterId)
            var applications = await _jobRepository.GetApplicationsByRecruiterIdAsync(request.RecruiterId);

            // 2. Ánh xạ từ Entity sang DTO
            var result = applications.Select(ja => new JobApplicationDto
            {
                ApplicationId = ja.Id,
                JobId = ja.JobId,
                JobSeekerId = ja.JobSeekerId,
                JobSeekerEmail = ja.JobSeeker.Email ?? string.Empty,
                JobTitle = ja.Job.Title,
                ApplicationDate = ja.ApplicationDate,
                Status = ja.Status.ToString(),
                CvPath = ja.CvPath,
                IsDownloaded = ja.IsDownloaded
            }).ToList();

            return result;
        }
    }
}