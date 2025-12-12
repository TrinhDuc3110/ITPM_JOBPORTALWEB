using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.User;
using System;

namespace JOBPORTALWEB.APPLICATION.Features.Users.Queries.GetProfile
{
    public class GetProfileQuery : IRequest<UpdateProfileRequest?>
    {
        public Guid UserId { get; set; }

        public GetProfileQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}