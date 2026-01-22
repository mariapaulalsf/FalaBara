using Dapper;
using MediatR;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Falabara.Application.Queries.Complaint
{
    public class SearchComplaintsQueryHandler : IRequestHandler<SearchComplaintsQuery, SearchComplaintsQueryResponse>
    {
        private readonly IDbConnection _dbConnection;

        public SearchComplaintsQueryHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<SearchComplaintsQueryResponse> Handle(SearchComplaintsQuery request, CancellationToken cancellationToken)
        {
            int offset = (request.Page - 1) * request.PerPage;

            string orderByClause = "ORDER BY c.\"CreatedAt\" DESC";

            if (request.OrderBy == "votes")
            {
                orderByClause = "ORDER BY LikesCount DESC, c.\"CreatedAt\" DESC";
            }

            string sql = $@"
            SELECT 
                c.""Id"",
                c.""Title"",
                c.""Description"",
                c.""Neighborhood"",
                c.""Location"",  
                c.""Latitude"",   
                c.""Longitude"",  
                c.""ImageUrl"",
                c.""CreatedAt"",
                c.""OfficialResponse"",
                u.""name"" as AuthorName,
                c.""UserId"" as AuthorId,
                (SELECT COUNT(*) FROM ""Votes"" v WHERE v.""ComplaintId"" = c.""Id"" AND v.""IsLike"" = true) as LikesCount,
                CASE WHEN EXISTS (
                    SELECT 1 FROM ""Votes"" v 
                    WHERE v.""ComplaintId"" = c.""Id"" 
                    AND v.""UserId"" = @CurrentUserId 
                    AND v.""IsLike"" = true
                ) THEN true ELSE false END as IsLikedByCurrentUser,

                CASE 
                    WHEN c.""Status"" = 0 THEN 'Aberto'
                    WHEN c.""Status"" = 1 THEN 'Em Análise'
                    WHEN c.""Status"" = 2 THEN 'Em Andamento'
                    WHEN c.""Status"" = 3 THEN 'Resolvido'
                    ELSE 'Cancelado'
                END as StatusName,

                CASE 
                    WHEN c.""Category"" = 0 THEN 'Saúde'
                    WHEN c.""Category"" = 1 THEN 'Obras/Infraestrutura'
                    WHEN c.""Category"" = 2 THEN 'Trânsito'
                    WHEN c.""Category"" = 3 THEN 'Iluminação'
                    WHEN c.""Category"" = 4 THEN 'Limpeza'
                    WHEN c.""Category"" = 5 THEN 'Segurança'
                    WHEN c.""Category"" = 6 THEN 'Educação'
                    WHEN c.""Category"" = 7 THEN 'Meio Ambiente'
                    ELSE 'Outros'
                END as CategoryName,

                COUNT(*) OVER() as totalitems
            FROM ""Complaints"" c
            JOIN ""Users"" u ON c.""UserId"" = u.""id""
            WHERE 
                (@Search IS NULL OR (
                    c.""Title"" ILIKE '%' || @Search || '%' OR
                    c.""Description"" ILIKE '%' || @Search || '%' 
                ))
                AND (@Neighborhood IS NULL OR c.""Neighborhood"" ILIKE '%' || @Neighborhood || '%')
                AND (@Category IS NULL OR c.""Category"" = @Category)
                AND (@Status IS NULL OR c.""Status"" = @Status)
                AND (@UserId IS NULL OR c.""UserId"" = @UserId)

            {orderByClause}
            
            LIMIT @PerPage OFFSET @Offset";
            
            var result = await _dbConnection.QueryAsync<SearchComplaintsQueryResponse.ComplaintDto>(
                sql,
                new
                {
                    Search = request.Search,
                    Neighborhood = request.Neighborhood,
                    Category = request.Category,
                    Status = request.Status,
                    UserId = request.UserId,
                    CurrentUserId = request.CurrentUserId, 
                    PerPage = request.PerPage,
                    Offset = offset
                });

            var list = result.ToList();

            return new SearchComplaintsQueryResponse()
            {
                Data = list,
                Length = list.Count,
                TotalItems = list.Count > 0 ? list.First().TotalItems : 0
            };
        }
    }
}