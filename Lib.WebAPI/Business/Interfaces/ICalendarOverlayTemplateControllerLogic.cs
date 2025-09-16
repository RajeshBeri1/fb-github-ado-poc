using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// ICalendarOverlayTemplateControllerLogic
    /// </summary>
    public interface ICalendarOverlayTemplateControllerLogic
    {
        /// <summary>
        /// Creates the asynchronous.
        /// </summary>
        /// <param name="calendarOverlayTemplate">The calendar Overlay template.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<CalendarOverlayTemplateDetailsDTO> CreateAsync(CalendarOverlayTemplateCreateDTO calendarOverlayTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<CalendarOverlayTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<CalendarOverlayTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets the asynchronous.
        /// </summary>
        /// <param name="search">The named search.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<CalendarOverlayTemplateInfoListDTO> GetAsync(CalendarOverlayTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates the asynchronous.
        /// </summary>
        /// <param name="calendarOverlayTemplate">The calendar Overlay template.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<CalendarOverlayTemplateDetailsDTO> UpdateAsync(
            CalendarOverlayTemplateUpdateDTO calendarOverlayTemplate, Guid userId, CancellationToken cancellationToken);
    }
}