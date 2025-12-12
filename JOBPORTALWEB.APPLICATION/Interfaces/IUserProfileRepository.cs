using JOBPORTALWEB.DOMAIN.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JOBPORTALWEB.APPLICATION.Interfaces
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetProfileByUserIdAsync(Guid userId);
        Task<UserProfile> AddProfileAsync(UserProfile profile);
        Task<bool> UpdateProfileAsync(UserProfile profile);
        Task<bool> ToggleSavedCandidateAsync(Guid recruiterId, Guid candidateId);
        Task<List<UserProfile>> GetSavedCandidatesByRecruiterIdAsync(Guid recruiterId);
    }
}
