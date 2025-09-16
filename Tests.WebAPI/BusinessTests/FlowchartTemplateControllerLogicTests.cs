using Azure.Storage.Blobs;
using FluentAssertions;
using Lamar;
using Lib.Annalect.Models;
using Lib.Athena.Business;
using Lib.Athena.Models;
using Lib.Aurora.Business;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.Common.Models;
using Lib.MediaopsToFlowChart.Business;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tests.WebAPI.Common;
using ZiggyCreatures.Caching.Fusion;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// FlowchartTemplateControllerLogicTests
    /// </summary>
    public class FlowchartTemplateControllerLogicTests : TestBase<IFlowchartTemplateControllerLogic>
    {
        private const string OwnerMessage = $"Owner is a different user.";

        /// <summary>
        /// Creates the asynchronous should return template when template is created.
        /// </summary>
        [Fact]
        public async Task CreateAsync_ShouldReturnTemplate_WhenTemplateIsCreated()
        {
            // Arrange

            var template = new FlowchartTemplateCreateDTO
            {
                TemplateName = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);
            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await sut.CreateAsync(template, mapper.Map<UserDetailsDTO>(user), CancellationToken);

            // Assert
            actual.Should().NotBeNull();
            actual.Name.Should().Be(template.TemplateName);
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

            var template = new FlowchartTemplate
            {
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

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

            var template = new FlowchartTemplate
            {
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

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

            var template = new FlowchartTemplate
            {
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var templateDto = mapper.Map<FlowchartTemplateDetailsDTO>(template);

            // Act
            // Considering as zero version if just created
            templateDto.Version=0;
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

            var templates = new List<FlowchartTemplateInfoDTO>();

            var guid = Guid.NewGuid();

            for (int i = 0; i < 100; i++)
            {
                var template = new FlowchartTemplate
                {
                    CreatedByUser = user,
                    CreatedByUserId = user.Id,
                    ModifiedByUserId = user.Id,
                    ModifiedByUser = user,
                    Name = $"{i}",
                    OmniClientId = guid,
                };

                portalDbContext.Set<FlowchartTemplate>().Add(template);

                templates.Add(mapper.Map<FlowchartTemplateInfoDTO>(template));
            }

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var search = new FlowchartTemplateSearchDTO
            {
                Count = 10,
                OrderAscending = false,
                OrderBy = nameof(CalendarTemplate.Name),
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

            var template = new FlowchartTemplate
            {
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var update = new FlowchartTemplateUpdateDTO
            {
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

            var template = new FlowchartTemplate
            {
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var update = new FlowchartTemplateUpdateDTO
            {
                Id = template.Id,
                Name = Guid.NewGuid().ToString(),
            };

            // Act
            var actual = await Assert.ThrowsAsync<ArgumentException>(() => sut.UpdateAsync(update, Guid.NewGuid(), CancellationToken));

            // Assert
            actual.Message.Should().Be(OwnerMessage);
        }

        /// <summary>
        /// Update Flowchart Template Name By Id asynchronous should return template when owner is same.
        /// </summary>
        [Fact]
        public async Task UpdateFlowchartTemplateNameByIdAsync_ShouldReturnTemplate_WhenOwnerIsSame()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var template = new FlowchartTemplate
            {
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var update = new UpdateFlowchartTemplateNameDto
            {
                FlowchartTemplateId = template.Id,
                TemplateName = Guid.NewGuid().ToString(),
                OmniClientId = template.OmniClientId,
            };

            // Act
            var actual = await sut.UpdateFlowChartTemplateNameByIdAsync(update, user.Id, CancellationToken);

            // Write Assert here
            actual.Should().NotBeNull();
            actual.Should().Be("Flowchart template name updated successfully.");

        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IPortalUnitOfWork>().Use<PortalUnitOfWork>();
            registry.For<IFlowchartTemplateControllerLogic>().Use<FlowchartTemplateControllerLogic>();
            registry.For<IClientControllerLogic>().Use<ClientControllerLogic>();
            registry.For<IUserProvider>().Use<UserProvider>();
            registry.For<IMigrationControllerLogic>().Use<MigrationControllerLogic>();
            registry.For<DataControllerLogic>().Use<DataControllerLogic>();
            registry.For<DataControllerLogicPmds>().Use<DataControllerLogicPmds>();
            registry.For<IHttpContextAccessor>().Use<HttpContextAccessor>();

            registry.For<IHttpClientFactory>().Use(Substitute.For<IHttpClientFactory>());

            var configuration = Substitute.For<IConfiguration>();
            registry.For<IConfiguration>().Use(configuration).Singleton();

            registry.For<MediaopsToFlowChartImportApi>().Use<MediaopsToFlowChartImportApi>();
            registry.For<ICacheLogic>().Use<CacheLogic>();
            registry.For<IAuroraQueryLogic>().Use<AuroraQueryLogic>();
            registry.For<AthenaDataConverter>().Use<AthenaDataConverter>();

            var cache = Substitute.For<IFusionCache>();
            registry.For<IFusionCache>().Use(cache).Singleton();

            var log1 = Substitute.For<ILog<DataControllerLogic>>();
            registry.For<ILog<DataControllerLogic>>().Use(log1).Singleton();

            var log2 = Substitute.For<ILog<DataControllerLogicPmds>>();
            registry.For<ILog<DataControllerLogicPmds>>().Use(log2).Singleton();

            var log3 = Substitute.For<ILog<IClientControllerLogic>>();
            registry.For<ILog<IClientControllerLogic>>().Use(log3).Singleton();

            var log4 = Substitute.For<ILog<AthenaDataConverter>>();
            registry.For<ILog<AthenaDataConverter>>().Use(log4).Singleton();

            var log5 = Substitute.For<ILog<IAuroraQueryLogic>>();
            registry.For<ILog<IAuroraQueryLogic>>().Use(log5).Singleton();

            var log6 = Substitute.For<ILog<IMigrationControllerLogic>>();
            registry.For<ILog<IMigrationControllerLogic>>().Use(log6).Singleton();

            var config = Substitute.For<IOptionsMonitor<ConnectionStrings>>();
            config.CurrentValue.Returns(new ConnectionStrings());
            registry.For<IOptionsMonitor<ConnectionStrings>>().Use(config).Singleton();

            var storageConfig = Substitute.For<IOptionsMonitor<StorageConfig>>();
            storageConfig.CurrentValue.Returns(new StorageConfig());
            registry.For<IOptionsMonitor<StorageConfig>>().Use(storageConfig).Singleton();

            registry.For<AzureBlobStorage>().Use<AzureBlobStorage>().Singleton();

            var blobServiceClient = Substitute.For<BlobServiceClient>();
            registry.For<BlobServiceClient>().Use(blobServiceClient).Singleton();

            var blobContainerClient = Substitute.For<BlobContainerClient>();
            registry.For<BlobContainerClient>().Use(blobContainerClient).Singleton();

            var defaultTemplateUpdaterConfig = Substitute.For<IOptionsMonitor<DefaultTemplateUpdaterConfig>>();
            defaultTemplateUpdaterConfig.CurrentValue.Returns(new DefaultTemplateUpdaterConfig());
            registry.For<IOptionsMonitor<DefaultTemplateUpdaterConfig>>().Use(defaultTemplateUpdaterConfig).Singleton();
            registry.For<IHubContext<NotificationHub, INotificationClient>>().Use(Substitute.For<IHubContext<NotificationHub, INotificationClient>>()).Singleton();
        }
    }
}