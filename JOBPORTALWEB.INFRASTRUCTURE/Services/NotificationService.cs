using JOBPORTALWEB.APPLICATION.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;


namespace JOBPORTALWEB.INFRASTRUCTURE.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendApplicationNotificationAsync(Guid recruiterId, string jobTitle, Guid candidateId)
        {
            var message = $"[Mới] Ứng viên {candidateId} đã nộp hồ sơ cho công việc: {jobTitle}";

            await _hubContext.Clients.User(recruiterId.ToString())
                .SendAsync("ReceiveApplicationNotification", message);
        }

        public async Task NotifyJobCreatedAsync(string jobTitle)
        {
            var message = $"Một công việc mới vừa được đăng: {jobTitle}. Hãy xem ngay!";
            await _hubContext.Clients.All.SendAsync("ReceiveJobUpdate", message);
        }
        public async Task NotifyAdminNewRecruiterAsync(string email, string name)
        {
            var message = $"🆕 Nhà tuyển dụng mới chờ duyệt: {name} ({email})";

            await _hubContext.Clients.Group("Admins")
                .SendAsync("ReceiveAdminNotification", message);
        }
    }
}