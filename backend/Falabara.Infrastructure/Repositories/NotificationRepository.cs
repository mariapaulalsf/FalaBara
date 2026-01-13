using Falabara.Domain.Entities;
using Falabara.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace Falabara.Infrastructure.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly FalabaraContext _context;

        public NotificationRepository(FalabaraContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Notification notification)
        {
            await _context.Notifications.AddAsync(notification);
            await _context.SaveChangesAsync(); 
        }

        public async Task<List<Notification>> GetByUserIdAsync(Guid userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
        
        public async Task MarkAllAsReadAsync(Guid userId)
        {
            var unread = await _context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var n in unread) n.MarkAsRead();
            await _context.SaveChangesAsync();
        }
    }
}