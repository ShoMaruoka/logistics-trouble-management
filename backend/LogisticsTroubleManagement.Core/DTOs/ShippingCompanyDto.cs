namespace LogisticsTroubleManagement.Core.DTOs
{
    public class ShippingCompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CompanyType { get; set; } = "External";
        public string? ContactInfo { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }

    public class CreateShippingCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CompanyType { get; set; } = "External";
        public string? ContactInfo { get; set; }
        public int SortOrder { get; set; }
    }

    public class UpdateShippingCompanyDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string CompanyType { get; set; } = "External";
        public string? ContactInfo { get; set; }
        public int SortOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
