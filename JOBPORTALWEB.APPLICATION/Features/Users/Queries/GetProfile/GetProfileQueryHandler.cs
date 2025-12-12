
using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.APPLICATION.DTOs.User;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Users.Queries.GetProfile
{
    public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, UpdateProfileRequest?>
    {
        private readonly IUserProfileRepository _profileRepository;

        public GetProfileQueryHandler(IUserProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<UpdateProfileRequest?> Handle(GetProfileQuery request, CancellationToken cancellationToken)
        {
            var profile = await _profileRepository.GetProfileByUserIdAsync(request.UserId);

            if (profile == null)
            {
                return null;
            }

            // Ánh xạ từ Entity sang DTO
            return new UpdateProfileRequest
            {
                Headline = profile.Headline,
                Bio = profile.Bio,
                Skills = profile.Skills,
                Education = profile.Education,
                Experience = profile.Experience,
                Location = profile.Location
            };
        }
    }
}