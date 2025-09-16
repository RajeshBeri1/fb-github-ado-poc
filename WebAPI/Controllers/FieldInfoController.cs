using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.CodeAnalysis;

namespace WebAPI.Controllers
{
    /// <summary>
    /// CommonController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class FieldInfoController : ControllerBase
    {
        private readonly IUserProvider userProvider;
        private readonly IFieldInfoServices fieldInfoServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoController" />
        /// class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="fieldInfoServices">The common services.</param>
        public FieldInfoController(IUserProvider userProvider, IFieldInfoServices fieldInfoServices)
        {
            this.userProvider = userProvider;
            this.fieldInfoServices = fieldInfoServices;
        }

        /// <summary>
        /// Create the CreateFieldInfo asynchronous.
        /// </summary>
        /// <param name="fieldInfoDto">fieldInfoDto</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPost("[action]")]
        public async Task<IActionResult> CreateFieldInfo(List<FieldInfoDto> fieldInfoDto, CancellationToken cancellationToken)
        {
            bool data = await fieldInfoServices.CreateFieldInfoAsync(fieldInfoDto, cancellationToken);
            if (data)
            {
                return Ok("Created new record successfully");
            }
            else
            {
                return Ok("Record already exist");
            }
        }

        /// <summary>
        /// Update the UpdateFieldInfo asynchronous.
        /// </summary>
        /// <param name="updateFieldInfoDto">updateFieldInfoDto</param>
        /// <param name="cancellationToken">cancellationToken</param>
        /// <returns>A <see cref="Task{TResult}"/> representing the result of the asynchronous operation.</returns>
        [HttpPatch("[action]")]
        public async Task<IActionResult> UpdateFieldInfo(UpdateFieldInfoDto updateFieldInfoDto, CancellationToken cancellationToken)
        {
            await fieldInfoServices.UpdateFieldInfoAsync(updateFieldInfoDto, cancellationToken);
            return Ok("Field Info data updated successfully");
        }

        /// <summary>
        /// Get all the FieldInfo list.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <param name="removed">The Removed.</param>
        [HttpPost("[action]")]
        public async Task<List<FieldInfo>> List(CancellationToken cancellationToken, bool removed = false)
        {
            return await fieldInfoServices.GetAllAsync(cancellationToken, removed);
        }

        /// <summary>
        /// Deletes the FieldInfo record.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpDelete("[action]/{id}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            await fieldInfoServices.DeleteAsync(id, user.Id, cancellationToken);
            return Ok("Field Info record deleted successfully");
        }
    }
}
