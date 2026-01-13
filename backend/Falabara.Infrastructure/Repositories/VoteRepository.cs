using Falabara.Domain.Entities;
using Falabara.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Falabara.Infrastructure.Repositories
{
    public class VoteRepository : IVoteRepository
    {
        private readonly FalabaraContext _context;

        public VoteRepository(FalabaraContext context)
        {
            _context = context;
        }

        public async Task<Vote?> GetByUserAndComplaintAsync(Guid userId, Guid complaintId)
        {
            return await _context.Votes
                .FirstOrDefaultAsync(v => v.UserId == userId && v.ComplaintId == complaintId);
        }

        public async Task AddAsync(Vote vote)
        {
            await _context.Votes.AddAsync(vote);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(Vote vote)
        {
            _context.Votes.Remove(vote);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Vote vote)
        {
            _context.Votes.Update(vote);
            await _context.SaveChangesAsync();
        }
                public async Task<int> GetLikesCountAsync(Guid complaintId)
        {
            return await _context.Votes
                .CountAsync(v => v.ComplaintId == complaintId && v.IsLike);
        }
    }
}