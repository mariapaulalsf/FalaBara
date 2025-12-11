using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Falabara.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; } /

        public string PasswordHash { get; set; }

        public UserType Type { get; set; }

        public string? Cpf { get; set; } 

        public string? Department { get; set; } 
        public string? EmployeeName { get; set; } 

        public virtual ICollection<Complaint> Complaints { get; set; } = new List<Complaint>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
    }
}