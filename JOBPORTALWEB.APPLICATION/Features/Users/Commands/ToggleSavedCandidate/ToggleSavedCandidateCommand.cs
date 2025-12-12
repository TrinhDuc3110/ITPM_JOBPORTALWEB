using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Users.Commands.ToggleSavedCandidate
{
    public class ToggleSavedCandidateCommand : IRequest<bool>
    {
        public Guid RecruiterId { get; set; }
        public Guid CandidateId { get; set; }
        public ToggleSavedCandidateCommand(Guid rid, Guid cid) { RecruiterId = rid; CandidateId = cid; }
    }

    public class ToggleSavedCandidateCommandHandler : IRequestHandler<ToggleSavedCandidateCommand, bool>
    {
        private readonly IUserProfileRepository _repo;
        public ToggleSavedCandidateCommandHandler(IUserProfileRepository repo) { _repo = repo; }

        public async Task<bool> Handle(ToggleSavedCandidateCommand request, CancellationToken cancellationToken)
        {
            return await _repo.ToggleSavedCandidateAsync(request.RecruiterId, request.CandidateId);
        }
    }
}