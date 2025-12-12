using MediatR;
using Microsoft.AspNetCore.Identity;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.APPLICATION.DTOs.Auth;
using JOBPORTALWEB.APPLICATION.Interfaces;

namespace JOBPORTALWEB.APPLICATION.Features.Auth.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;

        public LoginCommandHandler(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthService authService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var user = await _userManager.FindByEmailAsync(request.DTO.Email);

            if (user == null)
            {
                throw new ApplicationException("Email hoặc mật khẩu không hợp lệ.");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.DTO.Password, false);

            if (!result.Succeeded)
            {
                throw new ApplicationException("Email hoặc mật khẩu không hợp lệ.");
            }

            if (!user.IsActive)
            {
                throw new ApplicationException("Tài khoản của bạn đang chờ phê duyệt. Vui lòng liên hệ Admin.");
            }

            var roles = await _userManager.GetRolesAsync(user);

            var tokenString = _authService.CreateToken(user, roles);

            var expiry = DateTime.UtcNow.AddHours(2); 

            return new AuthResponse
            {
                UserId = user.Id.ToString(),
                Email = user.Email ?? string.Empty,
                Token = tokenString,
                Role = roles.FirstOrDefault() ?? string.Empty,
                Expires = expiry
            };
        }
    }
}