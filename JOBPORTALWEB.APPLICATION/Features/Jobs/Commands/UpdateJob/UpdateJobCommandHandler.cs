using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.UpdateJob
{
    public class UpdateJobCommandHandler : IRequestHandler<UpdateJobCommand, bool>
    {
        private readonly IJobRepository _jobRepository;

        public UpdateJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<bool> Handle(UpdateJobCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy công việc hiện tại từ DB
            var job = await _jobRepository.GetJobByIdAsync(request.Id);

            if (job == null)
            {
                // Không tìm thấy Job
                return false;
            }

            // 2. KIỂM TRA QUYỀN SỞ HỮU (AUTHORIZATION)
            if (job.RecruiterId != request.RecruiterId)
            {
                // Người dùng không sở hữu Job này (403 Forbidden)
                throw new UnauthorizedAccessException("Bạn không có quyền chỉnh sửa công việc này.");
            }

            // 3. Ánh xạ dữ liệu mới lên Entity
            job.Title = request.DTO.Title;
            job.Description = request.DTO.Description;
            job.Location = request.DTO.Location;
            job.SalaryMin = request.DTO.SalaryMin;
            job.SalaryMax = request.DTO.SalaryMax;
            job.ApplicationDeadline = request.DTO.ApplicationDeadline;

            // 4. Lưu vào DB
            return await _jobRepository.UpdateJobAsync(job);
        }
    }
}