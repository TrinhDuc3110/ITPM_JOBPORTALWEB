using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Enums;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.CloseJob
{
    public class CloseJobCommandHandler : IRequestHandler<CloseJobCommand, bool>
    {
        private readonly IJobRepository _jobRepository;

        public CloseJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<bool> Handle(CloseJobCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm công việc
            var job = await _jobRepository.GetJobByIdAsync(request.JobId);

            if (job == null)
            {
                return false; // Không tìm thấy
            }

            // 2. Kiểm tra quyền sở hữu (Chỉ người tạo mới được đóng)
            if (job.RecruiterId != request.RecruiterId)
            {
                throw new UnauthorizedAccessException("Bạn không có quyền đóng công việc này.");
            }

            // 3. Cập nhật trạng thái
            job.Status = JobStatus.Closed; // Đổi sang trạng thái Đóng (Enum value = 2)

            // 4. Lưu vào Database
            return await _jobRepository.UpdateJobAsync(job);
        }
    }
}