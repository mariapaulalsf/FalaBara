using System.ComponentModel.DataAnnotations;

namespace Falabara.Domain.Entities
{
    public class Vote
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ComplaintId { get; set; }
        public virtual Complaint Complaint { get; set; }

        public bool IsUpVote { get; set; } 
    }
}