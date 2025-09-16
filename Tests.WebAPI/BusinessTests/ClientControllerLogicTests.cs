using Lamar;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Tests.WebAPI.Common;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// ClientControllerLogicTests
    /// </summary>
    public class ClientControllerLogicTests : TestBase<IClientControllerLogic>
    {
        /// <summary>
        /// Create the client asynchronous.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the result of the asynchronous operation.</returns>
        [Fact]
        public async Task CreateAsync_ShouldReturnClient_WhenClientIsCreated()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var template = new OmniClientInfoDTO
            {
                Client = new ClientInfoDTO() { ClientId = guid.ToString(), Name = "kimberly-clark" , IncludeSourceMediaToolsData = false },
                Country = "US",
                Id = Guid.NewGuid(),
                Removed = false,
            };

            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);
            Guid userid = user.Id;
            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = Record.ExceptionAsync(()=> sut.CreateAsync(template, userid, CancellationToken)).Exception;

            // Assert
            Assert.Null(actual);
        }

        /// <summary>
        /// Gets the asynchronous should return Client list when searching.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnClient_WhenSearching()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);
            var guid = Guid.NewGuid();
            var template = new Client
            {
                ClientId = guid.ToString(),
                Id = guid,
                Removed = false,
                Name = "Netherlands",
                IncludeSourceMediaToolsData = false,
                OmniClients = new List<OmniClient>()
                {
                    new OmniClient()
                    {
                        ClientId = guid,
                        Country = "Netherlands",
                        Removed = false,
                        Id = guid,
                    },
                },
            };

            portalDbContext.Set<Client>().Add(template);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            var search = new OmniClientSearchDTO
            {
                Count = 10,
                OrderAscending = false,
                OrderBy = nameof(OmniClient.ClientId),
                Start = 5,
                SearchText = "Netherlands",
            };

            // Act
            var actual = await sut.GetListAsync(search, user.Id, CancellationToken);

            // Assert
            Assert.True(actual.TotalCount == 1);
        }

        /// <summary>
        /// Gets the asynchronous should return Client when Client already exists.
        /// </summary>
        [Fact]
        public async Task GetAsync_ShouldReturnClient_WhenClientExists()
        {
            // Arrange
            var user = FakeData.User.Generate();
            portalDbContext.Set<User>().Add(user);

            var guid = Guid.NewGuid();
            var client = new Client
            {
                ClientId = guid.ToString(),
                Id = guid,
                Removed = false,
                Name = "Netherlands",
                IncludeSourceMediaToolsData = false,
                OmniClients = new List<OmniClient>()
                {
                    new OmniClient()
                    {
                        ClientId = guid,
                        Country = "Netherlands",
                        Removed = false,
                        Id = guid,
                    },
                },
            };

            portalDbContext.Set<Client>().Add(client);

            await portalDbContext.SaveChangesAsync(CancellationToken);

            // Act
            var actual = await sut.GetAsync(client.Id, user.Id, CancellationToken);

            // Assert
            Assert.True(actual.Id == client.Id);
            Assert.Equal(actual.Client.Name, client.Name);
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IPortalUnitOfWork>().Use<PortalUnitOfWork>();
            registry.For<IClientControllerLogic>().Use<ClientControllerLogic>();
            registry.For<OmniAuthConfig>().Use(new OmniAuthConfig { Enabled = false }).Singleton();
            registry.For<IUserProvider>().Use<UserProvider>();
        }
    }
}
