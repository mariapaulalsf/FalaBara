namespace Falabara.Domain.Entities
{
    public enum FeedbackType { Site = 0, Prefeitura = 1 }

    public class Feedback
    {
        public Guid Id { get; private set; }
        public Guid UserId { get; private set; }
        public string Message { get; private set; }
        public FeedbackType Type { get; private set; }
        public int Rating { get; private set; } 
        public DateTime CreatedAt { get; private set; }

        public Feedback(Guid userId, string message, FeedbackType type, int rating)
        {
            Id = Guid.NewGuid();
            UserId = userId;
            Message = message;
            Type = type;
            Rating = (rating < 1) ? 1 : (rating > 5) ? 5 : rating; 
            CreatedAt = DateTime.UtcNow;
        }
    }
}