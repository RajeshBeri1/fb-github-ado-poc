using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Lib.WebAPI.DTOs;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// IUserProvider
    /// </summary>
    public interface IUserProvider
    {
        /// <summary>
        /// Gets the current asynchronous.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<UserDetailsDTO> GetCurrentAsync(ClaimsPrincipal user, CancellationToken cancellationToken);
    }
}