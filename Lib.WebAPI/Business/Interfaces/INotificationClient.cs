using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business.Interfaces
{
    /// <summary>
    /// Defines methods for sending notifications.
    /// </summary>
    public interface INotificationClient
    {
        /// <summary>
        /// Sends a notification asynchronously.
        /// </summary>
        /// <param name="message">The message to be sent as a notification.</param>
        /// <param name="isError">The error.</param>
        /// <param name="identifier"></param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task ReceiveNotification(string message, bool isError, string identifier);
    }
}
