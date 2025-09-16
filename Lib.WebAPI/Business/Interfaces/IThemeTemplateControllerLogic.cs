using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
 /// <summary>
 /// IThemeTemplateControllerLogic
 /// </summary>
 public interface IThemeTemplateControllerLogic
 {
  /// <summary>
  /// Creates the asynchronous.
  /// </summary>
  /// <param name="themeTemplate">The theme template.</param>
  /// <param name="user">The user.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  Task<ThemeTemplateDetailsDTO> CreateAsync(ThemeTemplateCreateDTO themeTemplate, UserDetailsDTO user, CancellationToken cancellationToken);

  /// <summary>
  /// Deletes the asynchronous.
  /// </summary>
  /// <param name="id">The identifier.</param>
  /// <param name="userId">The user identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  Task<ThemeTemplateInfoDTO> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken);

  /// <summary>
  /// Gets the asynchronous.
  /// </summary>
  /// <param name="id">The identifier.</param>
  /// <param name="userId">The user identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  Task<ThemeTemplateDetailsDTO> GetAsync(Guid id, Guid userId, CancellationToken cancellationToken);

  /// <summary>
  /// Gets the asynchronous.
  /// </summary>
  /// <param name="search">The search.</param>
  /// <param name="userId">The user identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  Task<ThemeTemplateInfoListDTO> GetAsync(ThemeTemplateSearchDTO search, Guid userId, CancellationToken cancellationToken);

  /// <summary>
  /// Updates the asynchronous.
  /// </summary>
  /// <param name="themeTemplate">The theme template.</param>
  /// <param name="userId">The user identifier.</param>
  /// <param name="cancellationToken">The cancellation token.</param>
  Task<ThemeTemplateDetailsDTO> UpdateAsync(ThemeTemplateUpdateDTO themeTemplate, Guid userId, CancellationToken cancellationToken);
 }
}