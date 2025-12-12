using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.APPLICATION.DTOs.User;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Users.Queries.GetRecruiterDashboard
{
    public class GetRecruiterDashboardQueryHandler : IRequestHandler<GetRecruiterDashboardQuery, RecruiterDashboardDto>
    {
        private readonly IJobRepository _jobRepository;

        public GetRecruiterDashboardQueryHandler(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }

        public async Task<RecruiterDashboardDto> Handle(GetRecruiterDashboardQuery request, CancellationToken cancellationToken)
        {
            return await _jobRepository.GetDashboardStatsAsync(request.RecruiterId);
        }
    }
}