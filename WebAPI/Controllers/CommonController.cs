using System.Diagnostics.CodeAnalysis;
using Lib.WebAPI.Business;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Enumerations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    /// <summary>
    /// CommonController
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [ExcludeFromCodeCoverage]
    public class CommonController : ControllerBase
    {
        private readonly IUserProvider userProvider;
        private readonly ICommonServices commonServices;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonController" />
        /// class.
        /// </summary>
        /// <param name="userProvider">The user provider.</param>
        /// <param name="commonServices">The common services.</param>
        public CommonController(IUserProvider userProvider, ICommonServices commonServices)
        {
            this.userProvider = userProvider;
            this.commonServices = commonServices;
        }

        /// <summary>
        /// Check the duplicate component name.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check Dto.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        [HttpPost("[action]")]
        public async Task<bool> CheckDuplicateComponentName(DuplicateNameCheckDto duplicateNameCheckDto, CancellationToken cancellationToken)
        {
            var user = await userProvider.GetCurrentAsync(HttpContext.User, cancellationToken);
            return await commonServices.CheckDuplicateComponentNameAsync(duplicateNameCheckDto, user, cancellationToken);
        }
    }
}