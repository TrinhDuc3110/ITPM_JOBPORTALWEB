using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using MediatR;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, string>
    {
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;

        public RegisterCommandHandler(UserManager<User> userManager, INotificationService notificationService)
        {
            _userManager = userManager;
            _notificationService = notificationService;
        }

        public async Task<string> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var user = new User
            {
                Email = request.DTO.Email,
                FullName = request.DTO.FullName,
                Role = request.DTO.Role,
                UserName = request.DTO.Email,
                CreatedAt = DateTime.UtcNow,
                IsActive = request.DTO.Role != UserRole.Recruiter
            };

            //Sử dụng Usermanager để tạp người dùng
            var result = await _userManager.CreateAsync(user, request.DTO.Password);
            if (result.Succeeded)
            {
                var roleName = request.DTO.Role.ToString();
                await _userManager.AddToRoleAsync(user, roleName);

                if (request.DTO.Role == UserRole.Recruiter)
                {
                    // Fire & Forget (không cần await để user không phải đợi)
                    _ = _notificationService.NotifyAdminNewRecruiterAsync(user.Email, user.FullName);

                    return "Đăng ký thành công! Tài khoản Nhà tuyển dụng cần chờ Admin duyệt.";
                }

                return "Đăng ký thành công!";
            }

            if (request.DTO.Role == UserRole.Recruiter)
            {
                return "Đăng ký thành công! Tài khoản Nhà tuyển dụng cần chờ Admin duyệt trước khi đăng nhập.";
            }

            return $"Đăng ký thất bại: {string.Join(", ", result.Errors.Select(e => e.Description))}";
        }
    }
}
