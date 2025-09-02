using System;
using System.Collections.Generic;

namespace LogisticsTroubleManagement.Domain.Entities
{
    public class DamageType : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string Category { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<Incident> Incidents { get; private set; } = new List<Incident>();

        private DamageType() { }

        public DamageType(string name, string? description = null, 
            string category = "General", int sortOrder = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Category = category;
            SortOrder = sortOrder;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description, string category, int sortOrder)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Category = category;
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
