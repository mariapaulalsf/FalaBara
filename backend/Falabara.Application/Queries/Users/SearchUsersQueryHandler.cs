using Dapper;
using MediatR;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Falabara.Domain.Entities;

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
            id, name, email, cpf, user_type, department, fone_number, active,
            COUNT(*) OVER() as TotalItems
        FROM ""Users""
        WHERE 
            (@Type IS NULL OR user_type = @Type) AND -- NOVO FILTRO
            (@Search IS NULL OR (
                name ILIKE '%' || @Search || '%' OR
                email ILIKE '%' || @Search || '%' OR
                cpf ILIKE '%' || @Search || '%'
            ))
        ORDER BY name ASC
        LIMIT @PerPage OFFSET @Offset";

            var result = await _dbConnection.QueryAsync<dynamic>(
                sql,
                new
                {
                    Search = request.Search,
                    Type = request.Type, 
                    PerPage = request.PerPage,
                    Offset = offset
                }
            );
            var list = result.Select(x => new UserDto
            {
                Id = x.id,
                Name = x.name,
                Email = x.email,
                Cpf = x.cpf,
                Type = (UserType)x.user_type,
                Department = x.department,
                FoneNumber = x.fone_number,
                Active = x.active
            }).ToList();

            int totalItems = result.Any() ? (int)result.First().totalitems : 0;

            return new SearchUsersQueryResponse
            {
                Data = list,
                TotalItems = totalItems
            };
        }
    }
}