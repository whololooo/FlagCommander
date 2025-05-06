using Moq;
using FlagCommander.Persistence;
using FlagCommander.Persistence.Models;

namespace FlagCommander.Tests
{
    public class FlagCommanderTests
    {
        private readonly Mock<IRepository> _mockRepository;
        private readonly IFlagCommander flagCommander;

        public FlagCommanderTests()
        {
            _mockRepository = new Mock<IRepository>();
            flagCommander = new FlagCommander(_mockRepository.Object);
        }

        [Fact]
        public async Task IsEnabledAsync_ReturnsFalse_WhenFeatureDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAsync("NonExistingFeature"))
                .ReturnsAsync((Flag?)null);

            // Act
            var result = await flagCommander.IsEnabledAsync("NonExistingFeature");

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsEnabledAsync_ReturnsBasedOnPercentage_WhenFeatureExists()
        {
            // Arrange
            var flag = new Flag();
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(flag);

            // Act
            var result = await flagCommander.IsEnabledAsync("ExistingFeature");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEnabledAsync_WithActorId_ReturnsFalse_WhenFeatureDoesNotExist()
        {
            // Arrange
            var actor = new TestActor { Id = "actor1" };
            _mockRepository.Setup(r => r.GetAsync("NonExistingFeature"))
                .ReturnsAsync((Flag?)null);

            // Act
            var result = await flagCommander.IsEnabledAsync("NonExistingFeature", actor, a => a.Id);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsEnabledAsync_WithActorId_ReturnsTrue_WhenActorIsInList()
        {
            // Arrange
            var actor = new TestActor { Id = "actor1" };
            var flag = new Flag 
            { 
                ActorIds = ["actor1"]
            };
            
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(flag);

            // Act
            var result = await flagCommander.IsEnabledAsync("ExistingFeature", actor, a => a.Id);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsEnabledAsync_WithCondition_ReturnsFalse_WhenFeatureDoesNotExist()
        {
            // Arrange
            var actor = new TestActor { IsAdmin = true };
            _mockRepository.Setup(r => r.GetAsync("NonExistingFeature"))
                .ReturnsAsync((Flag?)null);

            // Act
            var result = await flagCommander.IsEnabledAsync("NonExistingFeature", actor, a => a.IsAdmin);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsEnabledAsync_WithCondition_ReturnsTrue_WhenConditionIsMet()
        {
            // Arrange
            var actor = new TestActor { IsAdmin = true };
            var flag = new Flag();
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(flag);

            // Act
            var result = await flagCommander.IsEnabledAsync("ExistingFeature", actor, a => a.IsAdmin);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task EnableAsync_CallsRepository_WhenFeatureDoesNotExist()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAsync("NewFeature"))
                .ReturnsAsync((Flag?)null);

            // Act
            await flagCommander.EnableAsync("NewFeature");

            // Assert
            _mockRepository.Verify(r => r.EnableAsync("NewFeature"), Times.Once);
        }

        [Fact]
        public async Task EnableAsync_DoesNotCallRepository_WhenFeatureExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(new Flag());

            // Act
            await flagCommander.EnableAsync("ExistingFeature");

            // Assert
            _mockRepository.Verify(r => r.EnableAsync(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task EnableAsync_WithActor_AddsActorToFeature()
        {
            // Arrange
            var actor = new TestActor { Id = "actor1" };
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(new Flag());

            // Act
            await flagCommander.EnableAsync("ExistingFeature", actor, a => a.Id);

            // Assert
            _mockRepository.Verify(r => r.AddActorAsync("ExistingFeature", "actor1"), Times.Once);
        }

        [Fact]
        public async Task EnableAsync_WithActor_CreatesFeatureIfNotExists()
        {
            // Arrange
            var actor = new TestActor { Id = "actor1" };
            _mockRepository.Setup(r => r.GetAsync("NewFeature"))
                .ReturnsAsync((Flag?)null);

            // Act
            await flagCommander.EnableAsync("NewFeature", actor, a => a.Id);

            // Assert
            _mockRepository.Verify(r => r.EnableAsync("NewFeature"), Times.Once);
            _mockRepository.Verify(r => r.AddActorAsync("NewFeature", "actor1"), Times.Once);
        }

        [Fact]
        public async Task EnableAsyncPercentageOfTime_ThrowsException_WhenPercentageIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                flagCommander.EnableAsyncPercentageOfTime("Feature", 101));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                flagCommander.EnableAsyncPercentageOfTime("Feature", -1));
        }

        [Fact]
        public async Task EnableAsyncPercentageOfTime_SetsPercentage_WhenFeatureExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(new Flag());

            // Act
            await flagCommander.EnableAsyncPercentageOfTime("ExistingFeature", 50);

            // Assert
            _mockRepository.Verify(r => r.SetPercentageOfTimeAsync("ExistingFeature", 50), Times.Once);
        }

        [Fact]
        public async Task EnableAsyncPercentageOfActors_ThrowsException_WhenPercentageIsInvalid()
        {
            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                flagCommander.EnableAsyncPercentageOfActors("Feature", 101));
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => 
                flagCommander.EnableAsyncPercentageOfActors("Feature", -1));
        }

        [Fact]
        public async Task EnableAsyncPercentageOfActors_SetsPercentage_WhenFeatureExists()
        {
            // Arrange
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(new Flag());

            // Act
            await flagCommander.EnableAsyncPercentageOfActors("ExistingFeature", 50);

            // Assert
            _mockRepository.Verify(r => r.SetPercentageOfActorsAsync("ExistingFeature", 50), Times.Once);
        }

        [Fact]
        public async Task DisableAsync_CallsRepository()
        {
            // Act
            await flagCommander.DisableAsync("Feature");

            // Assert
            _mockRepository.Verify(r => r.DisableAsync("Feature"), Times.Once);
        }

        [Fact]
        public async Task DisableAsync_WithActor_DoesNothing_WhenFeatureDoesNotExist()
        {
            // Arrange
            var actor = new TestActor { Id = "actor1" };
            _mockRepository.Setup(r => r.GetAsync("NonExistingFeature"))
                .ReturnsAsync((Flag?)null);

            // Act
            await flagCommander.DisableAsync("NonExistingFeature", actor, a => a.Id);

            // Assert
            _mockRepository.Verify(r => r.DeleteActorAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task DisableAsync_WithActor_DeletesActor_WhenFeatureExists()
        {
            // Arrange
            var actor = new TestActor { Id = "actor1" };
            _mockRepository.Setup(r => r.GetAsync("ExistingFeature"))
                .ReturnsAsync(new Flag());

            // Act
            await flagCommander.DisableAsync("ExistingFeature", actor, a => a.Id);

            // Assert
            _mockRepository.Verify(r => r.DeleteActorAsync("ExistingFeature", "actor1"), Times.Once);
        }

        // Helper class for testing
        private class TestActor
        {
            public string Id { get; init; } = string.Empty;
            public bool IsAdmin { get; init; }
        }
    }
}