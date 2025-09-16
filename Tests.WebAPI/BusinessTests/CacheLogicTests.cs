using FluentAssertions;
using Lamar;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.Models;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using Tests.WebAPI.Common;
using ZiggyCreatures.Caching.Fusion;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// CacheLogicTests
    /// </summary>
    public class CacheLogicTests : TestBase<ICacheLogic>
    {
        private readonly FusionCacheConfig config = new();

        /// <summary>
        /// Gets the asynchronous should return when cache is disabled.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnUser_WhenCacheIsDisabled()
        {
            // Arrange
            config.Enabled = false;
            var user = FakeData.User.Generate();

            var keyName = $"{typeof(User).Name}_{user.Id}";

            var cache = container.GetInstance<IFusionCache>();

            // Act
            var actual = await sut.GetAsync(user.Id.ToString(), x => Task.FromResult(user), CancellationToken);

            // Assert
            await cache.DidNotReceiveWithAnyArgs().GetOrSetAsync<User>(
                keyName,
                Arg.Any<Func<FusionCacheFactoryExecutionContext<User>, CancellationToken, Task<User?>>>(),
                token: CancellationToken);

            actual.Should().BeEquivalentTo(user);
        }

        /// <summary>
        /// Gets the asynchronous should return when cache is enabled.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnUser_WhenCacheIsEnabled()
        {
            // Arrange
            config.Enabled = true;
            var user = FakeData.User.Generate();

            var keyName = $"{typeof(User).Name}_{user.Id}";

            var cache = container.GetInstance<IFusionCache>();

            cache.GetOrSetAsync<User>(
                keyName,
                Arg.Any<Func<FusionCacheFactoryExecutionContext<User>, CancellationToken, Task<User?>>>(),
                token: CancellationToken).Returns(user);

            // Act
            var actual = await sut.GetAsync(user.Id.ToString(), x => Task.FromResult(user), CancellationToken);

            // Assert
            await cache.Received(1).GetOrSetAsync<User>(
                keyName,
                Arg.Any<Func<FusionCacheFactoryExecutionContext<User>, CancellationToken, Task<User?>>>(),
                token: CancellationToken);

            actual.Should().BeEquivalentTo(user);
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            var cache = Substitute.For<IFusionCache>();

            registry.For<IFusionCache>().Use(cache).Singleton();
            registry.For<ICacheLogic>().Use<CacheLogic>();
            registry.For<FusionCacheConfig>().Use(config).Singleton();
        }
    }
}