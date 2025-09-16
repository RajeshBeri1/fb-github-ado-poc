using FluentAssertions;
using Lamar;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Tests.WebAPI.Common;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// ModelBaseRepositoryTests
    /// </summary>
    public class ModelBaseRepositoryTests : TestBase<IModelBaseRepository<User>>
    {
        /// <summary>
        /// Adds the asynchronous test.
        /// </summary>
        [Fact]
        public async Task AddAsync_ShouldSucceed_WhenUserAdded()
        {
            // Arrange
            var user = FakeData.User.Generate();

            // Act
            var actual = await sut.AddAsync(user, CancellationToken);

            // Assert
            actual.Should().BeEquivalentTo(user);
        }

        /// <summary>
        /// Gets the by identifier asynchronous should return user when user exists.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldReturnUser_WhenUserExists()
        {
            // Arrange
            var user = FakeData.User.Generate();

            portalDbContext.Set<User>().Add(user);
            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await sut.GetByIdAsync(user.Id, CancellationToken);

            // Assert
            actual.Should().BeEquivalentTo(user);
        }

        /// <summary>
        /// Gets the by identifier asynchronous should throw exception when user not
        /// exists.
        /// </summary>
        [Fact]
        public async Task GetByIdAsync_ShouldThrowException_WhenUserNotExists()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var actual = await Assert.ThrowsAsync<KeyNotFoundException>(() => sut.GetByIdAsync(guid, CancellationToken));

            // Assert
            actual.Message.Should().Be($"{typeof(User).Name} {guid} not found.");
        }

        /// <summary>
        /// Gets the by identifier asynchronous dto test.
        /// </summary>
        [Fact]
        public async Task GetByIdAsyncDto_ShouldReturnDTO_WhenUserExists()
        {
            // Arrange
            var user = FakeData.User.Generate();

            portalDbContext.Set<User>().Add(user);
            await portalDbContext.SaveChangesAsync(CancellationToken);

            var dtoUser = mapper.Map<UserDetailsDTO>(user);

            // Act
            var actual = await sut.GetByIdAsync<UserDetailsDTO>(user.Id, CancellationToken);

            // Assert
            actual.Should().BeEquivalentTo(dtoUser);
        }

        /// <summary>
        /// Gets the by identifier asynchronous dto should throw exception when user not
        /// exists.
        /// </summary>
        [Fact]
        public async Task GetByIdAsyncDto_ShouldThrowException_WhenUserNotExists()
        {
            // Arrange
            var guid = Guid.NewGuid();

            // Act
            var actual = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => sut.GetByIdAsync<UserDetailsDTO>(guid, CancellationToken));

            // Assert
            actual.Message.Should().Be($"{typeof(User).Name} {guid} not found.");
        }

        /// <summary>
        /// Softs the delete asynchronous should set removed flag when user exists.
        /// </summary>
        [Fact]
        public async Task SoftDeleteAsync_ShouldSetRemovedFlag_WhenUserExists()
        {
            // Arrange
            var user = FakeData.User.Generate();

            portalDbContext.Set<User>().Add(user);
            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await sut.SoftDeleteAsync(user.Id, CancellationToken);

            // Assert
            actual.Id.Should().Be(user.Id);
            actual.Removed.Should().Be(true);
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IModelBaseRepository<User>>().Use<ModelBaseRepository<User>>();
        }
    }
}