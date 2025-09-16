using DocumentFormat.OpenXml.Drawing;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// common services
    /// </summary>
    public class CommonServices : ICommonServices
    {
        private readonly IPortalUnitOfWork portal;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonServices"/> class.
        /// The CommonService constructor
        /// </summary>
        /// <param name="portal">The portal.</param>
        public CommonServices(IPortalUnitOfWork portal)
        {
            this.portal = portal;
        }

        /// <summary>
        /// check duplicate component name asynchronous.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check dto.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<bool> CheckDuplicateComponentNameAsync(DuplicateNameCheckDto duplicateNameCheckDto, UserDetailsDTO user, CancellationToken cancellationToken)
        {
            duplicateNameCheckDto.ThrowIfNull(nameof(duplicateNameCheckDto));
            duplicateNameCheckDto.ComponentName.ThrowIfNull(nameof(duplicateNameCheckDto.ComponentName));
            bool result = duplicateNameCheckDto.ComponentName switch
            {
                FlowChartComponent.Header => await portal.HeaderTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.Calendar => await portal.CalendarTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.CalendarOverlay => await portal.CalendarOverlayTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.MediaHierarchy => await portal.MediaHierarchyTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.GrandTotals => await portal.GrandTotalTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.RightHandTotals => await portal.TotalsTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.Theme => await portal.ThemeTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                FlowChartComponent.Footer => await portal.FooterTemplates.AnyAsync(cancellationToken, x => x.Name == duplicateNameCheckDto.Name && x.OmniClientId == duplicateNameCheckDto.OmniClientId),
                _ => false,
            };
            return result;
        }
    }
}
