using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JOBPORTALWEB.APPLICATION.DTOs.Auth;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommand : IRequest<bool>
    {
        public ResetPasswordRequest DTO { get; set; }

        public ResetPasswordCommand(ResetPasswordRequest dto)
        {
            DTO = dto;
        }
    }
}
