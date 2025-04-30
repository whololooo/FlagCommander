using FlagCommander.Persistence.Repositories.Sql;

namespace FlagCommander.Persistence.Test;

public class SqlLiteInMemoryRepositoryTests
{
    private readonly SqlLiteInMemoryRepository _repository;

    public SqlLiteInMemoryRepositoryTests()
    {
        _repository = new SqlLiteInMemoryRepository();
    }

    [Fact]
    public async Task GetAsync_ReturnsNull_WhenFeatureDoesNotExist()
    {
        // Act
        var result = await _repository.GetAsync("NonExistentFeature");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task EnableAsync_CreatesFeature_WhenFeatureDoesNotExist()
    {
        // Act
        await _repository.EnableAsync("NewFeature");
        var result = await _repository.GetAsync("NewFeature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewFeature", result!.Name);
    }

    [Fact]
    public async Task EnableAsync_EnablesFeature_WhenFeatureExists()
    {
        // Arrange
        await _repository.EnableAsync("ExistingFeature");

        // Act
        await _repository.EnableAsync("ExistingFeature");
        var result = await _repository.GetAsync("ExistingFeature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ExistingFeature", result!.Name);
    }
    
    [Fact]
    public async Task DisableAsync_DisablesFeature_WhenFeatureExists()
    {
        // Arrange
        await _repository.EnableAsync("ExistingFeature");

        // Act
        await _repository.DisableAsync("ExistingFeature");
        var result = await _repository.GetAsync("ExistingFeature");

        // Assert
        Assert.Null(result);
    }
}
