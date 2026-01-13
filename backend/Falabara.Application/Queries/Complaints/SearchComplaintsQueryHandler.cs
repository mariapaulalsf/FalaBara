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
            string sql = @"
            SELECT 
                c.""Id"",
                c.""Title"",
                c.""Description"",
                c.""Neighborhood"",
                c.""CreatedAt"",
                c.""OfficialResponse"",
                u.""name"" as AuthorName,
                
                -- Tradução do Status
                CASE 
                    WHEN c.""Status"" = 0 THEN 'Aberto'
                    WHEN c.""Status"" = 1 THEN 'Em Análise'
                    WHEN c.""Status"" = 2 THEN 'Em Andamento'
                    WHEN c.""Status"" = 3 THEN 'Resolvido'
                    ELSE 'Cancelado'
                END as StatusName,

                -- Tradução da Categoria (Simplificado)
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
                    c.""Description"" ILIKE '%' || @Search || '%' OR
                    c.""Neighborhood"" ILIKE '%' || @Search || '%'
                ))
                AND (@Category IS NULL OR c.""Category"" = @Category)
                AND (@Status IS NULL OR c.""Status"" = @Status)
            ORDER BY c.""CreatedAt"" DESC
            LIMIT @PerPage OFFSET @Offset";

            var result = await _dbConnection.QueryAsync<SearchComplaintsQueryResponse.ComplaintDto>(
                sql, 
                new { 
                    Search = request.Search, 
                    Category = request.Category,
                    Status = request.Status,
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