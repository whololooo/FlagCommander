using FlagCommander.Persistence.Repositories.Sql;

namespace FlagCommander.Persistence.Test;


public abstract class SqlRepositoryBaseTests
{
    protected SqlRepositoryBase? Repository;
    
    [Fact]
    public async Task GetAsync_ReturnsNull_WhenFeatureDoesNotExist()
    {
        // Act
        var result = await Repository!.GetAsync("NonExistentFeature");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task GetAsync_ReturnsNull_WhenFeatureExist()
    {
        // Arrange
        await Repository!.EnableAsync("NewFeature");
        
        // Act
        var result = await Repository!.GetAsync("NewFeature");

        // Assert
        Assert.NotNull(result);
    }

    [Fact]
    public async Task SetPercentageOfTime_ReturnsFeatureWithNewPercentageValue_WhenFeatureExists()
    {
        // Arrange
        await Repository!.EnableAsync("NewFeature");
        var feature = await Repository.GetAsync("NewFeature");
        var currentPercentage = feature!.PercentageOfTime;
        
        // Act
        await Repository.SetPercentageOfTimeAsync("NewFeature", 50);
        
        // Assert
        var updatedFeature = await Repository.GetAsync("NewFeature");
        Assert.NotNull(updatedFeature);
        Assert.NotEqual(currentPercentage, updatedFeature!.PercentageOfTime);
        Assert.Equal(100, currentPercentage);
        Assert.Equal(50, updatedFeature.PercentageOfTime);
    }
    
    [Fact]
    public async Task SetPercentageOfTime_ReturnsFeatureWithNewPercentageValue_WhenFeatureDoesNotExists()
    {
        // Act
        await Repository!.SetPercentageOfTimeAsync("NonExistingFeature", 50);
        
        // Assert
        var updatedFeature = await Repository.GetAsync("NonExistingFeature");
        Assert.Null(updatedFeature);
    }
    
    [Fact]
    public async Task SetPercentageOfActors_ReturnsFeatureWithNewPercentageValue_WhenFeatureExists()
    {
        // Arrange
        await Repository!.EnableAsync("NewFeature");
        var feature = await Repository.GetAsync("NewFeature");
        var currentPercentage = feature!.PercentageOfTime;
        
        // Act
        await Repository.SetPercentageOfActorsAsync("NewFeature", 50);
        
        // Assert
        var updatedFeature = await Repository.GetAsync("NewFeature");
        Assert.NotNull(updatedFeature);
        Assert.NotEqual(currentPercentage, updatedFeature!.PercentageOfActors);
        Assert.Equal(100, currentPercentage);
        Assert.Equal(50, updatedFeature.PercentageOfActors);
    }
    
    [Fact]
    public async Task SetPercentageOfActors_ReturnsFeatureWithNewPercentageValue_WhenFeatureDoesNotExists()
    {
        // Act
        await Repository!.SetPercentageOfActorsAsync("NonExistingFeature", 50);
        
        // Assert
        var updatedFeature = await Repository.GetAsync("NonExistingFeature");
        Assert.Null(updatedFeature);
    }

    [Fact]
    public async Task EnableAsync_CreatesFeature_WhenFeatureDoesNotExist()
    {
        // Act
        await Repository!.EnableAsync("NewFeature");
        var result = await Repository.GetAsync("NewFeature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("NewFeature", result!.Name);
    }

    [Fact]
    public async Task EnableAsync_EnablesFeature_WhenFeatureExists()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");

        // Act
        await Repository.EnableAsync("ExistingFeature");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("ExistingFeature", result!.Name);
    }
    
    [Fact]
    public async Task DisableAsync_DisablesFeature_WhenFeatureExists()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");

        // Act
        await Repository.DisableAsync("ExistingFeature");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DisableAsync_DisablesFeature_WhenFeatureDoesNotExists()
    {
        // Act
        await Repository!.DisableAsync("NonExistingFeature");
        var result = await Repository.GetAsync("NonExistingFeature");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteAsync_DeletesFeature_WhenFeatureExists()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");

        // Act
        await Repository.DeleteAsync("ExistingFeature");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task DeleteAsync_DoesNothing_WhenFeatureDoesNotExists()
    {
        // Act
        await Repository!.DeleteAsync("NonExistingFeature");
        var result = await Repository.GetAsync("NonExistingFeature");

        // Assert
        Assert.Null(result);
    }
    
    [Fact]
    public async Task AddActor_AddsNewActor_WhenFeatureExistsAndActorDoesNot()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");

        // Act
        await Repository.AddActorAsync("ExistingFeature", "NewActor");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("NewActor", result!.ActorIds);
    }
    
    [Fact]
    public async Task AddActor_AddsNewActor_WhenFeatureAndActorExist()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");
        await Repository.AddActorAsync("ExistingFeature", "NewActor");

        // Act
        await Repository.AddActorAsync("ExistingFeature", "NewActor");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.NotNull(result);
        Assert.Contains("NewActor", result!.ActorIds);
        Assert.Single(result!.ActorIds.Where(actorId => actorId == "NewActor"));
    }
    
    [Fact]
    public async Task DeleteActor_DeletesActor_WhenFeatureAndActorExist()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");
        await Repository.AddActorAsync("ExistingFeature", "ExistingActor");

        // Act
        await Repository.DeleteActorAsync("ExistingFeature", "ExistingActor");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("ExistingActor", result!.ActorIds);
    }
    
    [Fact]
    public async Task DeleteActor_DoesNothing_WhenFeatureExistsAndActorDoesNotExist()
    {
        // Arrange
        await Repository!.EnableAsync("ExistingFeature");

        // Act
        await Repository.DeleteActorAsync("ExistingFeature", "NonExistingActor");
        var result = await Repository.GetAsync("ExistingFeature");

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain("NonExistingActor", result!.ActorIds);
    }
    
    [Fact]
    public async Task DeleteActor_DoesNothing_WhenFeatureAndActorDoNotExist()
    {
        // Act
        await Repository!.DeleteActorAsync("NonExistingFeature", "NonExistingActor");
        var result = await Repository.GetAsync("NonExistingFeature");

        // Assert
        Assert.Null(result);
    }
}
