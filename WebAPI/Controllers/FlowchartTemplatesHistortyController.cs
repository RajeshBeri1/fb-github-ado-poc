using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.Business;
using Microsoft.AspNetCore.Mvc;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Controllers
{
    /// <summary>
    /// FlowchartTemplatesHistortyController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class FlowchartTemplatesHistortyController : ControllerBase
    {
        private readonly IFlowchartTemplatesHistortyControllerLogic _controllerLogic;
        private readonly IUserProvider _userProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="FlowchartTemplatesHistortyController" />
        /// class.
        /// </summary>
        /// <param name="controllerLogic">The controller logic.</param>
        /// <param name="userProvider">The user provider.</param>
        public FlowchartTemplatesHistortyController(IFlowchartTemplatesHistortyControllerLogic controllerLogic, IUserProvider userProvider)
        {
            _controllerLogic = controllerLogic;
            _userProvider = userProvider;
        }

        /// <summary>
        /// Lists the specified search.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<FlowchartTemplatesVersionHistortiesListDTO> List(FlowchartTemplatesVersionHistortySearchDTO search, CancellationToken cancellationToken)
        {
            var user = await _userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await _controllerLogic.GetAsync(search, user.Id, cancellationToken);

        }
    }
}
