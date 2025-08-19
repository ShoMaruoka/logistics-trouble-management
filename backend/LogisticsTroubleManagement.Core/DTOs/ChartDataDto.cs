namespace LogisticsTroubleManagement.Core.DTOs;

public class ChartDataDto
{
    public string ChartType { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public List<ChartSeriesDto> Series { get; set; } = new List<ChartSeriesDto>();
    public List<string> Labels { get; set; } = new List<string>();
}

public class ChartSeriesDto
{
    public string Name { get; set; } = string.Empty;
    public List<decimal> Data { get; set; } = new List<decimal>();
}

public class PieChartDataDto
{
    public string Title { get; set; } = string.Empty;
    public List<PieChartItemDto> Items { get; set; } = new List<PieChartItemDto>();
}

public class PieChartItemDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Color { get; set; } = string.Empty;
}
