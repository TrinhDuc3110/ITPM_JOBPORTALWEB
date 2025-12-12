
using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.User;



namespace JOBPORTALWEB.APPLICATION.Features.Users.Commands.UpdateProfile
{
    public class UpdateProfileCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public UpdateProfileRequest DTO { get; set; }

        public UpdateProfileCommand(Guid userId, UpdateProfileRequest dto)
        {
            UserId = userId;
            DTO = dto;
        }
    }
}