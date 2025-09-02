using Xunit;
using FluentValidation.TestHelper;
using LogisticsTroubleManagement.Core.DTOs;
using LogisticsTroubleManagement.Core.Validators;
using LogisticsTroubleManagement.Domain.Enums;
using System.ComponentModel.DataAnnotations;

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
                ReportedById = 1,
                TroubleType = 1,
                DamageType = 1,
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now
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
                ReportedById = 1,
                TroubleType = 1,
                DamageType = 1,
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now
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

        [Fact]
        public void Should_Pass_When_Enum_Values_Are_Valid()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1,
                TroubleType = 1, // 有効なEnum値
                DamageType = 1,  // 有効なEnum値
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }

        [Fact]
        public void Should_Fail_When_TroubleType_Is_Invalid()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1,
                TroubleType = 999, // 無効なEnum値
                DamageType = 1,
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("TroubleType"));
            Assert.Contains(validationResults, vr => vr.ErrorMessage.Contains("トラブル種類の値 '999' は無効です。"));
        }

        [Fact]
        public void Should_Fail_When_DamageType_Is_Invalid()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1,
                TroubleType = 1,
                DamageType = 999, // 無効なEnum値
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("DamageType"));
            Assert.Contains(validationResults, vr => vr.ErrorMessage.Contains("損傷種類の値 '999' は無効です。"));
        }

        [Fact]
        public void Should_Fail_When_DefectiveItems_Exceeds_TotalShipments()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1,
                TroubleType = 1,
                DamageType = 1,
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                TotalShipments = 10,
                DefectiveItems = 15, // TotalShipmentsを超える
                OccurrenceDate = DateTime.Now
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("DefectiveItems"));
            Assert.Contains(validationResults, vr => vr.ErrorMessage.Contains("不良品数は出荷総数以下である必要があります。"));
        }

        [Fact]
        public void Should_Fail_When_EffectivenessStatus_Implemented_Without_EffectivenessDate()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1,
                TroubleType = 1,
                DamageType = 1,
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now,
                EffectivenessStatus = EffectivenessStatus.Implemented, // 実施済み
                EffectivenessDate = null // 日付なし
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.False(isValid);
            Assert.Contains(validationResults, vr => vr.MemberNames.Contains("EffectivenessDate"));
            Assert.Contains(validationResults, vr => vr.ErrorMessage.Contains("有効性評価が実施済みの場合、有効性確認日は必須です。"));
        }

        [Fact]
        public void Should_Pass_When_EffectivenessStatus_Implemented_With_EffectivenessDate()
        {
            // Arrange
            var dto = new CreateIncidentDto
            {
                Title = "配送遅延トラブル",
                Description = "商品の配送が予定より2日遅れている",
                Category = "配送",
                Priority = Priority.Medium,
                ReportedById = 1,
                TroubleType = 1,
                DamageType = 1,
                Warehouse = 1,
                ShippingCompany = 1,
                IncidentDetails = "配送トラブルの詳細",
                OccurrenceDate = DateTime.Now,
                EffectivenessStatus = EffectivenessStatus.Implemented, // 実施済み
                EffectivenessDate = DateTime.Now // 日付あり
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(dto, new ValidationContext(dto), validationResults, true);

            // Assert
            Assert.True(isValid);
            Assert.Empty(validationResults);
        }
    }
}
