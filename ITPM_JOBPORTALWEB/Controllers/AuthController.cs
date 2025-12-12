using JOBPORTALWEB.APPLICATION.DTOs.Auth;
using JOBPORTALWEB.APPLICATION.Features.Auth.Commands.Login;
using JOBPORTALWEB.APPLICATION.Features.Auth.Commands.Register;
using JOBPORTALWEB.APPLICATION.Features.Auth.Commands.ForgotPassword;
using JOBPORTALWEB.APPLICATION.Features.Auth.Commands.ResetPassword;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace ITPM_JOBPORTALWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IAuthService _authService;
        private readonly IConfiguration _config;

        [ActivatorUtilitiesConstructor]
        public AuthController(
            IMediator mediator,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IAuthService authService,
            IConfiguration config)
        {
            _mediator = mediator;
            _userManager = userManager;
            _signInManager = signInManager;
            _authService = authService;
            _config = config;
        }

        // 1. Đăng ký
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var command = new RegisterCommand(request);
            var result = await _mediator.Send(command);

            if (result.Contains("thành công"))
            {
                return Ok(new { Message = result });
            }
            return BadRequest(new { Message = result });
        }

        // 2. Đăng nhập
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var command = new LoginCommand(request);
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (ApplicationException ex)
            {
                return Unauthorized(new { Message = ex.Message });
            }
            catch (Exception)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống khi đăng nhập." });
            }
        }

        // 3. External Login Challenge
        [HttpGet("external-login")]
        public IActionResult ExternalLogin(string provider, string returnUrl = "/")
        {
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, Url.Action(nameof(ExternalLoginCallback), "Auth", new { returnUrl }));
            return Challenge(properties, provider);
        }

        // 4. External Login Callback
        [HttpGet("external-login-callback")]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = "/")
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null) return Redirect($"{returnUrl}?error=External login failed.");

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);

            if (user == null)
            {
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                if (string.IsNullOrEmpty(email)) return Redirect($"{returnUrl}?error=Email not received.");

                user = new User
                {
                    Email = email,
                    UserName = email,
                    FullName = name ?? email,
                    Role = UserRole.jobSeeker,
                    EmailConfirmed = true
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded) return Redirect($"{returnUrl}?error=Account creation failed.");

                await _userManager.AddLoginAsync(user, info);
                await _userManager.AddToRoleAsync(user, UserRole.jobSeeker.ToString());
            }

            var roles = await _userManager.GetRolesAsync(user);
            var tokenString = _authService.CreateToken(user, roles);

            var accessTokenMinutesString = _config["Jwt:AccessTokenMinutes"];
            if (!double.TryParse(accessTokenMinutesString, out double minutes)) minutes = 60;
            var expiry = DateTime.UtcNow.AddMinutes(minutes);

            return Redirect($"{returnUrl}?token={tokenString}&role={roles.FirstOrDefault()}&expires={expiry.Ticks}");
        }

        // 5. Quên mật khẩu
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            await _mediator.Send(new ForgotPasswordCommand(request));
            return Ok(new { Message = "Nếu email tồn tại, token đã được gửi đi." });
        }

        // 6. Đặt lại mật khẩu
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var command = new ResetPasswordCommand(request);
            var result = await _mediator.Send(command);

            if (result) return Ok(new { Message = "Mật khẩu đã được đặt lại thành công." });
            return BadRequest(new { Message = "Yêu cầu không hợp lệ hoặc đã hết hạn." });
        }
    }
}