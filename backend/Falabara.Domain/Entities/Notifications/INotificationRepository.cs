namespace Falabara.Domain.Entities
{
    public interface INotificationRepository
    {
        Task AddAsync(Notification notification);
        Task<List<Notification>> GetByUserIdAsync(Guid userId);
        Task MarkAllAsReadAsync(Guid userId);
    }
}