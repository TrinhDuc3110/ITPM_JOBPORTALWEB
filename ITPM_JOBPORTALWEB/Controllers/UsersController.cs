using JOBPORTALWEB.APPLICATION.DTOs.User;
using JOBPORTALWEB.APPLICATION.Features.Users.Commands.UpdateProfile;
using JOBPORTALWEB.APPLICATION.Features.Users.Queries.GetProfile;
using JOBPORTALWEB.APPLICATION.Features.Users.Queries.GetRecruiterDashboard;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using JOBPORTALWEB.APPLICATION.Features.Users.Commands.ToggleSavedCandidate;

namespace ITPM_JOBPORTALWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. Cập nhật Profile
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized(new { Message = "ID không hợp lệ." });

            var command = new UpdateProfileCommand(userId, request);
            await _mediator.Send(command);

            return NoContent();
        }

        // 2. Lấy Profile
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid userId))
                return Unauthorized();

            var query = new GetProfileQuery(userId);
            var profile = await _mediator.Send(query);

            if (profile == null) return Ok(new { Message = "Hồ sơ chưa được khởi tạo." });
            return Ok(profile);
        }

        // 3. Dashboard Thống kê (Recruiter)
        [HttpGet("dashboard")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetDashboardStats()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            var query = new GetRecruiterDashboardQuery(recruiterId);
            var stats = await _mediator.Send(query);

            return Ok(stats);
        }


        [HttpPost("{candidateId}/save")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> ToggleSaveCandidate(Guid candidateId)
        {
            var recruiterId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isSaved = await _mediator.Send(new ToggleSavedCandidateCommand(recruiterId, candidateId));

            return Ok(new { IsSaved = isSaved, Message = isSaved ? "Đã lưu ứng viên." : "Đã bỏ lưu ứng viên." });
        }
    }
}