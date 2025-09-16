using Azure.Storage.Blobs;
using FluentAssertions;
using Lamar;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Flowchart;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using Tests.WebAPI.Common;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// CalendarOverlayTemplateControllerLogicTests
    /// </summary>
    public class CalendarOverlayTemplateControllerLogicTests : TestBase<ICalendarOverlayTemplateControllerLogic>
    {
        private const string OwnerMessage = $"Owner is a different user.";

        private readonly Mock<BlobServiceClient> _blobServiceClientMock;

        public CalendarOverlayTemplateControllerLogicTests()
        {
            _blobServiceClientMock = new Mock<BlobServiceClient>();
        }
        /// <summary>
        /// Creates the asynchronous should return template when template is created.
        /// </summary>
        [Fact]
        public async Task CreateAsync_ShouldReturnTemplate_WhenTemplateIsCreated()
        {
            // Arrange

            var template = new CalendarOverlayTemplateCreateDTO
            {
                Definition = new CalendarOverlayDefinition(),
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);
            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await sut.CreateAsync(template, mapper.Map<UserDetailsDTO>(user), CancellationToken);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(template.Name);
        }

        /// <summary>
        /// Deletes the asynchronous should return template when owner is same.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ShouldReturnTemplate_WhenOwnerIsSame()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var template = new CalendarOverlayTemplate
            {
                CalendarOverlayDefinition = "{}",
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<CalendarOverlayTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await sut.DeleteAsync(template.Id, user.Id, CancellationToken);

            // Assert
            actual.Id.Should().Be(template.Id);
            actual.Removed.Should().Be(true);
        }

        /// <summary>
        /// Deletes the asynchronous should throw exception when owner is different.
        /// </summary>
        [Fact]
        public async Task DeleteAsync_ShouldThrowException_WhenOwnerIsDifferent()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var template = new CalendarOverlayTemplate
            {
                CalendarOverlayDefinition = "{}",
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<CalendarOverlayTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.DeleteAsync(template.Id, Guid.NewGuid(), CancellationToken));

            // Assert
            actual.Message.Should().Be(OwnerMessage);
        }

        /// <summary>
        /// Gets the asynchronous should return template when template exists.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnTemplate_WhenTemplateExists()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var template = new CalendarOverlayTemplate
            {
                CalendarOverlayDefinition = "{}",
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<CalendarOverlayTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var templateDto = mapper.Map<CalendarOverlayTemplateDetailsDTO>(template);

            // Act
            var actual = await sut.GetAsync(template.Id, user.Id, CancellationToken);

            // Assert
            actual.Should().BeEquivalentTo(templateDto);
        }

        /// <summary>
        /// Gets the asynchronous should return templates when searching.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnTemplates_WhenSearching()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var templates = new List<CalendarOverlayTemplateInfoDTO>();

            var guid = Guid.NewGuid();

            for (int i = 0; i < 100; i++)
            {
                var template = new CalendarOverlayTemplate
                {
                    CalendarOverlayDefinition = "{}",
                    CreatedByUser = user,
                    CreatedByUserId = user.Id,
                    ModifiedByUserId = user.Id,
                    ModifiedByUser = user,
                    Name = $"{i}",
                    OmniClientId = guid,
                };

                portalDbContext.Set<CalendarOverlayTemplate>().Add(template);

                templates.Add(mapper.Map<CalendarOverlayTemplateInfoDTO>(template));
            }

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var search = new CalendarOverlayTemplateSearchDTO
            {
                Count = 10,
                OrderAscending = false,
                Removed = false,
                OrderBy = nameof(CalendarOverlayTemplate.Name),
                Start = 5,
                OmniClientId = guid,
            };

            templates = NamedSearch(templates, search).Items;

            // Act
            var actual = await sut.GetAsync(search, user.Id, CancellationToken);

            // Assert
            actual.Items.Should().BeEquivalentTo(templates);
        }

        /// <summary>
        /// Updates the asynchronous should return template when owner is same.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ShouldReturnTemplate_WhenOwnerIsSame()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var template = new CalendarOverlayTemplate
            {
                CalendarOverlayDefinition = "{}",
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<CalendarOverlayTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var update = new CalendarOverlayTemplateUpdateDTO
            {
                Definition = new(),
                Id = template.Id,
                Name = Guid.NewGuid().ToString(),
            };

            // Act
            var actual = await sut.UpdateAsync(update, user.Id, CancellationToken);

            // Assert
            actual.Name.Should().Be(update.Name);
        }

        /// <summary>
        /// Updates the asynchronous should throw exception when owner is different.
        /// </summary>
        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenOwnerIsDifferent()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var template = new CalendarOverlayTemplate
            {
                CalendarOverlayDefinition = "{}",
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<CalendarOverlayTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var update = new CalendarOverlayTemplateUpdateDTO
            {
                Definition = new(),
                Id = template.Id,
                Name = Guid.NewGuid().ToString(),
            };

            // Act
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.UpdateAsync(update, Guid.NewGuid(), CancellationToken));

            // Assert
            actual.Message.Should().Be(OwnerMessage);
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IPortalUnitOfWork>().Use<PortalUnitOfWork>();
            registry.For<ICalendarOverlayTemplateControllerLogic>().Use<CalendarOverlayTemplateControllerLogic>();

            var log = Substitute.For<ILog<CalendarOverlayTemplateControllerLogic>>();
            registry.For<ILog<CalendarOverlayTemplateControllerLogic>>().Use(log).Singleton();
            var config = Substitute.For<IOptionsMonitor<StorageConfig>>();
            config.CurrentValue.Returns(new StorageConfig());
            registry.For<IOptionsMonitor<StorageConfig>>().Use(config).Singleton();
            var storage = Substitute.For<AzureBlobStorage>(_blobServiceClientMock);
            registry.For<AzureBlobStorage>().Use(storage).Singleton();
        }
    }
}