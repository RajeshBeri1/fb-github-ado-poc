using Lib.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IFlowchartTemplatesHistortyControllerLogic
    /// </summary>
    public interface IFlowchartTemplatesHistortyControllerLogic
    {
        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<FlowchartTemplatesVersionHistortiesListDTO> GetAsync(FlowchartTemplatesVersionHistortySearchDTO search, Guid userId, CancellationToken cancellationToken);
    }
}
