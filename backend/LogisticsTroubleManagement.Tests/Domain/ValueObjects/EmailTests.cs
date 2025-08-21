using Xunit;
using LogisticsTroubleManagement.Domain.ValueObjects;

namespace LogisticsTroubleManagement.Tests.Domain.ValueObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@domain.co.jp")]
        [InlineData("test+tag@example.org")]
        [InlineData("123@example.com")]
        public void Create_WithValidEmail_ShouldCreateEmail(string validEmail)
        {
            // Act
            var email = Email.Create(validEmail);

            // Assert
            Assert.NotNull(email);
            Assert.Equal(validEmail, email.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_WithEmptyEmail_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
            Assert.Contains("Email cannot be empty", exception.Message);
        }

        [Fact]
        public void Create_WithNullEmail_ShouldThrowArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Email.Create(null!));
            Assert.Contains("Email cannot be empty", exception.Message);
        }

        [Theory]
        [InlineData("invalid-email")]
        [InlineData("@example.com")]
        [InlineData("test@")]
        [InlineData("test.example.com")]
        [InlineData("test@.com")]
        public void Create_WithInvalidEmailFormat_ShouldThrowArgumentException(string invalidEmail)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Email.Create(invalidEmail));
            Assert.Contains("Invalid email format", exception.Message);
        }

        [Fact]
        public void Equals_WithSameEmail_ShouldReturnTrue()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("test@example.com");

            // Act & Assert
            Assert.Equal(email1, email2);
            Assert.True(email1.Equals(email2));
            Assert.True(email1 == email2);
            Assert.False(email1 != email2);
        }

        [Fact]
        public void Equals_WithDifferentEmail_ShouldReturnFalse()
        {
            // Arrange
            var email1 = Email.Create("test1@example.com");
            var email2 = Email.Create("test2@example.com");

            // Act & Assert
            Assert.NotEqual(email1, email2);
            Assert.False(email1.Equals(email2));
            Assert.False(email1 == email2);
            Assert.True(email1 != email2);
        }

        [Fact]
        public void Equals_WithNull_ShouldReturnFalse()
        {
            // Arrange
            var email = Email.Create("test@example.com");

            // Act & Assert
            Assert.False(email.Equals(null));
            Assert.False(email == null);
            Assert.True(email != null);
        }

        [Fact]
        public void GetHashCode_WithSameEmail_ShouldReturnSameHashCode()
        {
            // Arrange
            var email1 = Email.Create("test@example.com");
            var email2 = Email.Create("test@example.com");

            // Act & Assert
            Assert.Equal(email1.GetHashCode(), email2.GetHashCode());
        }

        [Fact]
        public void ToString_ShouldReturnEmailValue()
        {
            // Arrange
            var emailValue = "test@example.com";
            var email = Email.Create(emailValue);

            // Act
            var result = email.ToString();

            // Assert
            Assert.Equal(emailValue, result);
        }

        [Fact]
        public void ImplicitConversion_FromString_ShouldWork()
        {
            // Arrange
            var emailValue = "test@example.com";

            // Act
            Email email = Email.Create(emailValue);

            // Assert
            Assert.Equal(emailValue, email.Value);
        }

        [Fact]
        public void ImplicitConversion_ToString_ShouldWork()
        {
            // Arrange
            var email = Email.Create("test@example.com");

            // Act
            string emailString = email;

            // Assert
            Assert.Equal("test@example.com", emailString);
        }
    }
}
