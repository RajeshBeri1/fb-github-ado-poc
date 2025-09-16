using System.Linq.Dynamic.Core;
using AutoMapper;
using Lamar;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.Configuration;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Tests.WebAPI.Common
{
    /// <summary>
    /// TestBase
    /// </summary>
    public abstract class TestBase<TInterface> : IDisposable
        where TInterface : class
    {
        /// <summary>
        /// The container
        /// </summary>
        protected IContainer container;

        /// <summary>
        /// The mapper
        /// </summary>
        protected IMapper mapper;

        /// <summary>
        /// The portal database context
        /// </summary>
        protected IPortalDbContext portalDbContext;

        /// <summary>
        /// The sut
        /// </summary>
        protected TInterface sut;

        private readonly CancellationTokenSource cancellationTokenSource = new();

        /// <summary>
        /// Gets the cancellation token.
        /// </summary>
        /// <value>The cancellation token.</value>
        protected CancellationToken CancellationToken => cancellationTokenSource.Token;

        /// <summary>
        /// Initializes a new instance of the <see cref="TestBase{TInterface}" /> class.
        /// </summary>
        public TestBase()
        {
            container = new Container(registry =>
            {
                var log = Substitute.For<ILog<TInterface>>();
                log.Add(Arg.Any<LogLevel>(), Arg.Any<string>());
                registry.For<ILog<TInterface>>().Use(log).Singleton();

                registry.For<IMapper>().Use(AutomapperConfiguration.GetMapper()).Singleton();

                registry.AddDbContext<IPortalDbContext, PortalDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));

                InitContainer(registry);
            });

            sut = container.GetInstance<TInterface>();
            portalDbContext = container.GetInstance<IPortalDbContext>();
            mapper = container.GetInstance<IMapper>();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or
        /// resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            container?.Dispose();
            cancellationTokenSource?.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected abstract void InitContainer(ServiceRegistry registry);

        /// <summary>
        /// Nameds the search.
        /// </summary>
        /// <param name="items">The items.</param>
        /// <param name="namedSearch">The named search.</param>
        protected (List<T> Items, int TotalCount) NamedSearch<T>(
            IEnumerable<T> items,
            NamedSearch namedSearch)
            where T : NamedModelBaseDTO
        {
            var query = items.AsQueryable();

            query = query.Where(x => namedSearch.SearchText == null || x.Name.Contains(namedSearch.SearchText));

            var totalCount = query.Count();

            if (!string.IsNullOrWhiteSpace(namedSearch.OrderBy))
            {
                query = namedSearch.OrderAscending ? query.OrderBy(namedSearch.OrderBy) : query.OrderBy($"{namedSearch.OrderBy} desc");
            }

            // default order, when not explicitly ordered
            if ((namedSearch.Start > 0 || namedSearch.Count > 0) && query.Expression.Type != typeof(IOrderedQueryable<T>))
            {
                query = query.OrderBy(x => x.Id);
            }

            if (namedSearch.Start > 0)
            {
                query = query.Skip(namedSearch.Start);
            }

            if (namedSearch.Count > 0)
            {
                query = query.Take(namedSearch.Count);
            }

            var result = query.ToList();

            return (result, totalCount);
        }
    }
}