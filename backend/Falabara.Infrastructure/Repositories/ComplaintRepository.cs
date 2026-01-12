using Falabara.Domain.Entities;
using Falabara.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Falabara.Infrastructure.Repositories
{
    public class ComplaintRepository : IComplaintRepository
    {
        private readonly FalabaraContext _context;

        public ComplaintRepository(FalabaraContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Complaint complaint)
        {
            await _context.Complaints.AddAsync(complaint);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Complaint>> GetAllAsync()
        {
            // Inclui os dados do Usuário (Include) para sabermos quem postou
            return await _context.Complaints
                .Include(c => c.User) 
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }

        public async Task<Complaint?> GetByIdAsync(Guid id)
        {
            return await _context.Complaints
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Complaint complaint)
        {
            _context.Complaints.Update(complaint);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Complaint complaint)
        {
            _context.Complaints.Remove(complaint);
            await _context.SaveChangesAsync();
        }
    }
}