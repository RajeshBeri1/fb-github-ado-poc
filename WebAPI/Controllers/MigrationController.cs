using Hangfire;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Net.Http.Headers;
using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Controllers
{
    /// <summary>
    /// MigrationController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class MigrationController : ControllerBase
    {
        private readonly IUserProvider userProvider;
        private readonly IMigrationControllerLogic migrationControllerLogic;

        /// <summary>
        /// Initializes a new instance of the <see cref="MigrationController" />
        /// class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="migrationControllerLogic">The migrationControllerLogic.</param>
        public MigrationController(IUserProvider userProvider, IMigrationControllerLogic migrationControllerLogic)
        {
            this.userProvider = userProvider;
            this.migrationControllerLogic = migrationControllerLogic;
        }

        /// <summary>
        /// Create the migration data asynchronous.
        /// </summary>
        /// <param name="createPlannedMediaMigrationDTO">createPlannedMediaMigrationDTO</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateMigration(List<CreatePlannedMediaMigrationDTO> createPlannedMediaMigrationDTO, CancellationToken cancellationToken)
        {
            bool data = await migrationControllerLogic.CreateMigrationAsync(createPlannedMediaMigrationDTO, cancellationToken);
            if (data)
            {
                return Ok("Created migration record successfully");
            }
            else
            {
                return Ok("Record already exist");
            }
        }

        /// <summary>
        /// Update the migration data asynchronous.
        /// </summary>
        /// <param name="updatePlannedMediaMigrationDTO">updatePlannedMediaMigrationDTO</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPatch("[action]")]
        public async Task<IActionResult> UpdateMigration(UpdatePlannedMediaMigrationDTO updatePlannedMediaMigrationDTO, CancellationToken cancellationToken)
        {
            await migrationControllerLogic.UpdateMigrationAsync(updatePlannedMediaMigrationDTO, cancellationToken);
            return Ok("Updated migration record successfully");
        }

        /// <summary>
        /// Gets the migration list.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        [HttpPost("[action]")]
        public async Task<List<PlannedMediaMigrationColumnMapping>> List(CancellationToken cancellationToken, bool removed = false)
        {
            return await migrationControllerLogic.GetAllAsync(cancellationToken, removed);
        }

        /// <summary>
        /// Deletes the migration record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await migrationControllerLogic.DeleteAsync(id, user.Id, cancellationToken);
            return Ok("Migration record deleted successfully");
        }

        /// <summary>
        /// Migrate flowchart defintion by using omniGuid asynchronous.
        /// </summary>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="connectionId">The connectionId.</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> MigrateClientToNewVersion(Guid omniClientId, string connectionId, CancellationToken cancellationToken)
        {
            connectionId ??= string.Empty; // Fix: Ensure connectionId is not null
            var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
            var backGroundJobId = BackgroundJob.Enqueue(() => migrationControllerLogic.MigrateClientToNewVersionAsync(omniClientId, connectionId, backgroundJobCancellation.Token));
            return Ok("Migration to Newer version is started in background");
        }

        /// <summary>
        /// Migrate flowchart defintion by using omniGuid asynchronous.
        /// </summary>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="connectionId">The connectionId.</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> MigrateClientToInitialVersion(Guid omniClientId, string connectionId, CancellationToken cancellationToken)
        {
            connectionId ??= string.Empty; // Fix: Ensure connectionId is not null
            var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));
            var backGroundJobId = BackgroundJob.Enqueue(() => migrationControllerLogic.MigrateClientToInitialVersionAsync(omniClientId, connectionId, backgroundJobCancellation.Token));
            return Ok("Migration to Initial Version is started in background");
        }

        /// <summary>
        /// Get MediaHierarchy Column Uses Details.
        /// </summary>
        /// <param name="name">name</param>
        /// <param name="omniClientId">omniClientId</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<MediaHierarchyColumnsDetailsResultDTO> GetMediaHierarchyColumnUsesDetails(string? name, Guid? omniClientId, CancellationToken cancellationToken)
        {
            var backgroundJobCancellation = new CancellationTokenSource(TimeSpan.FromHours(2));

            return await migrationControllerLogic.GetMediaHierarchyColumnUsesDetailsAsync(name, omniClientId, backgroundJobCancellation.Token);
        }
    }
}
