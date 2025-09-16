using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using DocumentFormat.OpenXml.Presentation;
using FluentAssertions;
using JasperFx.Core;
using Lamar;
using Lib.Athena.Models;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.Extensions.Options;
using Moq;
using NSubstitute;
using Tests.WebAPI.Common;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// The FlowchartTemplatesHistortyControllerLogicTests
    /// </summary>
    public class FlowchartTemplatesHistortyControllerLogicTests : TestBase<IFlowchartTemplatesHistortyControllerLogic>
    {
        /// <summary>
        /// Blob service client mock
        /// </summary>
        private readonly Mock<BlobServiceClient> blobServiceClientMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplatesHistortyControllerLogicTests"/> class.
        /// The FlowchartTemplatesHistortyControllerLogicTests
        /// </summary>
        public FlowchartTemplatesHistortyControllerLogicTests()
        {
            blobServiceClientMock = new Mock<BlobServiceClient>();
        }

        /// <summary>
        /// Gets the asynchronous FlowchartTemplatesHistortyList method Should Throw KeyNotFoundException When wrong TemplateId is passed.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldThrowKeyNotFoundException_WhenTemplateIdIsPassed()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);
            var guidTemplate = Guid.NewGuid();

            var template = new FlowchartTemplate
            {
                Id = guidTemplate,
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

            var guidTemplateHistory = Guid.NewGuid();

            var templateHistory = new FlowchartTemplatesVersionHistorty
            {
                Id = guidTemplateHistory,
                FlowchartTemplateId = guidTemplate,
                FlowchartDefinition = { },
                Removed = false,
                Name = "Test1",
                Version = 1,
                Comments = "Test",
                CreatedByUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedByUserId = user.Id,
                ModifiedDate = DateTime.UtcNow,
            };

            portalDbContext.Set<FlowchartTemplatesVersionHistorty>().Add(templateHistory);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var search = new FlowchartTemplatesVersionHistortySearchDTO
            {
                Count = 10,
                OrderAscending = false,
                OrderBy = nameof(OmniClient.Id),
                Start = 5,
                SearchText = " ",
                FlowchartTemplateId = Guid.NewGuid(),
            };

            // Act

            // Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(async () => await sut.GetAsync(search, user.Id, CancellationToken));
        }

        /// <summary>
        /// Gets the asynchronous Should return FlowchartTemplatesVersionHistorty list when searching.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnFlowchartTemplatesVersionHistorty_WhenSearching()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);
            var guidTemplate = Guid.NewGuid();

            var template = new FlowchartTemplate
            {
                Id = guidTemplate,
                CreatedByUser = user,
                CreatedByUserId = user.Id,
                ModifiedByUserId = user.Id,
                ModifiedByUser = user,
                Name = Guid.NewGuid().ToString(),
                OmniClientId = Guid.NewGuid(),
            };

            portalDbContext.Set<FlowchartTemplate>().Add(template);

            var guidTemplateHistory = Guid.NewGuid();

            var templateHistory = new FlowchartTemplatesVersionHistorty
            {
                Id = guidTemplateHistory,
                FlowchartTemplateId = guidTemplate,
                FlowchartDefinition = { },
                Removed = false,
                Name = "Test1",
                Version = 1,
                Comments = "Test",
                CreatedByUserId = user.Id,
                CreatedDate = DateTime.UtcNow,
                ModifiedByUserId = user.Id,
                ModifiedDate = DateTime.UtcNow,
            };

            portalDbContext.Set<FlowchartTemplatesVersionHistorty>().Add(templateHistory);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var search = new FlowchartTemplatesVersionHistortySearchDTO
            {
                Count = 10,
                OrderAscending = false,
                OrderBy = nameof(OmniClient.Id),
                Start = 5,
                SearchText = " ",
                FlowchartTemplateId = guidTemplate,
            };

            // Act
            var actual = await sut.GetAsync(search, user.Id, CancellationToken);

            // Assert
            Assert.True(actual.TotalCount == 1);

        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IPortalUnitOfWork>().Use<PortalUnitOfWork>();
            registry.For<IFlowchartTemplatesHistortyControllerLogic>().Use<FlowchartTemplatesHistortyControllerLogic>();
            registry.For<OmniAuthConfig>().Use(new OmniAuthConfig { Enabled = false }).Singleton();
            registry.For<IUserProvider>().Use<UserProvider>();

            var log = Substitute.For<ILog<FlowchartTemplatesHistortyControllerLogic>>();
            registry.For<ILog<FlowchartTemplatesHistortyControllerLogic>>().Use(log).Singleton();
            var config = Substitute.For<IOptionsMonitor<StorageConfig>>();
            config.CurrentValue.Returns(new StorageConfig());
            registry.For<IOptionsMonitor<StorageConfig>>().Use(config).Singleton();
            var storage = Substitute.For<AzureBlobStorage>(blobServiceClientMock);
            registry.For<AzureBlobStorage>().Use(storage).Singleton();
        }
    }
}
