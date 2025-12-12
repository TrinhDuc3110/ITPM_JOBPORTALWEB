using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace ITPM_JOBPORTALWEB.Hubs
{
    // Lớp Hub cơ bản để định nghĩa các phương thức giao tiếp
    public class NotificationHub : Hub
    {
        // Clients có thể gọi phương thức này
        public async Task SendMessage(string user, string message)
        {
            // Server gửi tin nhắn đến tất cả các Clients khác
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}