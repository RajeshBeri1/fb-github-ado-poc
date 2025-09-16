using Lib.Common.Business;
using Lib.MediaopsToFlowChart.DTO;
using Lib.MediaopsToFlowChart.Models;
using Newtonsoft.Json;
using OneOf.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Throw;
using static System.Net.WebRequestMethods;

namespace Lib.MediaopsToFlowChart.Business
{
    /// <summary>
    /// MediaopsToFlowChartImportApi
    /// </summary>
    public class MediaopsToFlowChartImportApi
    {
        /// <summary>
        /// The client name
        /// </summary>
        public const string ClientName = nameof(MediaopsToFlowChartImportApi);

        private readonly IHttpClientFactory httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaopsToFlowChartImportApi" /> class.
        /// </summary>
        /// <param name="httpClientFactory">The HTTP client factory.</param>
        public MediaopsToFlowChartImportApi(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public string GetMainUrl(string anSid, Guid id)
        {
            return $"https://devmediaops-api.annalect.com/dev/docs?ANsid={anSid}&clientid={id}";
        }

        /// <summary>
        /// Gets the user profile asynchronous.
        /// </summary>
        /// <param name="mediaopsColumnSearchDTO">The mediaopsColumnSearchDTO.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<List<MediaopsHierarchyDetails>> GetTableDataFromMediaopsAsync(MediaopsColumnSearchDTO mediaopsColumnSearchDTO, string ansid, CancellationToken cancellationToken)
        {
            using var client = httpClientFactory.CreateClient(ClientName);
            //client.BaseAddress = new Uri($"https://devmediaops-api.annalect.com/");
            client.DefaultRequestHeaders.Accept.Clear();
            //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));//ACCEPT header
            client.DefaultRequestHeaders.Add("ansid", ansid);
            client.DefaultRequestHeaders.Add("clientid", mediaopsColumnSearchDTO.ClientId);
            //client.DefaultRequestHeaders.Add("Referer", "https://devmediaops-api.annalect.com");
            client.DefaultRequestHeaders.Add("User-Agent", "FB");

            var url = $"data_templates?clientId={mediaopsColumnSearchDTO.ClientId}";

            if (!string.IsNullOrEmpty(mediaopsColumnSearchDTO.StartDate))
            {
                url += $"&startDate={mediaopsColumnSearchDTO.StartDate}";
            }
            if (!string.IsNullOrEmpty(mediaopsColumnSearchDTO.EndDate))
            {
                url += $"&endDate={mediaopsColumnSearchDTO.EndDate}";
            }
            if (!string.IsNullOrEmpty(mediaopsColumnSearchDTO.Status))
            {
                url += $"&status={mediaopsColumnSearchDTO.Status}";
            }
            if (!string.IsNullOrEmpty(mediaopsColumnSearchDTO.Name))
            {
                url += $"&name={mediaopsColumnSearchDTO.Name}";
            }
            if (mediaopsColumnSearchDTO.Id != null && mediaopsColumnSearchDTO.Id != 0)
            {
                url += $"&id={mediaopsColumnSearchDTO.Id}";
            }

            using var response = await client.GetAsync(url, cancellationToken);

            response.ThrowIfNull().IfFalse(x => x.StatusCode == HttpStatusCode.OK);

            var json = await response.Content.ReadAsStringAsync(cancellationToken);

            var result = Json.Deserialize<List<MediaopsHierarchyDetails>>(json);

            result.ThrowIfNull();

            return result;

        }
    }
}
