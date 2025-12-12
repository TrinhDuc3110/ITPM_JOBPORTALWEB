using MediatR;
using JOBPORTALWEB.APPLICATION.DTOs.Auth;





namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.ForgotPassword
{
    // Trả về true để tránh tiết lộ tài khoản nào tồn tại
    public class ForgotPasswordCommand : IRequest<bool>
    {
        public ForgotPasswordRequest DTO { get; set; }

        public ForgotPasswordCommand(ForgotPasswordRequest dto)
        {
            DTO = dto;
        }
    }
}