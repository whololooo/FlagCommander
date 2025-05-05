using FlagCommander.Persistence.Repositories.Sql;
using Testcontainers.PostgreSql;

namespace FlagCommander.Persistence.Test;

public class PostgresRepositoryTests
{
    private readonly PostgresRepository _repository;
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder()
        .WithImage("postgres:latest")
        .WithDatabase("testdb")
        .WithUsername("postgres")
        .WithPassword("postgres")
        .Build();
    
    private string _connectionString = "Host=localhost;Port=5432;Username=postgres;Password=postgres;Database=testdb";

    public PostgresRepositoryTests()
    {
        _repository = new PostgresRepository(_connectionString);
    }
    
    public Task InitializeAsync()
    {
        return _postgres.StartAsync();
    }

    public Task DisposeAsync()
    {
        return _postgres.DisposeAsync().AsTask();
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
