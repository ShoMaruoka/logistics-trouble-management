using Xunit;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;

namespace LogisticsTroubleManagement.Tests.Domain.Entities
{
    public class IncidentTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateIncident()
        {
            // Arrange
            var title = "配送遅延トラブル";
            var description = "商品の配送が予定より2日遅れている";
            var category = "配送";
            var priority = Priority.Medium;
            var reportedById = 1;

            // Act
            var incident = Incident.Create(title, description, category, reportedById, 
                TroubleType.DeliveryTrouble, DamageType.OtherDeliveryMistake, 
                Warehouse.WarehouseA, ShippingCompany.ATransport, priority);

            // Assert
            Assert.NotNull(incident);
            Assert.Equal(title, incident.Title);
            Assert.Equal(description, incident.Description);
            Assert.Equal(category, incident.Category);
            Assert.Equal(priority, incident.Priority);
            Assert.Equal(IncidentStatus.Open, incident.Status);
            Assert.Equal(reportedById, incident.ReportedById);
            Assert.NotEqual(default(DateTime), incident.ReportedDate);
            Assert.NotEqual(default(DateTime), incident.CreatedAt);
            Assert.NotEqual(default(DateTime), incident.UpdatedAt);
        }

        [Fact]
        public void AssignTo_WithValidUserId_ShouldAssignIncident()
        {
            // Arrange
            var incident = CreateTestIncident();
            var assignedUserId = 2;

            // Act
            incident.AssignTo(assignedUserId);

            // Assert
            Assert.Equal(assignedUserId, incident.AssignedToId);
            Assert.Equal(IncidentStatus.InProgress, incident.Status);
        }

        [Fact]
        public void UpdateStatus_WithValidStatus_ShouldUpdateStatus()
        {
            // Arrange
            var incident = CreateTestIncident();

            // Act
            incident.UpdateStatus(IncidentStatus.InProgress);

            // Assert
            Assert.Equal(IncidentStatus.InProgress, incident.Status);
            Assert.NotEqual(default(DateTime), incident.UpdatedAt);
        }

        [Fact]
        public void Resolve_WithResolution_ShouldResolveIncident()
        {
            // Arrange
            var incident = CreateTestIncident();
            var resolution = "配送業者に連絡し、再配送を手配した";

            // Act
            incident.Resolve(resolution);

            // Assert
            Assert.Equal(IncidentStatus.Resolved, incident.Status);
            Assert.Equal(resolution, incident.Resolution);
            Assert.NotNull(incident.ResolvedDate);
            Assert.NotEqual(default(DateTime), incident.UpdatedAt);
        }

        [Fact]
        public void UpdatePriority_WithValidPriority_ShouldUpdatePriority()
        {
            // Arrange
            var incident = CreateTestIncident();

            // Act
            incident.UpdatePriority(Priority.High);

            // Assert
            Assert.Equal(Priority.High, incident.Priority);
            Assert.NotEqual(default(DateTime), incident.UpdatedAt);
        }

        [Fact]
        public void IsOverdue_WhenOverdue_ShouldReturnTrue()
        {
            // Arrange
            var incident = CreateTestIncident();
            var expectedResolutionTime = TimeSpan.FromDays(7);
            
            // Use reflection to set private property for testing
            var reportedDateProperty = typeof(Incident).GetProperty("ReportedDate");
            reportedDateProperty?.SetValue(incident, DateTime.UtcNow.AddDays(-8));

            // Act
            var isOverdue = incident.IsOverdue(expectedResolutionTime);

            // Assert
            Assert.True(isOverdue);
        }

        [Fact]
        public void IsOverdue_WhenNotOverdue_ShouldReturnFalse()
        {
            // Arrange
            var incident = CreateTestIncident();
            var expectedResolutionTime = TimeSpan.FromDays(30);

            // Act
            var isOverdue = incident.IsOverdue(expectedResolutionTime);

            // Assert
            Assert.False(isOverdue);
        }

        [Fact]
        public void GetResolutionTime_WhenResolved_ShouldReturnResolutionTime()
        {
            // Arrange
            var incident = CreateTestIncident();
            var reportedDate = DateTime.UtcNow.AddDays(-5);
            var resolvedDate = DateTime.UtcNow.AddDays(-1);
            
            // Use reflection to set private properties for testing
            var reportedDateProperty = typeof(Incident).GetProperty("ReportedDate");
            var resolvedDateProperty = typeof(Incident).GetProperty("ResolvedDate");
            
            reportedDateProperty?.SetValue(incident, reportedDate);
            resolvedDateProperty?.SetValue(incident, resolvedDate);

            // Act
            var resolutionTime = incident.GetResolutionTime();

            // Assert
            Assert.NotEqual(TimeSpan.Zero, resolutionTime);
            Assert.Equal(4, resolutionTime.Days);
        }

        [Fact]
        public void GetResolutionTime_WhenNotResolved_ShouldReturnZero()
        {
            // Arrange
            var incident = CreateTestIncident();

            // Act
            var resolutionTime = incident.GetResolutionTime();

            // Assert
            Assert.Equal(TimeSpan.Zero, resolutionTime);
        }

        private static Incident CreateTestIncident()
        {
            return Incident.Create(
                "テストインシデント",
                "テスト用のインシデント",
                "テスト",
                1,
                TroubleType.ProductTrouble,
                DamageType.DamageOrContamination,
                Warehouse.WarehouseA,
                ShippingCompany.InHouse,
                Priority.Medium
            );
        }
    }
}
