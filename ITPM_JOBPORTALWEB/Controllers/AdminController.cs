using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.DOMAIN.Enums;
using JOBPORTALWEB.INFRASTRUCTURE.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ITPM_JOBPORTALWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Thống kê tổng quan
        [HttpGet("dashboard-stats")]
        public async Task<IActionResult> GetStats()
        {
            var totalUsers = await _context.Users.CountAsync();
            var totalJobs = await _context.Jobs.CountAsync();
            var totalApplications = await _context.JobApplications.CountAsync();
            var pendingJobs = await _context.Jobs.CountAsync(j => j.Status == JobStatus.Draft);
            var pendingRecruiters = await _context.Users.CountAsync(u => u.Role == UserRole.Recruiter && !u.IsActive);

            return Ok(new
            {
                TotalUsers = totalUsers,
                TotalJobs = totalJobs,
                TotalApplications = totalApplications,
                PendingJobs = pendingJobs,
                PendingRecruiters = pendingRecruiters
            });
        }

        // 2. Quản lý Users (Lấy danh sách)
        [HttpGet("users")]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    Role = u.Role.ToString(),
                    u.EmailConfirmed
                })
                .ToListAsync();
            return Ok(users);
        }

        // 3. Xóa User (Ban)
        [HttpDelete("users/{id}")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            // Không cho phép xóa chính mình
            if (User.Identity.Name == user.UserName) return BadRequest("Không thể xóa chính mình!");

            await _userManager.DeleteAsync(user);
            return Ok(new { Message = "Đã xóa người dùng." });
        }

        // 4. Quản lý Jobs (Lấy tất cả)
        [HttpGet("jobs")]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _context.Jobs
                .Include(j => j.Company)
                .OrderByDescending(j => j.PostedDate)
                .Select(j => new
                {
                    j.Id,
                    j.Title,
                    Company = j.Company.Name != null ? j.Company.Name : "N/A",
                    j.Location,
                    j.PostedDate,
                    Status = j.Status.ToString()
                })
                .ToListAsync();
            return Ok(jobs);
        }

        // 5. Duyệt / Đóng Job
        [HttpPatch("jobs/{id}/status")]
        public async Task<IActionResult> ChangeJobStatus(int id, [FromBody] int status)
        {
            var job = await _context.Jobs.FindAsync(id);
            if (job == null) return NotFound();

            job.Status = (JobStatus)status;
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Cập nhật trạng thái thành công." });
        }

        [HttpGet("pending-recruiters")]
        public async Task<IActionResult> GetPendingRecruiters()
        {
            var users = await _context.Users
                .Where(u => u.Role == UserRole.Recruiter && u.IsActive == false) 
                .Select(u => new
                {
                    u.Id,
                    u.FullName,
                    u.Email,
                    u.CreatedAt,
                    CompanyName = u.Company != null ? u.Company.Name : "Chưa cập nhật"
                })
                .ToListAsync();
            return Ok(users);
        }

        // 7. Duyệt tài khoản (Approve)
        [HttpPatch("users/{id}/approve")]
        public async Task<IActionResult> ApproveUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            user.IsActive = true;
            await _userManager.UpdateAsync(user);

            return Ok(new { Message = "Đã duyệt tài khoản thành công." });
        }

        // 8. Từ chối (Xóa luôn)
        [HttpDelete("users/{id}/reject")]
        public async Task<IActionResult> RejectUser(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            await _userManager.DeleteAsync(user);
            return Ok(new { Message = "Đã từ chối và xóa yêu cầu." });
        }
    }
}