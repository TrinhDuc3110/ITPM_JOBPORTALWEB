using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


[Authorize]
public class NotificationHub : Hub
{
 
    public override async Task OnConnectedAsync()
    {
        var user = Context.User;

        // 1. Kiểm tra nếu User là Admin thì thêm vào nhóm "Admins"
        // Lưu ý: Chuỗi "Admin" phải khớp chính xác với Role bạn lưu trong Token
        if (user != null && (user.IsInRole("Admin") || user.HasClaim(ClaimTypes.Role, "Admin")))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        // 2. (Tùy chọn) Thêm User vào nhóm theo ID riêng của họ để gửi tin nhắn cá nhân
        // Mặc định SignalR đã hỗ trợ gửi theo UserIdentifier, nhưng việc group thủ công đôi khi linh hoạt hơn
        var userId = user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, userId);
        }

        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(System.Exception? exception)
    {
        var user = Context.User;

        if (user != null && (user.IsInRole("Admin") || user.HasClaim(ClaimTypes.Role, "Admin")))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }

        await base.OnDisconnectedAsync(exception);
    }
}