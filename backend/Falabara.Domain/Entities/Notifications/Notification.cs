using System;

namespace Falabara.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; } 
        public string Message { get; private set; }
        public bool IsRead { get; private set; }
        public DateTime CreatedAt { get; private set; }

        protected Notification() { }

        public Notification(Guid userId, string message)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Message = message;
            IsRead = false;
            CreatedAt = DateTime.UtcNow;
        }

        public void MarkAsRead()
        {
            IsRead = true;
        }
    }
}