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
            var role = UserRole.User;

            // Act
            var user = User.Create(username, email, firstName, lastName, role);

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username, user.Username);
            Assert.Equal(email, user.Email.Value);
            Assert.Equal(firstName, user.FirstName);
            Assert.Equal(lastName, user.LastName);
            Assert.Equal(role, user.Role);
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
        public void UpdateRole_WithValidRole_ShouldUpdateRole()
        {
            // Arrange
            var user = CreateTestUser();
            var newRole = UserRole.Manager;

            // Act
            user.UpdateRole(newRole);

            // Assert
            Assert.Equal(newRole, user.Role);
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
        public void HasPermission_WithLowerRole_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateTestUser(UserRole.Manager);

            // Act
            var hasPermission = user.HasPermission(UserRole.User);

            // Assert
            Assert.True(hasPermission);
        }

        [Fact]
        public void HasPermission_WithHigherRole_ShouldReturnFalse()
        {
            // Arrange
            var user = CreateTestUser(UserRole.User);

            // Act
            var hasPermission = user.HasPermission(UserRole.Manager);

            // Assert
            Assert.False(hasPermission);
        }

        [Fact]
        public void CanManageIncidents_WithManagerRole_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateTestUser(UserRole.Manager);

            // Act
            var canManage = user.CanManageIncidents();

            // Assert
            Assert.True(canManage);
        }

        [Fact]
        public void CanManageIncidents_WithUserRole_ShouldReturnFalse()
        {
            // Arrange
            var user = CreateTestUser(UserRole.User);

            // Act
            var canManage = user.CanManageIncidents();

            // Assert
            Assert.False(canManage);
        }

        [Fact]
        public void CanManageUsers_WithAdminRole_ShouldReturnTrue()
        {
            // Arrange
            var user = CreateTestUser(UserRole.Admin);

            // Act
            var canManage = user.CanManageUsers();

            // Assert
            Assert.True(canManage);
        }

        [Fact]
        public void CanManageUsers_WithManagerRole_ShouldReturnFalse()
        {
            // Arrange
            var user = CreateTestUser(UserRole.Manager);

            // Act
            var canManage = user.CanManageUsers();

            // Assert
            Assert.False(canManage);
        }

        private static User CreateTestUser(UserRole role = UserRole.User)
        {
            return User.Create(
                "testuser",
                "test@example.com",
                "Test",
                "User",
                role
            );
        }
    }
}
