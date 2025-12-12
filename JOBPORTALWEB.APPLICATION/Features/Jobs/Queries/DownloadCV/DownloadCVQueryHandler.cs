using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System;
using System.IO;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.DownloadCV
{
    public class DownloadCVQueryHandler : IRequestHandler<DownloadCVQuery, (string FilePath, string FileName, Guid OwnerId)?>
    {
        private readonly IJobRepository _jobRepository;

        public DownloadCVQueryHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<(string FilePath, string FileName, Guid OwnerId)?> Handle(DownloadCVQuery request, CancellationToken cancellationToken)
        {
            // 1. Lấy JobApplication (Cần thêm Include(Job) vào Repository để truy cập RecruiterId)
            var application = await _jobRepository.GetApplicationByIdAsync(request.ApplicationId);

            if (application == null)
            {
                return null;
            }

            // 2. *** KIỂM TRA QUYỀN SỞ HỮU DỮ LIỆU ***
            // JobId của Application phải thuộc Job mà Recruiter đang sở hữu.
            // Để làm điều này, chúng ta cần JobRepository trả về Application kèm theo Job Entity
            if (application.Job.RecruiterId != request.RecruiterId)
            {
                // Ném lỗi để Controller bắt và trả về 403 Forbidden
                throw new UnauthorizedAccessException("Bạn không có quyền truy cập hồ sơ này.");
            }

            await _jobRepository.MarkAsDownloadedAsync(request.ApplicationId);

            // 3. Trả về thông tin file cần thiết cho Controller
            return (application.CvPath, Path.GetFileName(application.CvPath), application.Job.RecruiterId);
        }
    }
}