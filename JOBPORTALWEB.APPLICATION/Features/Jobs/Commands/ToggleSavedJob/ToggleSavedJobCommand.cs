using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.ToggleSavedJob
{
    public class ToggleSavedJobCommand : IRequest<bool>
    {
        public int JobId { get; set; }
        public Guid UserId { get; set; }
        public ToggleSavedJobCommand(int jobId, Guid userId) { JobId = jobId; UserId = userId; }
    }

    public class ToggleSavedJobCommandHandler : IRequestHandler<ToggleSavedJobCommand, bool>
    {
        private readonly IJobRepository _repo;
        public ToggleSavedJobCommandHandler(IJobRepository repo) { _repo = repo; }

        public async Task<bool> Handle(ToggleSavedJobCommand request, CancellationToken cancellationToken)
        {
            return await _repo.ToggleSavedJobAsync(request.JobId, request.UserId);
        }
    }
}