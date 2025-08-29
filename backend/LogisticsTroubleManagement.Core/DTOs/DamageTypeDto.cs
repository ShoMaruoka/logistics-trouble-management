namespace LogisticsTroubleManagement.Core.DTOs
{
    public class DamageTypeDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateDamageTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public int SortOrder { get; set; }
    }

    public class UpdateDamageTypeDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string Category { get; set; } = "General";
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
