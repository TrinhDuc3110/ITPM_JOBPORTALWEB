using MediatR;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetSavedJobs
{
    public class GetSavedJobsQuery : IRequest<List<Job>>
    {
        public Guid UserId { get; set; }
        public GetSavedJobsQuery(Guid userId) { UserId = userId; }
    }

    public class GetSavedJobsQueryHandler : IRequestHandler<GetSavedJobsQuery, List<Job>>
    {
        private readonly IJobRepository _repo;
        public GetSavedJobsQueryHandler(IJobRepository repo) { _repo = repo; }

        public async Task<List<Job>> Handle(GetSavedJobsQuery request, CancellationToken cancellationToken)
        {
            return await _repo.GetSavedJobsBySeekerIdAsync(request.UserId);
        }
    }
}