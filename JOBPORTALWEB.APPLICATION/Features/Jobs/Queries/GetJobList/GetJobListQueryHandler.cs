using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.APPLICATION.DTOs.Common;


namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobList
{
    // SỬA: Kiểu trả về phải là PaginatedList<Job>
    public class GetJobListQueryHandler : IRequestHandler<GetJobListQuery, PaginatedList<Job>>
    {
        private readonly IJobRepository _jobRepository;

        public GetJobListQueryHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<PaginatedList<Job>> Handle(GetJobListQuery request, CancellationToken cancellationToken)
        {
            // Gọi Repository với tất cả các tham số từ Query
            return await _jobRepository.GetPaginatedJobsAsync(
                request.PageNumber,
                request.PageSize,
                request.Location,
                request.SearchTerm,
                request.IsRemote,
                request.JobType,
                request.MinSalary,
                request.CurrentUserId
            );
        }
    }
}