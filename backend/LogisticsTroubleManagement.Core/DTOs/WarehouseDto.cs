namespace LogisticsTroubleManagement.Core.DTOs
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public int SortOrder { get; set; }
    }

    public class UpdateWarehouseDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Location { get; set; }
        public string? ContactInfo { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
