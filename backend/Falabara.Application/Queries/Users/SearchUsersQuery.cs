using MediatR;
using System.Collections.Generic;

namespace Falabara.Application.Queries.User
{
    public class SearchUsersQuery : IRequest<SearchUsersQueryResponse>
    {
        public string? Search { get; set; }
        public int? Type { get; set; }
        public int Page { get; set; }
        public int PerPage { get; set; }

        public SearchUsersQuery(string? search, int? type, int page, int perPage)
        {
            Search = search;
            Type = type;
            Page = page <= 0 ? 1 : page;
            PerPage = perPage <= 0 ? 10 : perPage;
        }
    }

    public class SearchUsersQueryResponse
    {
        public List<UserDto> Data { get; set; } = new();
        public int TotalItems { get; set; }
    }
}