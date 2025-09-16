using Amazon.Athena;
using Lamar;
using Lib.Athena.Business;
using Lib.Athena.Business.Interfaces;
using Lib.Athena.Models;
using Microsoft.Extensions.Options;
using NSubstitute;
using Tests.WebAPI.Common;

namespace Tests.WebAPI.BusinessTests
{
    /// <summary>
    /// AthenaQueryLogicTests
    /// </summary>
    public class AthenaQueryLogicTests : TestBase<IAthenaQueryLogic>
    {
        private const string MediaPlansTable = "media_plans";

        /// <summary>
        /// Initializes the container.
        /// </summary>
        /// <param name="registry">The registry.</param>
        protected override void InitContainer(ServiceRegistry registry)
        {
            registry.For<IAmazonAthena>().Use(Substitute.For<IAmazonAthena>()).Singleton();

            var config = Substitute.For<IOptionsMonitor<AthenaQueryConfig>>();
            config.CurrentValue.Returns(new AthenaQueryConfig());
            registry.For<IOptionsMonitor<AthenaQueryConfig>>().Use(config).Singleton();

            registry.For<IAthenaQueryLogic>().Use<AthenaQueryLogic>();
        }
    }
}