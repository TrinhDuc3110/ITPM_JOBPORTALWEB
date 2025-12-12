using JOBPORTALWEB.APPLICATION.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.Login
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public LoginRequest DTO { get; set; }

        public LoginCommand(LoginRequest dto)
        {
            DTO = dto;
        }
    }
}
