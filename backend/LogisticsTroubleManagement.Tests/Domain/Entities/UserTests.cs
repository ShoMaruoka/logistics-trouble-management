using Xunit;
using LogisticsTroubleManagement.Domain.Entities;
using LogisticsTroubleManagement.Domain.Enums;
using LogisticsTroubleManagement.Domain.ValueObjects;

namespace LogisticsTroubleManagement.Tests.Domain.Entities
{
    public class UserTests
    {
        [Fact]
        public void Create_WithValidData_ShouldCreateUser()
        {
            // Arrange
            var username = "testuser";
            var email = "test@example.com";
            var firstName = "Test";
            var lastName = "User";
            var roleId = 1; // Clerk role ID

            // Act
            var user = User.Create(username, email, firstName, lastName, roleId);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            Assert.Equal(email, user.Email.Value);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(roleId, user.RoleId);
            Assert.True(user.IsActive);
            Assert.NotEqual(default(DateTime), user.CreatedAt);
            Assert.NotEqual(default(DateTime), user.UpdatedAt);
        }

        [Fact]
        public void UpdateProfile_WithValidData_ShouldUpdateProfile()
        {
            // Arrange
            var user = CreateTestUser();
            var newFirstName = "Updated";
            var newLastName = "Name";
            var newPhoneNumber = "090-1234-5678";

            // Act
            user.UpdateProfile(newFirstName, newLastName, newPhoneNumber);

            // Assert
            Assert.Equal(newFirstName, user.FirstName);
            Assert.Equal(newLastName, user.LastName);
            Assert.NotNull(user.PhoneNumber);
            Assert.Equal("09012345678", user.PhoneNumber.Value);
        }

        [Fact]
        public void UpdateEmail_WithValidEmail_ShouldUpdateEmail()
        {
            // Arrange
            var user = CreateTestUser();
            var newEmail = "updated@example.com";

            // Act
            user.UpdateEmail(newEmail);

            // Assert
            Assert.Equal(newEmail, user.Email.Value);
        }

        [Fact]
        public void UpdateRole_WithValidRoleId_ShouldUpdateRole()
        {
            // Arrange
            var user = CreateTestUser();
            var newRoleId = 2; // Manager role ID

            // Act
            user.UpdateRole(newRoleId);

            // Assert
            Assert.Equal(newRoleId, user.RoleId);
            Assert.True(user.HasRole(newRoleId));
        }

        [Fact]
        public void Deactivate_ShouldDeactivateUser()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            user.Deactivate();

            // Assert
            Assert.False(user.IsActive);
        }

        [Fact]
        public void Activate_ShouldActivateUser()
        {
            // Arrange
            var user = CreateTestUser();
            user.Deactivate();

            // Act
            user.Activate();

            // Assert
            Assert.True(user.IsActive);
        }

        [Fact]
        public void GetFullName_ShouldReturnFullName()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var fullName = user.GetFullName();

            // Assert
            Assert.Equal("User Test", fullName);
        }

        [Fact]
        public void HasRole_WithValidRoleId_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateTestUser();
            user.UpdateRole(2); // Manager role ID

            // Act
            var hasRole = user.HasRole(2);

            // Assert
            Assert.True(hasRole);
        }

        [Fact]
        public void HasRole_WithInvalidRoleId_ShouldReturnFalse()
        {
            // Arrange
            var user = CreateTestUser();

            // Act
            var hasRole = user.HasRole(999); // Non-existent role ID

            // Assert
            Assert.False(hasRole);
        }

        [Fact]
        public void CanManageIncidents_WithManagerRole_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateTestUserWithRole("Incident Manager");

            // Act
            var canManage = user.CanManageIncidents();

            // Assert
            Assert.True(canManage);
        }

        [Fact]
        public void CanManageIncidents_WithUserRole_ShouldReturnFalse()
        {
            // Arrange
            var user = CreateTestUserWithRole("Clerk");

            // Act
            var canManage = user.CanManageIncidents();

            // Assert
            Assert.False(canManage);
        }

        [Fact]
        public void CanManageUsers_WithAdminRole_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateTestUserWithRole("Admin");

            // Act
            var canManage = user.CanManageUsers();

            // Assert
            Assert.True(canManage);
        }

        [Fact]
        public void CanManageUsers_WithManagerRole_ShouldReturnFalse()
        {
            // Arrange
            var user = CreateTestUserWithRole("Incident Manager");

            // Act
            var canManage = user.CanManageUsers();

            // Assert
            Assert.False(canManage);
        }

        private static User CreateTestUser()
        {
            return User.Create(
                "testuser",
                "test@example.com",
                "Test",
                "User",
                1 // Default role ID (Clerk)
            );
        }

        private static User CreateTestUserWithRole(string roleName)
        {
            var roleId = roleName switch
            {
                "Admin" => 4,
                "Incident Manager" => 2,
                "Warehouse Staff" => 3,
                "Clerk" => 1,
                _ => 1
            };

            var user = User.Create(
                "testuser",
                "test@example.com",
                "Test",
                "User",
                roleId
            );

            // Roleナビゲーションプロパティを設定（テスト用）
            var role = new Role(roleName, $"Test {roleName}");
            user.SetRole(role);

            return user;
        }
    }
}
