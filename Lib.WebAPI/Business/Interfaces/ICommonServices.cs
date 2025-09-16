using Lib.WebAPI.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// Common services interface
    /// </summary>
    public interface ICommonServices
    {
        /// <summary>
        /// check duplicate component name asynchronous.
        /// </summary>
        /// <param name="duplicateNameCheckDto">The duplicate name check dto.</param>
        /// <param name="user">The user.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        Task<bool> CheckDuplicateComponentNameAsync(DuplicateNameCheckDto duplicateNameCheckDto, UserDetailsDTO user, CancellationToken cancellationToken);
    }
}
