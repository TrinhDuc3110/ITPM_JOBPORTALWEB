using JOBPORTALWEB.APPLICATION.DTOs.Job;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.ApplyJob;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.CloseJob;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.CreateJob;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.DeleteJob;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.ToggleSavedJob;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Commands.UpdateJob;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.DownloadCV;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetApplications;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobDetail;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetJobList;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetRecruiterJobs;
using JOBPORTALWEB.APPLICATION.Features.Jobs.Queries.GetSavedJobs;
using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.INFRASTRUCTURE.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ITPM_JOBPORTALWEB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class JobsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IJobRepository _jobRepository;

        public JobsController(IMediator mediator, IJobRepository jobRepository)
        {
            _mediator = mediator;
            _jobRepository = jobRepository;
        }

        // 1. Tạo Job (Recruiter)
        [HttpPost]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CreateJob([FromBody] CreateJobRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized(new { Message = "ID không hợp lệ." });

            var command = new CreateJobCommand(request, recruiterId);
            var jobId = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetJobById), new { id = jobId }, new { JobId = jobId, Message = "Tạo thành công." });
        }

        // 2. Sửa Job (Recruiter)
        [HttpPut("{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> UpdateJob(int id, [FromBody] UpdateJobRequest request)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            try
            {
                var command = new UpdateJobCommand(id, request, recruiterId);
                var result = await _mediator.Send(command);

                if (!result) return NotFound(new { Message = "Không tìm thấy Job." });
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
        }

        // 3. Xóa Job (Recruiter)
        [HttpDelete("{id}")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DeleteJob(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            try
            {
                var command = new DeleteJobCommand(id, recruiterId);
                var result = await _mediator.Send(command);

                if (!result) return NotFound(new { Message = "Không tìm thấy Job." });
                return NoContent();
            }
            catch (UnauthorizedAccessException)
            {
                return StatusCode(403, new { Message = "Không có quyền xóa." });
            }
        }

        // 4. Nộp CV (JobSeeker)
        [HttpPost("{jobId}/apply")]
        [Authorize(Roles = "jobSeeker")]
        public async Task<IActionResult> ApplyForJob(
                int jobId,
                [FromForm] ApplyJobRequest request,
                IFormFile cvFile,
                [FromServices] IFileStorageService fileStorageService)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid jobSeekerId))
                return Unauthorized(new { Message = "ID không hợp lệ." });

            if (cvFile == null || cvFile.Length == 0)
                return BadRequest(new { Message = "Vui lòng đính kèm CV." });

            // Lưu file
            var folder = "Resumes";
            var fileName = $"{Guid.NewGuid()}_{cvFile.FileName}";
            string cvPath;

            using (var stream = cvFile.OpenReadStream())
            {
                cvPath = await fileStorageService.SaveFileAsync(stream, fileName, folder);
            }

            var command = new ApplyJobCommand(jobId, jobSeekerId, request, cvPath);
            var applicationId = await _mediator.Send(command);

            return Ok(new { ApplicationId = applicationId, Message = "Ứng tuyển thành công!" });
        }

        // 5. Xem Đơn Ứng tuyển (Recruiter)
        [HttpGet("applications")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetJobApplications()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            var query = new GetApplicationsQuery(recruiterId);
            var applications = await _mediator.Send(query);
            return Ok(applications);
        }

        // 6. Tải CV (Recruiter)
        [HttpGet("applications/{applicationId}/download-cv")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> DownloadCv(int applicationId, [FromServices] IFileStorageService fileStorageService)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            try
            {
                var query = new DownloadCVQuery(applicationId, recruiterId);
                var fileInfo = await _mediator.Send(query);

                if (fileInfo == null) return NotFound(new { Message = "Không tìm thấy hồ sơ." });

                var fileResult = await fileStorageService.GetFileAsync(fileInfo.Value.FilePath);
                if (fileResult == null) return NotFound(new { Message = "File không tồn tại trên server." });

                return File(fileResult.Value.Stream, fileResult.Value.ContentType, fileInfo.Value.FileName);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobById(int id)
        {
            Guid? currentUserId = null;
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid parsedId))
            {
                currentUserId = parsedId;
            }

            var query = new GetJobDetailQuery(id, currentUserId);
            var jobDetail = await _mediator.Send(query);

            if (jobDetail == null) return NotFound(new { Message = "Không tìm thấy công việc." });
            return Ok(jobDetail);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetJobsList([FromQuery] GetJobListQuery query)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!string.IsNullOrEmpty(userIdString) && Guid.TryParse(userIdString, out Guid userId))
            {
                query.CurrentUserId = userId;
            }

            var response = await _mediator.Send(query);
            return Ok(response);
        }

        [HttpPost("{id}/save")]
        [Authorize(Roles = "jobSeeker")]
        public async Task<IActionResult> ToggleSaveJob(int id)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var isSaved = await _mediator.Send(new ToggleSavedJobCommand(id, userId));

            return Ok(new { IsSaved = isSaved, Message = isSaved ? "Đã lưu công việc." : "Đã bỏ lưu công việc." });
        }

        // 2. Xem danh sách Job đã lưu
        [HttpGet("saved")]
        [Authorize(Roles = "jobSeeker")]
        public async Task<IActionResult> GetSavedJobs()
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var jobs = await _mediator.Send(new GetSavedJobsQuery(userId));
            return Ok(jobs);
        }

        // 9. Đóng Công việc (Close Job)
        [HttpPatch("{id}/close")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> CloseJob(int id)
        {
            // 1. Lấy ID User từ Token
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
            {
                return Unauthorized(new { Message = "Token không hợp lệ." });
            }

            try
            {
                // 2. Gửi Command xuống tầng Application
                var command = new CloseJobCommand(id, recruiterId);
                var result = await _mediator.Send(command);

                if (!result)
                {
                    return NotFound(new { Message = "Không tìm thấy công việc." });
                }

                // 3. Trả về thành công
                return Ok(new { Message = "Đã đóng công việc thành công." });
            }
            catch (UnauthorizedAccessException ex)
            {
                // Trả về 403 nếu không phải chủ sở hữu
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi hệ thống: " + ex.Message });
            }
        }

        [HttpGet("my-jobs")]
        [Authorize(Roles = "Recruiter")]
        public async Task<IActionResult> GetMyJobs([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdString == null || !Guid.TryParse(userIdString, out Guid recruiterId))
                return Unauthorized();

            var query = new GetRecruiterJobsQuery(recruiterId, pageNumber, pageSize);
            var result = await _mediator.Send(query);

            return Ok(result);
        }
    }
}