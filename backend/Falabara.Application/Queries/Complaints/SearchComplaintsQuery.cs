using MediatR;
using System.Collections.Generic;
using Falabara.Domain.Entities; // Para acessar os Enums

namespace Falabara.Application.Queries.Complaint
{
    public class SearchComplaintsQuery : IRequest<SearchComplaintsQueryResponse>
    {
        public string? Search { get; set; } 

        public string? Neighborhood { get; set; }
        public ComplaintCategory? Category { get; set; }
        public ComplaintStatus? Status { get; set; }  

        public string OrderBy { get; set; }  

        public Guid? UserId { get; set; } 
        public Guid? CurrentUserId { get; set; } 
        
        public int Page { get; set; }
        public int PerPage { get; set; }

        public SearchComplaintsQuery(string? search, string? neighborhood, ComplaintCategory? category, ComplaintStatus? status, string orderBy, Guid? userId, Guid? currentUserId, int page, int perPage)
        {
            Search = search;
            Neighborhood = neighborhood;
            Category = category;
            Status = status;
            OrderBy = orderBy?.ToLower() ?? "date";
            UserId = userId; 
            CurrentUserId = currentUserId; 
            Page = page <= 0 ? 1 : page;
            PerPage = perPage <= 0 ? 10 : perPage;
        }
    }

    public class SearchComplaintsQueryResponse
    {
        public List<ComplaintDto> Data { get; set; } = new();
        public int Length { get; set; }
        public int TotalItems { get; set; }

        public class ComplaintDto
        {
            public Guid Id { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string? Neighborhood { get; set; } 
            public string? Location { get; set; }
            public double Latitude { get; set; }  
            public double Longitude { get; set; } 
            public string? ImageUrl { get; set; }
            public DateTime CreatedAt { get; set; }

            public string StatusName { get; set; }   
            public string CategoryName { get; set; } 

             public Guid AuthorId { get; set; }
            public string AuthorName { get; set; } 
            public int LikesCount { get; set; }
            public bool IsLikedByCurrentUser { get; set; }
            public string? OfficialResponse { get; set; }
            
            public int TotalItems { get; set; }
        }
    }
}