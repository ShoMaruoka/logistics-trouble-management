using System;
using System.Collections.Generic;

namespace LogisticsTroubleManagement.Domain.Entities
{
    public class ShippingCompany : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string CompanyType { get; private set; }
        public string? ContactInfo { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<Incident> Incidents { get; private set; } = new List<Incident>();

        private ShippingCompany() { }

        public ShippingCompany(string name, string? description = null, 
            string companyType = "External", string? contactInfo = null, int sortOrder = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            CompanyType = companyType;
            ContactInfo = contactInfo;
            SortOrder = sortOrder;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description, string companyType, 
            string? contactInfo, int sortOrder)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            CompanyType = companyType;
            ContactInfo = contactInfo;
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;
        }

        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
