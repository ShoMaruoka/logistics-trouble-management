using LogisticsTroubleManagement.Core.DTOs;
using System.Text.Json;

namespace LogisticsTroubleManagement.Tests.Core;

public class PagedResultDtoTests
{
    [Fact]
    public void PagedResultDto_Constructor_ShouldSetAllPropertiesCorrectly()
    {
        // Arrange
        var items = new List<string> { "item1", "item2", "item3" };
        var totalCount = 23;
        var page = 2;
        var pageSize = 5;

        // Act
        var result = new PagedResultDto<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.Equal(items, result.Items);
        Assert.Equal(totalCount, result.TotalCount);
        Assert.Equal(page, result.Page);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(5, result.TotalPages); // Math.Ceiling(23/5) = 5
        Assert.True(result.HasPreviousPage); // page > 1
        Assert.True(result.HasNextPage); // page < totalPages
    }

    [Fact]
    public void PagedResultDto_Serialization_ShouldUsePageProperty()
    {
        // Arrange
        var items = new List<string> { "test" };
        var result = new PagedResultDto<string>(items, 10, 2, 5);

        // Act
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        // Assert
        Assert.Contains("\"page\":", json);
        Assert.DoesNotContain("\"pageNumber\":", json);
        Assert.Contains("\"totalCount\":", json);
        Assert.Contains("\"pageSize\":", json);
        Assert.Contains("\"totalPages\":", json);
        Assert.Contains("\"hasPreviousPage\":", json);
        Assert.Contains("\"hasNextPage\":", json);
    }

    [Fact]
    public void PagedResultDto_WithFirstPage_ShouldSetCorrectNavigationFlags()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var page = 1;

        // Act
        var result = new PagedResultDto<string>(items, 10, page, 5);

        // Assert
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
    }

    [Fact]
    public void PagedResultDto_WithLastPage_ShouldSetCorrectNavigationFlags()
    {
        // Arrange
        var items = new List<string> { "item1" };
        var totalCount = 10;
        var pageSize = 5;
        var lastPage = 2; // Math.Ceiling(10/5) = 2

        // Act
        var result = new PagedResultDto<string>(items, totalCount, lastPage, pageSize);

        // Assert
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
    }

    [Fact]
    public void PagedResultDto_WithSinglePage_ShouldSetCorrectNavigationFlags()
    {
        // Arrange
        var items = new List<string> { "item1", "item2" };
        var totalCount = 2;
        var pageSize = 5;
        var page = 1;

        // Act
        var result = new PagedResultDto<string>(items, totalCount, page, pageSize);

        // Assert
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
        Assert.Equal(1, result.TotalPages);
    }
}
