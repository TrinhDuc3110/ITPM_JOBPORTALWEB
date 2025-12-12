using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Common;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetRecruiterJobs
{
    public class GetRecruiterJobsQueryHandler : IRequestHandler<GetRecruiterJobsQuery, PaginatedList<Job>>
    {
        private readonly IJobRepository _jobRepository;

        public GetRecruiterJobsQueryHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<PaginatedList<Job>> Handle(GetRecruiterJobsQuery request, CancellationToken cancellationToken)
        {
            return await _jobRepository.GetJobsByRecruiterIdAsync(request.RecruiterId, request.PageNumber, request.PageSize);
        }
    }
}