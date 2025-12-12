using MediatR;
using Microsoft.AspNetCore.Identity;
using JOBPORTALWEB.DOMAIN.Entities;
using Microsoft.AspNetCore.WebUtilities; // QUAN TRỌNG
using System.Text;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.ResetPassword
{
    public class ResetPasswordCommandHandler : IRequestHandler<ResetPasswordCommand, bool>
    {
        private readonly UserManager<User> _userManager;

        public ResetPasswordCommandHandler(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.DTO.Email);
            if (user == null) return false;

            IdentityResult result;

            // --- LOGIC MỚI: XỬ LÝ TOKEN LINH HOẠT ---

            // THỬ NGHIỆM 1: Dùng Token trực tiếp (nếu Token chưa bị encode)
            try
            {
                result = await _userManager.ResetPasswordAsync(user, request.DTO.Token, request.DTO.NewPassword);
                if (result.Succeeded) return true;
            }
            catch {}

            // THỬ NGHIỆM 2: Giải mã Base64Url (Dành cho Token lấy từ Link Email)
            try
            {
                var tokenBytes = WebEncoders.Base64UrlDecode(request.DTO.Token);
                var decodedToken = Encoding.UTF8.GetString(tokenBytes);

                result = await _userManager.ResetPasswordAsync(user, decodedToken, request.DTO.NewPassword);

                if (result.Succeeded) return true;

                // *** GHI LOG LỖI RA CONSOLE ***
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                Console.WriteLine($"❌ LỖI IDENTITY: {errors}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ LỖI GIẢI MÃ: {ex.Message}");
            }

            return false;
        }
    }
}