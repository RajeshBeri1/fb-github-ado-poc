using System.Linq.Expressions;
using System.Security.Authentication;
using System.Security.Claims;
using FluentAssertions;
using Lamar;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Tests.WebAPI.Common;
using ZiggyCreatures.Caching.Fusion;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// UserProviderTests
    /// </summary>
    public class UserProviderTests : TestBase<IUserProvider>
    {
        private const string ClaimIdNotSet = $"Identity is not a claims identity";

        /// <summary>
        /// Gets the current asynchronous should return user when user exists.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = FakeData.User.Generate();
            var userDto = mapper.Map<UserDetailsDTO>(user);

            portalDbContext.Set<User>().Add(user);
            await portalDbContext.SaveChangesAsync(CancellationToken);

            /*
            var cache = container.GetInstance<ICacheLogic>();
            */

            var portal = container.GetInstance<IPortalUnitOfWork>();
            var repo = container.GetInstance<INamedModelBaseRepository<User>>();

            portal.Users.Returns(repo);
            repo.GetByNameAsync<UserDetailsDTO>(user.Name, CancellationToken).Returns(userDto);

            /*
            Func<CancellationToken, Task<UserDetailsDTO>> privateAddOrGetAsync = default!;

            cache.GetAsync(user.Name, Arg.Do<Func<CancellationToken, Task<UserDetailsDTO>>>(x => privateAddOrGetAsync = x), CancellationToken)
                .Returns(userDto);
            */

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                /*
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.EMail),
                */
            }));

            // Act
            var actual = await sut.GetCurrentAsync(claimsPrincipal, CancellationToken);

            /*
            var actual = await privateAddOrGetAsync(CancellationToken);
            */

            // Assert
            await repo.DidNotReceiveWithAnyArgs().AddAsync(user, CancellationToken);
            await portal.DidNotReceiveWithAnyArgs().SaveChangesAsync(CancellationToken);
            await repo.Received(1).GetByNameAsync<UserDetailsDTO>(user.Name, CancellationToken);

            actual.Should().BeEquivalentTo(userDto);
        }

        /// <summary>
        /// Gets the current asynchronous should save user when user not exists.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldSaveUser_WhenUserNotExists()
        {
            // Arrange
            var user = FakeData.User.Generate();
            var userDto = mapper.Map<UserDetailsDTO>(user);

            /*
            var cache = container.GetInstance<ICacheLogic>();
            */

            var portal = container.GetInstance<IPortalUnitOfWork>();
            var repo = container.GetInstance<INamedModelBaseRepository<User>>();

            portal.Users.Returns(repo);
            repo.GetByNameAsync<UserDetailsDTO>(user.Name, CancellationToken).Throws(new KeyNotFoundException());
            repo.AddOrUpdateAsync(Arg.Is<User>(x => x.Name == user.Name), Arg.Any<Expression<Func<User, object>>>(), CancellationToken).Returns(user);

            /*
            Func<CancellationToken, Task<UserDetailsDTO>> privateAddOrGetAsync = default!;

            cache.GetAsync(user.Name, Arg.Do<Func<CancellationToken, Task<UserDetailsDTO>>>(x => privateAddOrGetAsync = x), CancellationToken)
                .Returns(userDto);
            */

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                /*
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, user.EMail),
                */
            }));

            // Act
            var actual = await sut.GetCurrentAsync(claimsPrincipal, CancellationToken);

            /*
            var actual = await privateAddOrGetAsync(CancellationToken);
            */

            // Assert
            await repo.Received(1).AddOrUpdateAsync(Arg.Is<User>(x => x.Name == user.Name), Arg.Any<Expression<Func<User, object>>>(), CancellationToken);

            /*
            await portal.Received(1).SaveChangesAsync(CancellationToken);
            */

            await repo.Received(1).GetByNameAsync<UserDetailsDTO>(user.Name, CancellationToken);

            actual.Should().BeEquivalentTo(userDto);
        }

        /*
        /// <summary>
        /// Gets the current asynchronous should throw exception when claim email is not
        /// set.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldThrowException_WhenClaimEmailIsNotSet()
        {
            // Arrange
            var user = FakeData.User.Generate();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Name, user.DisplayName),
                new Claim(ClaimTypes.Email, string.Empty),
            }));

            // Act
            var actual = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetCurrentAsync(claimsPrincipal, CancellationToken));

            // Assert
            actual.Message.Should().Be($"Claim {nameof(ClaimTypes.Email)} is null or empty.");
        }*/

        /// <summary>
        /// Gets the current asynchronous should throw exception when claim name
        /// identifier is not set.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldThrowException_WhenClaimNameIdentifierIsNotSet()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, string.Empty),
            }));

            // Act
            var actual = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetCurrentAsync(claimsPrincipal, CancellationToken));

            // Assert
            actual.Message.Should().Be($"Claim {nameof(ClaimTypes.NameIdentifier)} is null or empty.");
        }

        /*
        /// <summary>
        /// Gets the current asynchronous should throw exception when claim name is not
        /// set.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldThrowException_WhenClaimNameIsNotSet()
        {
            // Arrange
            var user = FakeData.User.Generate();

            var claimsPrincipal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Name),
                new Claim(ClaimTypes.Name, string.Empty),
            }));

            // Act
            var actual = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetCurrentAsync(claimsPrincipal, CancellationToken));

            // Assert
            actual.Message.Should().Be($"Claim {nameof(ClaimTypes.Name)} is null or empty.");
        }*/

        /// <summary>
        /// Gets the current asynchronous should throw exception when claims identity is
        /// not set.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldThrowException_WhenClaimsIdentityIsNotSet()
        {
            // Arrange
            var claimsPrincipal = new ClaimsPrincipal();

            // Act
            var actual = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetCurrentAsync(claimsPrincipal, CancellationToken));

            // Assert
            actual.Message.Should().Be(ClaimIdNotSet);
        }

        /// <summary>
        /// Gets the current asynchronous should return user when claims principal set.
        /// </summary>
        [Fact]
        public async Task GetCurrentAsync_ShouldThrowException_WhenClaimsPrincipalIsNotSet()
        {
            // Arrange
            ClaimsPrincipal claimsPrincipal = default!;

            // Act
            var actual = await Assert.ThrowsAsync<AuthenticationException>(() => sut.GetCurrentAsync(claimsPrincipal, CancellationToken));

            // Assert
            actual.Message.Should().Be(ClaimIdNotSet);
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IPortalUnitOfWork>().Use(Substitute.For<IPortalUnitOfWork>()).Singleton();
            registry.For<ICacheLogic>().Use(Substitute.For<ICacheLogic>()).Singleton();
            registry.For<INamedModelBaseRepository<User>>().Use(Substitute.For<INamedModelBaseRepository<User>>()).Singleton();
            registry.For<IFusionCache>().Use(Substitute.For<IFusionCache>()).Singleton();
            registry.For<FusionCacheConfig>().Use(new FusionCacheConfig { Enabled = false }).Singleton();

            registry.For<OmniAuthConfig>().Use(new OmniAuthConfig { Enabled = false }).Singleton();
            registry.For<OktaConfig>().Use(new OktaConfig { Enabled = true }).Singleton();
            registry.For<IUserProvider>().Use<UserProvider>();
        }
    }
}