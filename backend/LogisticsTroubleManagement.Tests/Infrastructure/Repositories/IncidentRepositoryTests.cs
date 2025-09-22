using Xunit;
using Microsoft.EntityFrameworkCore;
using LogisticsTroubleManagement.Infrastructure.Data;
using LogisticsTroubleManagement.Infrastructure.Repositories;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;
using DomainEnums = LogisticsTroubleManagement.Domain.Enums;
using System.Reflection;

namespace LogisticsTroubleManagement.Tests.Infrastructure.Repositories
{
    public class IncidentRepositoryTests : IDisposable
    {
        private readonly ApplicationDbContext _context;
        private readonly IncidentRepository _repository;

        public IncidentRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _repository = new IncidentRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingId_ShouldReturnIncident()
        {
            // Arrange
            var incident = CreateTestIncident();
            await _context.Incidents.AddAsync(incident);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync(incident.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(incident.Id, result.Id);
            Assert.Equal(incident.Title, result.Title);
        }

        [Fact]
        public async Task GetByIdAsync_WithNonExistingId_ShouldReturnNull()
        {
            // Act
            var result = await _repository.GetByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllIncidents()
        {
            // Arrange
            var incidents = new List<Incident>
            {
                CreateTestIncident("インシデント1"),
                CreateTestIncident("インシデント2"),
                CreateTestIncident("インシデント3")
            };

            await _context.Incidents.AddRangeAsync(incidents);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        [Fact]
        public async Task AddAsync_ShouldAddIncident()
        {
            // Arrange
            var incident = CreateTestIncident();

            // Act
            await _repository.AddAsync(incident);
            await _context.SaveChangesAsync();

            // Assert
            var savedIncident = await _context.Incidents.FindAsync(incident.Id);
            Assert.NotNull(savedIncident);
            Assert.Equal(incident.Title, savedIncident.Title);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateIncident()
        {
            // Arrange
            var incident = CreateTestIncident();
            await _context.Incidents.AddAsync(incident);
            await _context.SaveChangesAsync();

            var updatedTitle = "更新されたタイトル";
            incident.UpdateDetails(updatedTitle, incident.Description, incident.Category);

            // Act
            await _repository.UpdateAsync(incident);
            await _context.SaveChangesAsync();

            // Assert
            var updatedIncident = await _context.Incidents.FindAsync(incident.Id);
            Assert.NotNull(updatedIncident);
            Assert.Equal(updatedTitle, updatedIncident.Title);
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteIncident()
        {
            // Arrange
            var incident = CreateTestIncident();
            await _context.Incidents.AddAsync(incident);
            await _context.SaveChangesAsync();

            // Act
            await _repository.DeleteAsync(incident);
            await _context.SaveChangesAsync();

            // Assert
            var deletedIncident = await _context.Incidents.FindAsync(incident.Id);
            Assert.Null(deletedIncident);
        }

        [Fact]
        public async Task GetByStatusAsync_ShouldReturnIncidentsWithStatus()
        {
            // Arrange
            var incident1 = CreateTestIncident("未対応1");
            incident1.UpdateStatus(IncidentStatus.Pending); // 新仕様：未対応に設定
            
            var incident2 = CreateTestIncident("未対応2");
            incident2.UpdateStatus(IncidentStatus.Pending); // 新仕様：未対応に設定

            var resolvedIncident = CreateTestIncident("解決済み");
            resolvedIncident.Resolve("解決しました");

            var incidents = new List<Incident> { incident1, incident2, resolvedIncident };

            await _context.Incidents.AddRangeAsync(incidents);
            await _context.SaveChangesAsync();

            // Act
            var pendingIncidents = await _repository.GetByStatusAsync(IncidentStatus.Pending); // 新仕様：未対応

            // Assert
            Assert.NotNull(pendingIncidents);
            Assert.Equal(2, pendingIncidents.Count());
            Assert.All(pendingIncidents, i => Assert.Equal(IncidentStatus.Pending, i.Status));
        }

        [Fact]
        public async Task GetByPriorityAsync_ShouldReturnIncidentsWithPriority()
        {
            // Arrange
            var incidents = new List<Incident>
            {
                CreateTestIncident("高優先度1", Priority.High),
                CreateTestIncident("高優先度2", Priority.High),
                CreateTestIncident("中優先度", Priority.Medium)
            };

            await _context.Incidents.AddRangeAsync(incidents);
            await _context.SaveChangesAsync();

            // Act
            var highPriorityIncidents = await _repository.GetByPriorityAsync(Priority.High);

            // Assert
            Assert.NotNull(highPriorityIncidents);
            Assert.Equal(2, highPriorityIncidents.Count());
            Assert.All(highPriorityIncidents, i => Assert.Equal(Priority.High, i.Priority));
        }

        [Fact]
        public async Task GetOverdueIncidentsAsync_ShouldReturnOverdueIncidents()
        {
            // Arrange
            var overdueIncident = CreateTestIncident("期限切れ");
            var normalIncident = CreateTestIncident("通常");

            // Use reflection to set private property for testing
            var reportedDateProperty = typeof(Incident).GetProperty("ReportedDate", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            var setter = reportedDateProperty?.GetSetMethod(true);
            setter?.Invoke(overdueIncident, new object[] { DateTime.UtcNow.AddDays(-8) });

            var incidents = new List<Incident> { overdueIncident, normalIncident };
            await _context.Incidents.AddRangeAsync(incidents);
            await _context.SaveChangesAsync();

            var expectedResolutionTime = TimeSpan.FromDays(7);

            // Act
            var overdueIncidents = await _repository.GetOverdueIncidentsAsync(expectedResolutionTime);

            // Assert
            Assert.NotNull(overdueIncidents);
            Assert.Single(overdueIncidents);
            Assert.Equal("期限切れ", overdueIncidents.First().Title);
        }

        [Fact]
        public async Task GetByAssignedToAsync_ShouldReturnAssignedIncidents()
        {
            // Arrange
            var user = CreateTestUser();
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            var assignedIncident = CreateTestIncident("割り当て済み");
            assignedIncident.AssignTo(user.Id);

            var unassignedIncident = CreateTestIncident("未割り当て");

            var incidents = new List<Incident> { assignedIncident, unassignedIncident };
            await _context.Incidents.AddRangeAsync(incidents);
            await _context.SaveChangesAsync();

            // Act
            var assignedIncidents = await _repository.GetByAssignedToAsync(user.Id);

            // Assert
            Assert.NotNull(assignedIncidents);
            Assert.Single(assignedIncidents);
            Assert.Equal("割り当て済み", assignedIncidents.First().Title);
        }

        private static User CreateTestUser(int id = 1, string email = "test@example.com")
        {
            return User.Create(
                "testuser",
                email,
                "Test",
                "User",
                1 // Default role ID (Clerk)
            );
        }

        private static Incident CreateTestIncident(string title = "テストインシデント", Priority priority = Priority.Medium)
        {
            return Incident.Create(title, "テスト用のインシデント", "テスト", 1, 
                (int)DomainEnums.TroubleType.ProductTrouble, (int)DomainEnums.DamageType.DamageOrContamination, 
                (int)DomainEnums.Warehouse.WarehouseA, (int)DomainEnums.ShippingCompany.InHouse, DateTime.UtcNow, priority);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
