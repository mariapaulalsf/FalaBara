using MediatR;
using Dapper;
using System.Data; 
using Falabara.Application.DTOs;

namespace Falabara.Application.Queries.Dashboard
{
    public class GetDashboardMetricsQuery : IRequest<DashboardDto>
    {
    }

    public class GetDashboardMetricsHandler : IRequestHandler<GetDashboardMetricsQuery, DashboardDto>
    {
        private readonly IDbConnection _dbConnection; 
        public GetDashboardMetricsHandler(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        public async Task<DashboardDto> Handle(GetDashboardMetricsQuery request, CancellationToken cancellationToken)
        {
            var result = new DashboardDto();

            if (_dbConnection.State != ConnectionState.Open)
                _dbConnection.Open();

            // SQL 1: Contagem Geral 
            var sqlTotals = @"
                SELECT 
                    COUNT(*) as Total,
                    COUNT(CASE WHEN ""Status"" = 3 THEN 1 END) as Resolvido, 
                    COUNT(CASE WHEN ""Status"" = 1 THEN 1 END) as EmAnalise
                FROM ""Complaints"";";
            
            var totals = await _dbConnection.QueryFirstOrDefaultAsync<dynamic>(sqlTotals);
            
            if (totals != null)
            {
                result.TotalComplaints = (int)totals.total;
                result.TotalResolved = (int)totals.resolvido;
                result.TotalInAnalysis = (int)totals.emanalise;
            }
            // SQL 2: Gráfico por Categoria (Barras)
            var sqlCategory = @"
                SELECT ""Category"" as Label, COUNT(*) as Value 
                FROM ""Complaints"" 
                GROUP BY ""Category"";";
            
            var categories = await _dbConnection.QueryAsync<ChartData>(sqlCategory);
            result.ComplaintsByCategory = categories.ToList();

            // SQL 3: Gráfico por Status (Pizza)
            var sqlStatus = @"
                SELECT CAST(""Status"" AS TEXT) as Label, COUNT(*) as Value 
                FROM ""Complaints"" 
                GROUP BY ""Status"";";

            var statusData = await _dbConnection.QueryAsync<ChartData>(sqlStatus);
            result.ComplaintsByStatus = statusData.ToList();

            return result;
        }
    }
}