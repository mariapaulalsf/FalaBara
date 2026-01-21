using Dapper;
using MediatR;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Falabara.Application.Queries.User
{
    public class GetUserByIdQueryHandler : IRequestHandler<GetUserByIdQuery, UserDto>
    {
        private readonly IDbConnection _dbConnection;

        public GetUserByIdQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            string sql = @"
                SELECT 
                    ""Id"", ""Name"", ""Email"", ""Cpf"", ""Type"", ""Department"", ""FoneNumber"", ""Active""
                FROM ""Users""
                WHERE ""Id"" = @Id";

            return await _dbConnection.QueryFirstOrDefaultAsync<UserDto>(sql, new { Id = request.Id });
        }
    }
}