using JOBPORTALWEB.APPLICATION.DTOs.Job;
using JOBPORTALWEB.DOMAIN.Entities;
using MediatR;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobDetail
{
    public class GetJobDetailQuery : IRequest<JobDetailDto?>
    {
        public int JobId { get; set; }
        public Guid? CurrentUserId { get; set; } // Thêm ID người đang xem (có thể null nếu chưa login)

        public GetJobDetailQuery(int jobId, Guid? currentUserId)
        {
            JobId = jobId;
            CurrentUserId = currentUserId;
        }
    }

}