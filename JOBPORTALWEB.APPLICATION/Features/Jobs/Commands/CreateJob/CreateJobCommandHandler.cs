using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using MediatR;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.CreateJob
{
    public class CreateJobCommandHandler : IRequestHandler<CreateJobCommand, int>
    {
        private readonly IJobRepository _jobRepository;
        private readonly INotificationService _notificationService;

        public CreateJobCommandHandler(
            IJobRepository jobRepository,
            INotificationService notificationService)
        {
            _jobRepository = jobRepository;
            _notificationService = notificationService;
        }

        public async Task<int> Handle(CreateJobCommand request, CancellationToken cancellationToken)
        {
            // Lấy companyId từ recruiter
            var companyId = await _jobRepository.GetCompanyIdByRecruiterIdAsync(request.RecruiterId);

            var job = new Job
            {
                Title = request.DTO.Title,
                Description = request.DTO.Description,
                Country = request.DTO.Country,
                City = request.DTO.City,
                Location = $"{request.DTO.City}, {request.DTO.Country}",
                IsRemote = request.DTO.IsRemote,

                SalaryMin = request.DTO.SalaryMin,
                SalaryMax = request.DTO.SalaryMax,
                SalaryType = request.DTO.SalaryType,

                JobType = request.DTO.JobType,
                JobLevel = request.DTO.JobLevel,
                Vacancies = request.DTO.Vacancies,
                Experience = request.DTO.Experience,
                Education = request.DTO.Education,
                Tags = request.DTO.Tags,
                Benefits = request.DTO.Benefits,

                ApplicationDeadline = request.DTO.ApplicationDeadline,
                PostedDate = DateTime.UtcNow,
                Status = JobStatus.Active,
                RecruiterId = request.RecruiterId,

                CompanyId = companyId   
            };

            var newJob = await _jobRepository.AddJobAsync(job);
            await _notificationService.NotifyJobCreatedAsync(newJob.Title);

            return newJob.Id;
        }

    }
}
