using AutoMapper;
using Lib.Athena.Business;
using Lib.Athena.Consts;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business.Interfaces;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.Models;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// CommonDataControllerLogic
    /// </summary>
    public class CommonDataControllerLogic
    {

        private readonly IPortalUnitOfWork portal;
        private readonly OmniAuthConfig omniAuthConfig;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommonDataControllerLogic" /> class.
        /// </summary>
        /// <param name="portal">portal</param>
        /// <param name="omniAuthConfig">omniAuthConfig</param>
        public CommonDataControllerLogic(
            IPortalUnitOfWork portal,
            OmniAuthConfig omniAuthConfig)
        {
            this.portal = portal;
            this.omniAuthConfig = omniAuthConfig;
        }

        /// <summary>
        /// SQLs the quote.
        /// </summary>
        /// <param name="text">The text.</param>
        public static string SqlQuote(string text)
        {
            return $"'{text.Replace("'", "''")}'";
        }

        public string GetOmniClientIdRestrictions(string omniClientId, bool isBriefRequired = true)
        {
            if (!omniAuthConfig.ClientIdCheck)
            {
                return string.Empty;
            }

            if (!isBriefRequired)
            {
                return $@"lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
            }
            return $@"lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid}) IN {omniClientId} AND
                      lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
        }

        public string GetOmniClientIdRestrictionsWithData(string omniClientId, bool? isBriefRequired)
        {
            if (!omniAuthConfig.ClientIdCheck)
            {
                return string.Empty;
            }
            if (!isBriefRequired.HasValue)
            {
                return $@"lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
            }
            else if (isBriefRequired.Value)
            {
                return $@"lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid}) IN {omniClientId} AND
                      lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
            }
            return $@"lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
        }

        public string GetOmniClientIdRestrictionsBasedOnTable(string omniClientId, string tableName, bool isWithinDynamicFilter = false)
        {
            if (!omniAuthConfig.ClientIdCheck)
            {
                return string.Empty;
            }
            if (string.Equals(tableName, AthenaConsts.MediaPlansTable, StringComparison.CurrentCultureIgnoreCase))
            {
                return $@"lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
            }
            else if (string.Equals(tableName, AthenaConsts.MediaBriefsTable, StringComparison.CurrentCultureIgnoreCase) || (isWithinDynamicFilter && string.Equals(tableName, AthenaConsts.CampaignTable, StringComparison.CurrentCultureIgnoreCase)))
            {
                return $@"lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
            }
            else if (isWithinDynamicFilter)
            {
                return $@"lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.OmniGuid}) IN {omniClientId} AND
                      lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
            }
            else
            {
                return string.Empty;
            }
        }

        public string GetOmniClientIdRestrictionsBasedOnTablePmds(string omniClientId, string tableName)
        {
            if (!omniAuthConfig.ClientIdCheck)
            {
                return string.Empty;
            }
            return $@"lower({tableName}.{AthenaConsts.OmniGuid}) IN {omniClientId}";
        }

        /// <summary>
        /// Check the GetOmniClientHierarchy for current client.
        /// </summary>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="cancellationToken">The cancellation token. Token</param>
        public async Task<string> GetOmniClientHierarchy(Guid omniClientId, CancellationToken cancellationToken)
        {
            var allClients = await portal.OmniClients.GetAllAsync(cancellationToken);
            var childClientsList = new List<Guid>()
            {
                omniClientId,
            };
            GetChild(omniClientId, allClients.ToList(), childClientsList);
            var inQuery = string.Join(",", childClientsList.Select(x => $"{SqlQuote(x.ToString().ToLowerInvariant())}"));
            return $"({inQuery})";
        }

        /// <summary>
        /// Check the GetChild for current client.
        /// </summary>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="allClients">The allClients list. Token</param>
        /// <param name="childClientsList">The childClientsList list. Token</param>
        protected void GetChild(Guid omniClientId, List<OmniClient> allClients, List<Guid> childClientsList)
        {
            var childClients = allClients.Where(x => x.ParentId == omniClientId && x.Id != omniClientId).ToList();
            foreach (var client in childClients)
            {
                childClientsList.Add(client.Id);
                GetChild(client.Id, allClients, childClientsList);
            }
        }
    }
}
