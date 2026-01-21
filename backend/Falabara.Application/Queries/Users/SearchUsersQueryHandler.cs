using Dapper;
using MediatR;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Falabara.Application.Queries.User
{
    public class SearchUsersQueryHandler : IRequestHandler<SearchUsersQuery, SearchUsersQueryResponse>
    {
        private readonly IDbConnection _dbConnection;

        public SearchUsersQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<SearchUsersQueryResponse> Handle(SearchUsersQuery request, CancellationToken cancellationToken)
        {
            int offset = (request.Page - 1) * request.PerPage;

            string sql = @"
                SELECT 
                    ""Id"", 
                    ""Name"", 
                    ""Email"", 
                    ""Cpf"", 
                    ""Type"", 
                    ""Department"", 
                    ""FoneNumber"", 
                    ""Active"",
                    COUNT(*) OVER() as TotalItems
                FROM ""Users""
                WHERE 
                    (@Search IS NULL OR (
                        ""Name"" ILIKE '%' || @Search || '%' OR
                        ""Email"" ILIKE '%' || @Search || '%' OR
                        ""Cpf"" ILIKE '%' || @Search || '%'
                    ))
                ORDER BY ""Name"" ASC
                LIMIT @PerPage OFFSET @Offset";

            var result = await _dbConnection.QueryAsync<dynamic>(
                sql, 
                new { Search = request.Search, PerPage = request.PerPage, Offset = offset }
            );

            var list = result.Select(x => new UserDto
            {
                Id = x.Id,
                Name = x.Name,
                Email = x.Email,
                Cpf = x.Cpf,
                Type = (Domain.Entities.UserType)x.Type,
                Department = x.Department,
                FoneNumber = x.FoneNumber,
                Active = x.Active
            }).ToList();

            int totalItems = result.Any() ? (int)result.First().TotalItems : 0;

            return new SearchUsersQueryResponse
            {
                Data = list,
                TotalItems = totalItems
            };
        }
    }
}