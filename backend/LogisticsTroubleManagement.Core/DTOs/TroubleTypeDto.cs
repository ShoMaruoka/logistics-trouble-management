namespace LogisticsTroubleManagement.Core.DTOs
{
    public class TroubleTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#3B82F6";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateTroubleTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#3B82F6";
        public int SortOrder { get; set; }
    }

    public class UpdateTroubleTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Color { get; set; } = "#3B82F6";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
