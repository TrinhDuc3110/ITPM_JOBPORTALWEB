using System;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Interfaces
{
    public interface INotificationService
    {
        Task SendApplicationNotificationAsync(Guid recruiterId, string jobTitle, Guid jobSeekerId);
        Task NotifyJobCreatedAsync(string jobTitle);
        Task NotifyAdminNewRecruiterAsync(string email, string name);
    }
}