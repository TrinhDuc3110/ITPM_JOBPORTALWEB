using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.ForgotPassword
{
    public class ForgotPasswordCommandHandler : IRequestHandler<ForgotPasswordCommand, bool>
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;

        public ForgotPasswordCommandHandler(UserManager<User> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.DTO.Email);

            // Luôn trả về true để ngăn chặn việc dò tìm Email hợp lệ (Email Enumeration Attack)
            if (user == null)
            {
                return true;
            }

            // 1. Tạo Token Đặt lại Mật khẩu
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // 2. Mã hóa Token (Url Encode)
            // Token cần được encode vì nó chứa các ký tự đặc biệt (+, /)
            var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

            // 3. Xây dựng Link Đặt lại (Reset Link)
            // Thêm Email và Token vào Query String của ResetUrlBase
            var resetLink = QueryHelpers.AddQueryString(request.DTO.ResetUrlBase,
                                                        "email", user.Email);
            resetLink = QueryHelpers.AddQueryString(resetLink, "token", encodedToken);

            // 4. Gửi Email (Sử dụng IEmailService đã cấu hình)
            var subject = "Yêu cầu Đặt lại Mật khẩu Job Portal";
            var content = $"Vui lòng nhấp vào liên kết sau để đặt lại mật khẩu: <a href='{resetLink}'>Đặt lại Mật khẩu</a>";

            await _emailService.SendEmailAsync(user.Email!, subject, content);

            return true;
        }
    }
}