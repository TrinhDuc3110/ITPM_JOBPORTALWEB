// File: JOBPORTALWEB.APPLICATION/Features/Jobs/Commands/DeleteJob/DeleteJobCommandHandler.cs

using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.DeleteJob
{
    // Đảm bảo triển khai đúng IRequestHandler<Command, bool>
    public class DeleteJobCommandHandler : IRequestHandler<DeleteJobCommand, bool>
    {
        private readonly IJobRepository _jobRepository;

        public DeleteJobCommandHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<bool> Handle(DeleteJobCommand request, CancellationToken cancellationToken)
        {
            // 1. Lấy Job Entity
            var job = await _jobRepository.GetJobByIdAsync(request.Id);

            if (job == null)
            {
                return false;
            }

            // 2. *** KIỂM TRA QUYỀN SỞ HỮU (AUTHORIZATION) ***
            if (job.RecruiterId != request.RecruiterId)
            {
                // Ném ra lỗi để Controller bắt và trả về 403
                throw new UnauthorizedAccessException("Bạn không có quyền xóa công việc này.");
            }

            // 3. Xóa và lưu DB
            return await _jobRepository.DeleteJobAsync(job);
        }
    }
}