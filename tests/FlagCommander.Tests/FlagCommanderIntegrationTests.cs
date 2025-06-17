using FlagCommander.Persistence.Repositories.Sql;

namespace FlagCommander.Tests
{
    public class FlagCommanderIntegrationTests
    {
        private readonly FlagCommander _flagManager;

        public FlagCommanderIntegrationTests()
        {
            var repository = new SqlLiteInMemoryRepository();
            _flagManager = new FlagCommander(repository);
        }

        [Fact]
        public async Task FullFeatureFlagLifecycle()
        {
            // Feature should not exist initially
            var isEnabled = await _flagManager.IsEnabledAsync("TestFeature");
            Assert.False(isEnabled);

            // Enable the feature with 100% to ensure consistent results
            await _flagManager.EnableAsync("TestFeature");
            await _flagManager.EnablePercentageOfTimeAsync("TestFeature", 100);
            
            // Check if enabled
            isEnabled = await _flagManager.IsEnabledAsync("TestFeature");
            Assert.True(isEnabled);

            // Disable the feature
            await _flagManager.DisableAsync("TestFeature");
            
            // Should be disabled now
            isEnabled = await _flagManager.IsEnabledAsync("TestFeature");
            Assert.False(isEnabled);
        }

        [Fact]
        public async Task ActorBasedFeatureFlag()
        {
            var actor1 = new TestActor { Id = "user1" };
            var actor2 = new TestActor { Id = "user2" };

            // Enable for actor1 only
            await _flagManager.EnableAsync("ActorFeature", actor1, a => a.Id);
            await _flagManager.EnablePercentageOfActorsAsync("ActorFeature", 100);
            
            // Should be enabled for actor1, disabled for actor2
            var isEnabledForActor1 = await _flagManager.IsEnabledAsync("ActorFeature", actor1, a => a.Id);
            var isEnabledForActor2 = await _flagManager.IsEnabledAsync("ActorFeature", actor2, a => a.Id);
            Assert.True(isEnabledForActor1);
            Assert.False(isEnabledForActor2);
            
            // Add actor2
            await _flagManager.EnableAsync("ActorFeature", actor2, a => a.Id);
            
            // Both should be enabled now
            isEnabledForActor2 = await _flagManager.IsEnabledAsync("ActorFeature", actor2, a => a.Id);
            Assert.True(isEnabledForActor2);
            
            // Remove actor1
            await _flagManager.DisableAsync("ActorFeature", actor1, a => a.Id);
            
            // actor1 should now be disabled, actor2 still enabled
            isEnabledForActor1 = await _flagManager.IsEnabledAsync("ActorFeature", actor1, a => a.Id);
            Assert.False(isEnabledForActor1);
            Assert.True(await _flagManager.IsEnabledAsync("ActorFeature", actor2, a => a.Id));
        }

        [Fact]
        public async Task ConditionalFeatureFlag()
        {
            var adminUser = new TestActor { Id = "admin", IsAdmin = true };
            var regularUser = new TestActor { Id = "user", IsAdmin = false };

            await _flagManager.EnableAsync("AdminFeature");
            
            // Test with admin condition
            var isEnabledForAdmin = await _flagManager.IsEnabledAsync("AdminFeature", adminUser, a => a.IsAdmin);
            var isEnabledForRegular = await _flagManager.IsEnabledAsync("AdminFeature", regularUser, a => a.IsAdmin);
            
            // Should be enabled for admin, disabled for regular user
            Assert.True(isEnabledForAdmin);
            Assert.False(isEnabledForRegular);
        }

        [Fact]
        public async Task PercentageOfTimeFeatureFlag()
        {
            // Create feature with 0% chance
            await _flagManager.EnableAsync("ZeroPercentFeature");
            await _flagManager.EnablePercentageOfTimeAsync("ZeroPercentFeature", 0);
            
            // Should always be disabled at 0%
            var isEnabled = await _flagManager.IsEnabledAsync("ZeroPercentFeature");
            Assert.False(isEnabled);
            
            // Set to 100% chance
            await _flagManager.EnablePercentageOfTimeAsync("ZeroPercentFeature", 100);
            
            // Should always be enabled at 100%
            isEnabled = await _flagManager.IsEnabledAsync("ZeroPercentFeature");
            Assert.True(isEnabled);
        }

        [Fact]
        public async Task PercentageOfActorsFeatureFlag()
        {
            var actor = new TestActor { Id = "user1" };
            
            // Create actor-percentage based flag
            await _flagManager.EnableAsync("ActorPercentFeature");
            await _flagManager.EnableAsync("ActorPercentFeature", actor, a => a.Id);
            
            // Set to 100%
            await _flagManager.EnablePercentageOfActorsAsync("ActorPercentFeature", 100);
            var isEnabled = await _flagManager.IsEnabledAsync("ActorPercentFeature", actor, a => a.Id);
            Assert.True(isEnabled);
            
            // Set to 0%
            await _flagManager.EnablePercentageOfActorsAsync("ActorPercentFeature", 0);
            isEnabled = await _flagManager.IsEnabledAsync("ActorPercentFeature", actor, a => a.Id);
            Assert.False(isEnabled);
        }

        [Fact]
        public async Task MultipleFeatureFlagsCoexist()
        {
            // Enable two separate features with different settings
            await _flagManager.EnableAsync("Feature1");
            await _flagManager.EnableAsync("Feature2");
            await _flagManager.EnablePercentageOfTimeAsync("Feature1", 100);
            await _flagManager.EnablePercentageOfTimeAsync("Feature2", 0);
            
            // Feature1 should be enabled, Feature2 disabled
            Assert.True(await _flagManager.IsEnabledAsync("Feature1"));
            Assert.False(await _flagManager.IsEnabledAsync("Feature2"));
        }

        [Fact]
        public async Task PercentageOfTime75Percent()
        {
            await _flagManager.EnableAsync("Feature1");
            await _flagManager.EnablePercentageOfTimeAsync("Feature1", 75);

            var results = new List<bool>();
            for (var i = 0; i < 1000; i++)
            {
                results.Add(await _flagManager.IsEnabledAsync("Feature1"));
            }
            
            var enabledCount = results.Count(r => r);
            var percentage = (double)enabledCount / results.Count * 100;
            Assert.InRange(percentage, 70, 80); // Allowing some margin for randomness
        }
        
        [Fact]
        public async Task PercentageOfTime22Percent()
        {
            await _flagManager.EnableAsync("Feature1");
            await _flagManager.EnablePercentageOfTimeAsync("Feature1", 22);

            var results = new List<bool>();
            for (var i = 0; i < 1000; i++)
            {
                results.Add(await _flagManager.IsEnabledAsync("Feature1"));
            }
            
            var enabledCount = results.Count(r => r);
            var percentage = (double)enabledCount / results.Count * 100;
            Assert.InRange(percentage, 17, 27); // Allowing some margin for randomness
        }
        
        [Fact]
        public async Task PercentageOfActors75Percent()
        {
            await _flagManager.EnableAsync("Feature1");
            await _flagManager.EnablePercentageOfActorsAsync("Feature1", 75);

            
            for (var i = 0; i < 1000; i++)
            {
                var actor = new TestActor { Id = $"user{i}" };
                await _flagManager.EnableAsync("Feature1", actor, a => a.Id);
            }
            
            var results = new List<bool>();
            for (var i = 0; i < 1000; i++)
            {
                var actor = new TestActor { Id = $"user{i}" };
                results.Add(await _flagManager.IsEnabledAsync("Feature1", actor, a => a.Id));
            }
            
            var enabledCount = results.Count(r => r);
            var percentage = (double)enabledCount / results.Count * 100;
            Assert.InRange(percentage, 70, 80); // Allowing some margin for randomness
        }
        
        [Fact]
        public async Task PercentageOfActors31Percent()
        {
            await _flagManager.EnableAsync("Feature1");
            await _flagManager.EnablePercentageOfActorsAsync("Feature1", 31);

            
            for (var i = 0; i < 1000; i++)
            {
                var actor = new TestActor { Id = $"user{i}" };
                await _flagManager.EnableAsync("Feature1", actor, a => a.Id);
            }
            
            var results = new List<bool>();
            for (var i = 0; i < 1000; i++)
            {
                var actor = new TestActor { Id = $"user{i}" };
                results.Add(await _flagManager.IsEnabledAsync("Feature1", actor, a => a.Id));
            }
            
            var enabledCount = results.Count(r => r);
            var percentage = (double)enabledCount / results.Count * 100;
            Assert.InRange(percentage, 26, 36); // Allowing some margin for randomness
        }

        // Helper class for testing
        private class TestActor
        {
            public string Id { get; init; } = string.Empty;
            public bool IsAdmin { get; init; }
        }
    }
}