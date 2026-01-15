using System;

namespace Falabara.Domain.Entities
{
    public class Complaint
    {
        public Guid Id { get; private set; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public string? Location { get; private set; }
        public string? Neighborhood { get; private set; }
        public double Latitude { get; private set; }
        public double Longitude { get; private set; }
        public string? ImageUrl { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public Guid UserId { get; private set; }
        public User User { get; set; }

        public ComplaintStatus Status { get; private set; }
        public ComplaintCategory Category { get; private set; }
        public string? OfficialResponse { get; private set; }

        protected Complaint() { }
        public Complaint(string title, string description, string location, string neighborhood, double latitude, double longitude, string? imageUrl, ComplaintCategory category, Guid userId)
        {
            if (string.IsNullOrEmpty(title)) throw new Exception("O título é obrigatório.");
            if (string.IsNullOrEmpty(description)) throw new Exception("A descrição é obrigatória.");

            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Location = location;
            Neighborhood = neighborhood;
            Latitude = latitude;
            Longitude = longitude;
            ImageUrl = imageUrl;
            Category = category;
            UserId = userId;
            Status = ComplaintStatus.Aberto;
            CreatedAt = DateTime.UtcNow;
        }

        public void UpdateStatus(ComplaintStatus newStatus, string? response)
        {
            Status = newStatus;

            if (!string.IsNullOrEmpty(response))
            {
                OfficialResponse = response;
            }
        }
    }
}