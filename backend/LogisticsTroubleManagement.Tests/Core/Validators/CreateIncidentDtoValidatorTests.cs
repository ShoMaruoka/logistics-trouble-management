using Xunit;
using FluentValidation.TestHelper;
using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Validators;
using LogisticsTroubleManagement.Domain.Enums;

namespace LogisticsTroubleManagement.Tests.Core.Validators
{
    public class CreateIncidentDtoValidatorTests
    {
        private readonly CreateIncidentDtoValidator _validator;

        public CreateIncidentDtoValidatorTests()
        {
            _validator = new CreateIncidentDtoValidator();
        }

        [Fact]
        public void Should_Pass_When_Valid_Data()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Fail_When_Title_Is_Empty(string title)
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = title,
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Fail_When_Title_Is_Null()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = null!,
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Fail_When_Title_Is_Too_Long()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = new string('a', 201), // 200文字を超える
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Fail_When_Description_Is_Empty(string description)
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = description,
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Fail_When_Description_Is_Null()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = null!,
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Fail_When_Description_Is_Too_Long()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = new string('a', 2001), // 2000文字を超える
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Should_Fail_When_Category_Is_Empty(string category)
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = category,
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Category);
        }

        [Fact]
        public void Should_Fail_When_Category_Is_Null()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = null!,
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Category);
        }

        [Fact]
        public void Should_Fail_When_Category_Is_Too_Long()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = new string('a', 51), // 50文字を超える
                Priority = Priority.Medium,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.Category);
        }

        [Fact]
        public void Should_Fail_When_ReportedById_Is_Zero()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 0
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ReportedById);
        }

        [Fact]
        public void Should_Fail_When_ReportedById_Is_Negative()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = -1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldHaveValidationErrorFor(x => x.ReportedById);
        }

        [Fact]
        public void Should_Pass_When_All_Fields_Are_Valid()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている。顧客からの問い合わせがあり、緊急対応が必要。",
                Category = "配送・物流",
                Priority = Priority.High,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [InlineData(Priority.Low)]
        [InlineData(Priority.Medium)]
        [InlineData(Priority.High)]
        [InlineData(Priority.Critical)]
        public void Should_Pass_When_Priority_Is_Valid(Priority priority)
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = priority,
                ReportedById = 1
            };

            // Act
            var result = _validator.TestValidate(dto);

            // Assert
            result.ShouldNotHaveValidationErrorFor(x => x.Priority);
        }
    }
}
