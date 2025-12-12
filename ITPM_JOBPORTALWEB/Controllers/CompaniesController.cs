using JOBPORTALWEB.APPLICATION.DTOs.Company;
using JOBPORTALWEB.APPLICATION.Features.Companies.Commands.CreateCompany;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.INFRASTRUCTURE.Data;
using JOBPORTALWEB.APPLICATION.Interfaces; 
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace ITPM_JOBPORTALWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CompaniesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public CompaniesController(IMediator mediator, UserManager<User> userManager, ApplicationDbContext context)
        {
            _mediator = mediator;
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateCompany(
            [FromForm] CreateCompanyRequest request,
            IFormFile? logoFile,                  
            [FromServices] IFileStorageService fileService)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            // Kiểm tra xem user đã có công ty chưa
            var user = await _userManager.FindByIdAsync(userIdString);
            if (user.CompanyId != null)
            {
                return BadRequest(new { Message = "Bạn đã có hồ sơ công ty. Vui lòng dùng chức năng Cập nhật." });
            }

            // --- XỬ LÝ UPLOAD LOGO (NẾU CÓ) ---
            string logoPath = "";
            if (logoFile != null && logoFile.Length > 0)
            {
                var folder = "CompanyLogos";
                var fileName = $"logo_{recruiterId}_{Guid.NewGuid()}{Path.GetExtension(logoFile.FileName)}";
                logoPath = await fileService.SaveFileAsync(logoFile.OpenReadStream(), fileName, folder);
            }

            var command = new CreateCompanyCommand(request, recruiterId);
            var companyId = await _mediator.Send(command);

            // Nếu có logo, cập nhật lại vào DB ngay lập tức
            if (!string.IsNullOrEmpty(logoPath))
            {
                var newCompany = await _context.Companies.FindAsync(companyId);
                if (newCompany != null)
                {
                    newCompany.LogoUrl = logoPath;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { CompanyId = companyId, Message = "Tạo hồ sơ công ty thành công!" });
        }

        // 2. Lấy Công ty của tôi (GET)
        [HttpGet("my-company")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetMyCompany()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || user.CompanyId == null)
            {
                return NotFound(new { Message = "Chưa có hồ sơ công ty." });
            }

            var company = await _context.Companies.FindAsync(user.CompanyId);
            return Ok(company);
        }

        // 3. Cập nhật Công ty (PUT) - Đã chuẩn [FromForm]
        [HttpPut("my-company")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateCompany(
            [FromForm] CreateCompanyRequest request,
            IFormFile? logoFile,
            [FromServices] IFileStorageService fileService)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null) return Unauthorized();

            var user = await _userManager.FindByIdAsync(userIdString);
            if (user == null || user.CompanyId == null)
            {
                return NotFound(new { Message = "Bạn chưa tạo công ty." });
            }

            var company = await _context.Companies.FindAsync(user.CompanyId);
            if (company == null) return NotFound();

            // Cập nhật thông tin
            company.Name = request.Name;
            company.Website = request.Website;
            company.Description = request.Description;
            company.Location = request.Location;

            // Xử lý Upload Logo
            if (logoFile != null && logoFile.Length > 0)
            {
                var folder = "CompanyLogos";
                var fileName = $"{company.Id}_{Guid.NewGuid()}{Path.GetExtension(logoFile.FileName)}";
                var logoPath = await fileService.SaveFileAsync(logoFile.OpenReadStream(), fileName, folder);

                company.LogoUrl = logoPath;
            }

            await _context.SaveChangesAsync();

            return Ok(new { Message = "Cập nhật thành công!", LogoUrl = company.LogoUrl });
        }
    }
}