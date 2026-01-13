using System;

namespace Falabara.Domain.Entities
{
    public class Vote
    {
        public Guid Id { get; private set; }
        public Guid ComplaintId { get; private set; } 
        public Guid UserId { get; private set; }   
        public bool IsLike { get; private set; }// True = Like, False = Dislike

        protected Vote() { }

        public Vote(Guid complaintId, Guid userId, bool isLike)
        {
            Id = Guid.NewGuid();
            ComplaintId = complaintId;
            UserId = userId;
            IsLike = isLike;
        }

        public void UpdateVote(bool isLike)
        {
            IsLike = isLike;
        }
    }
}