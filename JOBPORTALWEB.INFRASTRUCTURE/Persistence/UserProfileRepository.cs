using JOBPORTALWEB.APPLICATION.Interfaces;
using JOBPORTALWEB.DOMAIN.Entities;
using JOBPORTALWEB.INFRASTRUCTURE.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace JOBPORTALWEB.INFRASTRUCTURE.Persistence
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public UserProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<UserProfile?> GetProfileByUserIdAsync(Guid userId)
        {
            return await _context.UserProfiles.FindAsync(userId);
        }

        public async Task<UserProfile> AddProfileAsync(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
            return profile;
        }

        public async Task<bool> UpdateProfileAsync(UserProfile profile)
        {
            _context.UserProfiles.Update(profile);
            _context.Entry(profile).Property(p => p.UserId).IsModified = false;

            var rowsAffected = await _context.SaveChangesAsync();
            return rowsAffected > 0;
        }

        public async Task<bool> ToggleSavedCandidateAsync(Guid recruiterId, Guid candidateId)
        {
            var existing = await _context.SavedCandidates
                .FirstOrDefaultAsync(s => s.RecruiterId == recruiterId && s.CandidateId == candidateId);

            if (existing != null)
            {
                _context.SavedCandidates.Remove(existing);
                await _context.SaveChangesAsync();
                return false; // Unsaved
            }

            var saved = new SavedCandidate { RecruiterId = recruiterId, CandidateId = candidateId };
            _context.SavedCandidates.Add(saved);
            await _context.SaveChangesAsync();
            return true; // Saved
        }

        public async Task<List<UserProfile>> GetSavedCandidatesByRecruiterIdAsync(Guid recruiterId)
        {
            // Lấy list UserProfile của những người đã được lưu
            return await _context.SavedCandidates
                .Where(s => s.RecruiterId == recruiterId)
                .Include(s => s.Candidate).ThenInclude(u => u.UserProfile)
                .Select(s => s.Candidate.UserProfile) // Trả về UserProfile
                .ToListAsync();
        }
    }
}