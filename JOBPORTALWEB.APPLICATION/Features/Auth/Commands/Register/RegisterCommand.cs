using JOBPORTALWEB.APPLICATION.DTOs.Auth;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.Register
{
    public class RegisterCommand : IRequest<string>
    {
        public RegisterRequest DTO { get; set; }

        public RegisterCommand(RegisterRequest dto) 
        {
            DTO = dto;
        }
    }
}
