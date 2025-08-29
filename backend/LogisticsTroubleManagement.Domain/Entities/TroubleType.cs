using System;
using System.Collections.Generic;

namespace LogisticsTroubleManagement.Domain.Entities
{
    public class TroubleType : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public string Color { get; private set; }
        public int SortOrder { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public virtual ICollection<Incident> Incidents { get; private set; } = new List<Incident>();

        private TroubleType() { }

        public TroubleType(string name, string? description = null, 
            string color = "#3B82F6", int sortOrder = 0)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Color = color;
            SortOrder = sortOrder;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Update(string name, string? description, string color, int sortOrder)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description;
            Color = color;
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
