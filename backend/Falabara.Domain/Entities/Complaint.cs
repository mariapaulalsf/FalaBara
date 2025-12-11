using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Falabara.Domain.Entities
{
    public class Complaint
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; } 

        public string Location { get; set; } 

        public string Neighborhood { get; set; } 

        public string? PhotoUrl { get; set; } 

        public ComplaintCategory Category { get; set; }
        public Severity Severity { get; set; }
        public ComplaintStatus Status { get; set; } 

        public DateTime CreatedAt { get; set; } 
        public DateTime? ResolvedAt { get; set; } 

        public int UpVotes { get; set; }
        public int DownVotes { get; set; } 

        public int UserId { get; set; }
        public virtual User User { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}