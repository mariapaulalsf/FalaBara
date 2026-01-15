namespace Falabara.Application.DTOs
{
    public class DashboardDto
    {
        public int TotalComplaints { get; set; }
        public int TotalResolved { get; set; }
        public int TotalInAnalysis { get; set; }
        public List<ChartData> ComplaintsByCategory { get; set; } = new();
        public List<ChartData> ComplaintsByStatus { get; set; } = new();
    }

    public class ChartData
    {
        public string Label { get; set; } 
        public int Value { get; set; }   
    }
}