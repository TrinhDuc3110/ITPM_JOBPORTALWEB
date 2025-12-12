using MediatR;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using System.Threading.Tasks;
using System.Threading;

namespace JOBPORTALWEB.APPLICATION.Features.Users.Commands.UpdateProfile
{
    public class UpdateProfileCommandHandler : IRequestHandler<UpdateProfileCommand, bool>
    {
        private readonly IUserProfileRepository _profileRepository;

        public UpdateProfileCommandHandler(IUserProfileRepository profileRepository)
        {
            _profileRepository = profileRepository;
        }

        public async Task<bool> Handle(UpdateProfileCommand request, CancellationToken cancellationToken)
        {
            // 1. Tìm Profile hiện tại
            var profile = await _profileRepository.GetProfileByUserIdAsync(request.UserId);

            bool isNewProfile = (profile == null);

            if (isNewProfile)
            {
                // Nếu chưa có, tạo đối tượng mới để chuẩn bị Add
                profile = new UserProfile { UserId = request.UserId };
            }

            // 2. Ánh xạ DTO lên Entity (Cả trường hợp MỚI và CẬP NHẬT)
            // Sử dụng toán tử ?? để chỉ cập nhật nếu giá trị DTO khác null
            profile.Headline = request.DTO.Headline ?? profile.Headline;
            profile.Bio = request.DTO.Bio ?? profile.Bio;
            profile.Skills = request.DTO.Skills ?? profile.Skills;
            profile.Education = request.DTO.Education ?? profile.Education;
            profile.Experience = request.DTO.Experience ?? profile.Experience;
            profile.Location = request.DTO.Location ?? profile.Location;

            // 3. Lưu (Add hoặc Update)
            if (isNewProfile)
            {
                await _profileRepository.AddProfileAsync(profile);
            }
            else
            {
                await _profileRepository.UpdateProfileAsync(profile);
            }

            return true;
        }
    }
}