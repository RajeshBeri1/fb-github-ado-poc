using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using JasperFx.Core;
using Lib.Athena.Business;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DefaultTemplateUpdater
    /// </summary>
    public class DefaultTemplateUpdater
    {
        private readonly ILog<DefaultTemplateUpdater> log;
        private readonly IFlowchartTemplateControllerLogic flowchartTemplateControllerLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultTemplateUpdater" /> class.
        /// </summary>
        /// <param name="log">The log.</param>
        /// <param name="flowchartTemplateControllerLogic">The flowchartTemplateControllerLogic.</param>"
        public DefaultTemplateUpdater(
            ILog<DefaultTemplateUpdater> log,
            IFlowchartTemplateControllerLogic flowchartTemplateControllerLogic)
        {
            this.log = log;
            this.flowchartTemplateControllerLogic = flowchartTemplateControllerLogic;
        }

        /// <summary>
        /// Runs the asynchronous.
        /// </summary>
        /// <param name="ansid">The ansid.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="isUpdate">The isUpdate</param>
        public async Task RunAsync(string ansid, CancellationToken cancellationToken, Guid? omniClientId, bool isUpdate = false)
        {
            try
            {
                omniClientId = (omniClientId.HasValue && omniClientId != Guid.Empty) ? omniClientId : Guid.Empty;

                log.Add(LogLevel.Information, $"Update default template started.");

                await flowchartTemplateControllerLogic.UpdateDefaultTemplateForAllClientAsync(ansid, cancellationToken, omniClientId, isUpdate);
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, $"Error updating default template", ex);
            }
        }
    }
}
