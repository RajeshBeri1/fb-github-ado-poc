using Lib.WebAPI.Business.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// NotificationHub
    /// </summary>
    public class NotificationHub : Hub<INotificationClient>
    {
        /// <summary>
        /// Called when a new connection is established with the hub.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Sends a notification message to all connected clients.
        /// </summary>
        /// <param name="message">The notification message to send.</param>
        /// <param name="isError"></param>
        /// <param name="identifier"></param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        public async Task SendNotificationAsync(string message, bool isError, string identifier)
        {
            await Clients.All.ReceiveNotification(message, isError, identifier);
        }

        /// <summary>
        /// Gets the connection ID for the current connection.
        /// </summary>
        /// <returns>The connection ID.</returns>
        public string GetConnectionId()
        {
            return Context.ConnectionId;
        }
    }
}
