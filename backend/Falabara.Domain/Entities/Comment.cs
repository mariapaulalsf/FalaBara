using System;
using System.ComponentModel.DataAnnotations;

namespace Falabara.Domain.Entities
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public string Content { get; set; }

        public string? PhotoUrl { get; set; } 

        public DateTime CreatedAt { get; set; } 

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public int ComplaintId { get; set; }
        public virtual Complaint Complaint { get; set; }
    }
}