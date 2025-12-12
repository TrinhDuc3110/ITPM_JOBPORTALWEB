using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums; // Cần thiết
using MediatR;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.ApplyJob
{
    public class ApplyJobCommandHandler : IRequestHandler<ApplyJobCommand, int>
    {
        private readonly IJobRepository _jobRepository;
        private readonly INotificationService _notificationService;

        public ApplyJobCommandHandler(IJobRepository jobRepository, INotificationService notificationService)
        {
            _jobRepository = jobRepository;
            _notificationService = notificationService;
        }

        public async Task<int> Handle(ApplyJobCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy thông tin công việc
            var job = await _jobRepository.GetJobByIdAsync(request.JobId);

            // 2. Kiểm tra tồn tại
            if (job == null)
            {
                throw new ApplicationException($"Không tìm thấy công việc với ID: {request.JobId}.");
            }

            // 3. Kiểm tra Trạng thái (Active/Closed)
            if (job.Status != JobStatus.Active)
            {
                throw new ApplicationException("Rất tiếc, công việc này đã đóng hoặc đang tạm ngưng tuyển dụng.");
            }

            // 4. Kiểm tra Hạn nộp
            if (job.ApplicationDeadline < DateTime.UtcNow)
            {
                throw new ApplicationException("Đã quá hạn nộp hồ sơ cho công việc này.");
            }

            // 5. Kiểm tra Spam
            var appliedCount = await _jobRepository.CountApplicationsAsync(request.JobId, request.JobSeekerId);
            if (appliedCount >= 3)
            {
                throw new ApplicationException("Bạn đã nộp hồ sơ cho công việc này quá 3 lần.");
            }

            // 6. Tạo đơn ứng tuyển
            var application = new JobApplication
            {
                JobId = request.JobId,
                JobSeekerId = request.JobSeekerId,
                CoverLetter = request.DTO.CoverLetter,
                CvPath = request.CvPath,
                ApplicationDate = DateTime.UtcNow,
                Status = ApplicationStatus.Applied
            };

            // 7. Lưu vào Database
            var applicationId = await _jobRepository.AddApplicationAsync(application);

            // 8. Gửi thông báo
            await _notificationService.SendApplicationNotificationAsync(
                job.RecruiterId,
                job.Title,
                application.JobSeekerId
            );

            return applicationId;
        }
    }
}