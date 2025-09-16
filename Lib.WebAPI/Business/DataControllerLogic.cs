using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Wordprocessing;
using Lib.Athena.Business;
using Lib.Athena.Business.Interfaces;
using Lib.Athena.Consts;
using Lib.Athena.Enumerations;
using Lib.Aurora.Business.Interfaces;
using Lib.Common.Business;
using Lib.Common.Business.Interfaces;
using Lib.Common.Models;
using Lib.WebAPI.Business.Interfaces;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Data;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OneOf.Types;
using Throw;

namespace Lib.WebAPI.Business
{
    /// <summary>
    /// DataControllerLogic
    /// </summary>
    public class DataControllerLogic : CommonDataControllerLogic
    {
        /// <summary>
        /// ColumnDetails
        /// </summary>
        public record ColumnDetails(string ColumnName, bool IsCurrency);

        /// <summary>
        /// The table join with planid
        /// </summary>
        public static readonly string TableJoinWithPlanId = $@"FROM {AthenaConsts.CampaignTable}
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON ({AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId})
            INNER JOIN {AthenaConsts.MediaPlansTable} ON
            (
                {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaBriefId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaBriefId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.PlanId}

            )";

        /// <summary>
        /// The table join
        /// </summary>
        public static readonly string TableDataDictionaryJoin = $@"FROM {AthenaConsts.CampaignTable}
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON 
            ({AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId})
       
            INNER JOIN {AthenaConsts.MediaPlansTable} ON
            (
                {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaBriefId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaBriefId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
                
            )";

        /// <summary>
        /// The table join
        /// </summary>
        public static readonly string TableJoin = $@"FROM {AthenaConsts.CampaignTable}
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON 
            ({AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId}
            AND  {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}
            AND  {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState}
            AND  {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState})
            INNER JOIN {AthenaConsts.MediaPlansTable} ON
            (
                {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaBriefId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaBriefId}
                AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
                AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.PlanId}
                AND  {AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.AmdState}
                AND  {AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.DataState}
            )";


        /// <summary>
        /// The table media brief join
        /// </summary>
        public static readonly string TableJoinMediBriefs = $@"FROM {AthenaConsts.CampaignTable}
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON
            (
            {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId} 
            AND  {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}
            )";

        /// <summary>
        /// The table media plan join
        /// </summary>
        public static readonly string TableJoinMediaPlans = $@"FROM {AthenaConsts.CampaignTable}           
            INNER JOIN {AthenaConsts.MediaPlansTable} ON
            (
                {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.PlanId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.AmdState}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.DataState}
            )";

        /// <summary>
        /// The table media plan join
        /// </summary>
        public static readonly string TableJoinMediaBriefs = $@"FROM {AthenaConsts.CampaignTable}           
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON
            (
                {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}
                AND {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState}
                AND  {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState}
            )";

        /// <summary>
        /// The table campaign join
        /// </summary>
        public static readonly string TableCampaignJoin = $@"FROM {AthenaConsts.CampaignTable}
            INNER JOIN {AthenaConsts.MediaBriefsTable} ON 
            ({AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId}
            AND {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}
            AND {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState}
            AND {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState})";

        /// <summary>
        /// The table campaign with media plans join
        /// </summary>
        public static readonly string TableCampaignMediaPlanJoin = $@"FROM {AthenaConsts.CampaignTable}
            INNER JOIN {AthenaConsts.MediaPlansTable} ON 
            ({AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
            AND {AthenaConsts.CampaignTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.PlanId}
            AND {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.AmdState}
            AND {AthenaConsts.CampaignTable}.{AthenaConsts.DataState}={AthenaConsts.MediaPlansTable}.{AthenaConsts.DataState})";

        /// <summary>
        /// The table media plan and media brief join
        /// </summary>
        public static readonly string TableMediaPlanAndMediaBriefJoin = $@"FROM {AthenaConsts.MediaBriefsTable}
            INNER JOIN {AthenaConsts.MediaPlansTable} ON
            (
                {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaBriefId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaBriefId}
                AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId}
                AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.PlanId}={AthenaConsts.MediaPlansTable}.{AthenaConsts.PlanId}
            )";

        /// <summary>
        /// Fields to map for metric and non currency columns
        /// </summary>
        public static readonly string MetricColumnsWithNonCurrency = "[{\"ColumnName\":\"actualctc\",\"IsCurrency\":true},{\"ColumnName\":\"actualctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"actualnetmedia\",\"IsCurrency\":true},{\"ColumnName\":\"actualnetmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"actuals\",\"IsCurrency\":true},{\"ColumnName\":\"actuals_local\",\"IsCurrency\":true},{\"ColumnName\":\"addedvalue\",\"IsCurrency\":true},{\"ColumnName\":\"addedvalue_local\",\"IsCurrency\":true},{\"ColumnName\":\"admissions\",\"IsCurrency\":true},{\"ColumnName\":\"adserving\",\"IsCurrency\":true},{\"ColumnName\":\"adserving_local\",\"IsCurrency\":true},{\"ColumnName\":\"adverificationcost\",\"IsCurrency\":true},{\"ColumnName\":\"adverificationcost_local\",\"IsCurrency\":true},{\"ColumnName\":\"asbof\",\"IsCurrency\":true},{\"ColumnName\":\"asbof_local\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"briefedctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"commission\",\"IsCurrency\":true},{\"ColumnName\":\"commission_local\",\"IsCurrency\":true},{\"ColumnName\":\"dst\",\"IsCurrency\":true},{\"ColumnName\":\"dst_local\",\"IsCurrency\":true},{\"ColumnName\":\"eqgrps\",\"IsCurrency\":false},{\"ColumnName\":\"footfalls\",\"IsCurrency\":false},{\"ColumnName\":\"grossmedia\",\"IsCurrency\":true},{\"ColumnName\":\"grossmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"grps\",\"IsCurrency\":false},{\"ColumnName\":\"impressions\",\"IsCurrency\":false},{\"ColumnName\":\"installation\",\"IsCurrency\":true},{\"ColumnName\":\"installation_local\",\"IsCurrency\":true},{\"ColumnName\":\"localtax1\",\"IsCurrency\":true},{\"ColumnName\":\"localtax1_local\",\"IsCurrency\":true},{\"ColumnName\":\"localtax2\",\"IsCurrency\":true},{\"ColumnName\":\"localtax2_local\",\"IsCurrency\":true},{\"ColumnName\":\"medialevycost\",\"IsCurrency\":true},{\"ColumnName\":\"medialevycost_local\",\"IsCurrency\":true},{\"ColumnName\":\"mediaops\",\"IsCurrency\":true},{\"ColumnName\":\"mediaops_local\",\"IsCurrency\":true},{\"ColumnName\":\"netmedia\",\"IsCurrency\":true},{\"ColumnName\":\"netmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"othercost\",\"IsCurrency\":true},{\"ColumnName\":\"othercost_local\",\"IsCurrency\":true},{\"ColumnName\":\"plannedctc\",\"IsCurrency\":true},{\"ColumnName\":\"plannedctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"plannednetmedia\",\"IsCurrency\":true},{\"ColumnName\":\"plannednetmedia_local\",\"IsCurrency\":true},{\"ColumnName\":\"population\",\"IsCurrency\":false},{\"ColumnName\":\"pretaxtotal\",\"IsCurrency\":true},{\"ColumnName\":\"pretaxtotal_local\",\"IsCurrency\":true},{\"ColumnName\":\"production\",\"IsCurrency\":true},{\"ColumnName\":\"production_local\",\"IsCurrency\":true},{\"ColumnName\":\"screens\",\"IsCurrency\":false},{\"ColumnName\":\"spots\",\"IsCurrency\":false},{\"ColumnName\":\"tacticalctc\",\"IsCurrency\":true},{\"ColumnName\":\"tacticalctc_local\",\"IsCurrency\":true},{\"ColumnName\":\"totalclient\",\"IsCurrency\":true},{\"ColumnName\":\"totalclient_local\",\"IsCurrency\":true},{\"ColumnName\":\"totalfees\",\"IsCurrency\":true},{\"ColumnName\":\"totalfees_local\",\"IsCurrency\":true},{\"ColumnName\":\"totalnet\",\"IsCurrency\":true},{\"ColumnName\":\"totalnet_local\",\"IsCurrency\":true},{\"ColumnName\":\"totaltax\",\"IsCurrency\":true},{\"ColumnName\":\"totaltax_local\",\"IsCurrency\":true},{\"ColumnName\":\"unitrate\",\"IsCurrency\":true},{\"ColumnName\":\"unitrate_local\",\"IsCurrency\":true},{\"ColumnName\":\"units\",\"IsCurrency\":false},{\"ColumnName\":\"vat\",\"IsCurrency\":true},{\"ColumnName\":\"vat_local\",\"IsCurrency\":true}]";

        /// <summary>
        /// MetricCurrencyColumns
        /// </summary>
#pragma warning disable CS8604 // Possible null reference argument.
        public static readonly string[] MetricCurrencyColumns = Json.Deserialize<Collection<ColumnDetails>>(MetricColumnsWithNonCurrency).Where(x => x.IsCurrency).Select(x => x.ColumnName).ToArray();
#pragma warning restore CS8604 // Possible null reference argument.
        /// <summary>
        /// Media Brief
        /// </summary>
        public static readonly string TableMediaBriefJoin = $@"FROM {AthenaConsts.MediaBriefsTable}";

        /// <summary>
        /// Media Plan
        /// </summary>
        public static readonly string TableMediaPlanJoin = $@"FROM {AthenaConsts.MediaPlansTable}";

        /// <summary>
        /// DateTime with timestamp fields
        /// </summary>
        public static readonly string[] TimeStampWithTimeZoneColumns = new string[]
        {
            "amd_timestamp",
            "campaignstart",
            "campaignend",
            "file_date",
            "mygrid_flightstartdate",
            "mygrid_flightenddate",
            "effective_date",
            "approvaldeadline",
            "flightstart",
            "flightend",
            "week_text",
            "bookingdeadline",
            "materialclose",
            "issuedate",
            "onsaledate",
        };
        private const string EffectiveDateKey = $"{AthenaConsts.MediaPlansTable}_{AthenaConsts.MediaPlansEffectiveDate}";
        private const string FlightEndKey = $"{AthenaConsts.MediaPlansTable}_{AthenaConsts.MediaPlansFlightEnd}";
        private const string FlightStartKey = $"{AthenaConsts.MediaPlansTable}_{AthenaConsts.MediaPlansFlightStart}";
        private const string GrandTotalsPrefix = "gt";
        private const string HeaderPrefix = "header";
        private const string LevelInflightOverlayPrefix = "lvlifo";
        private const string LevelMetricPrefix = "lvlmtc";
        private const string LevelSubTotalPrefix = "lvlsbt";
        private const string MediaBriefIdKey = $"{AthenaConsts.MediaBriefsTable}_{AthenaConsts.MediaBriefId}";
        private const string MediaHierarchyLevelPrefix = "lvl";
        private const string MediaHierarchySubLevelPrefix = "sblvl";
        private const string SubLevelInflightOverlayPrefix = "sblvlifo";
        private const string SubLevelMetricPrefix = "sblvlmtc";
        private const string SubLevelSubTotalPrefix = "sblvlsbt";
        private const string SystemCampaignIdKey = $"{AthenaConsts.CampaignTable}_{AthenaConsts.SystemCampaignId}";
        private const string TotalsPrefix = "total";
        private const string CampaignLocalCurrencyKey = $"{AthenaConsts.CampaignTable}_{AthenaConsts.CampaignLocalCurrencyName}";
        private const string LegendPrefix = "legend";
        private const string DateFormat = "MM/dd";
        private const string CustomFlightRange = "CustomFlightRange";
        private const string LevelSubTotalSummaryPrefix = "lvlsbtsmry";

        private static readonly string CampaignConstraints = $@"
            {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId} IS NOT NULL AND
            lower({AthenaConsts.CampaignTable}.{AthenaConsts.DataState}) = 'submitted' AND
            {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState} = 'active'

            ";

        private static readonly string CampaignConstraintsWithSource = $@"
            {AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId} IS NOT NULL AND
            lower({AthenaConsts.CampaignTable}.{AthenaConsts.DataState}) = 'submitted' AND
            lower({AthenaConsts.CampaignTable}.{AthenaConsts.Source}) <> 'mediatools' AND
            {AthenaConsts.CampaignTable}.{AthenaConsts.AmdState} = 'active'

            ";

        private static readonly string DefaultColumns = $@"{AthenaConsts.CampaignTable}.{AthenaConsts.SystemCampaignId} AS {SystemCampaignIdKey}";

        private static readonly string MediaBriefsConstraints = $@"
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId} IS NOT NULL AND
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.Channel} IS NOT NULL AND
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.SubChannel} IS NOT NULL AND
            COALESCE({AthenaConsts.MediaBriefsTable}.{AthenaConsts.BuyMethod}, 'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaBriefsTable}.{AthenaConsts.BriefDetail}, 'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaBriefsTable}.{AthenaConsts.MixTypeID}, 0) <> 1 AND
             lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState}) = 'submitted' AND
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState} = 'active'
            ";

        private static readonly string MediaBriefsConstraintsWithSource = $@"
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.SystemCampaignId} IS NOT NULL AND
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.Channel} IS NOT NULL AND
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.SubChannel} IS NOT NULL AND
            COALESCE({AthenaConsts.MediaBriefsTable}.{AthenaConsts.BuyMethod}, 'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaBriefsTable}.{AthenaConsts.BriefDetail}, 'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaBriefsTable}.{AthenaConsts.MixTypeID}, 0) <> 1 AND
             lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.DataState}) = 'submitted' AND
             lower({AthenaConsts.MediaBriefsTable}.{AthenaConsts.Source}) <> 'mediatools' AND
            {AthenaConsts.MediaBriefsTable}.{AthenaConsts.AmdState} = 'active'
            ";

        private static readonly string MediaPlansConstraints = $@"
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId} IS NOT NULL AND
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.Channel} IS NOT NULL AND
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.SubChannel} IS NOT NULL AND
            COALESCE({AthenaConsts.MediaPlansTable}.{AthenaConsts.BuyMethod},'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaPlansTable}.{AthenaConsts.BriefDetail},'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaPlansTable}.{AthenaConsts.MixTypeID}, 0) <> 1 AND
             lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.DataState}) = 'submitted' AND
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.AmdState} = 'active'
            ";

        private static readonly string MediaPlansConstraintsWithSource = $@"
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.SystemCampaignId} IS NOT NULL AND
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.Channel} IS NOT NULL AND
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.SubChannel} IS NOT NULL AND
            COALESCE({AthenaConsts.MediaPlansTable}.{AthenaConsts.BuyMethod},'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaPlansTable}.{AthenaConsts.BriefDetail},'N/A') IS NOT NULL AND
            COALESCE({AthenaConsts.MediaPlansTable}.{AthenaConsts.MixTypeID}, 0) <> 1 AND
             lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.DataState}) = 'submitted' AND
             lower({AthenaConsts.MediaPlansTable}.{AthenaConsts.Source}) <> 'mediatools' AND
            {AthenaConsts.MediaPlansTable}.{AthenaConsts.AmdState} = 'active'
            ";

        private static readonly Guid DefaultMetricTableId = Guid.Parse("f0867476-5d91-4fbf-8ebf-c5308309f4ea");
        private static readonly string DefaultMetricColumnName = "netmedia";


        private readonly ICacheLogic cache;
        private readonly AthenaDataConverter dataConverter;
        private readonly ILog<DataControllerLogic> log;
        private readonly IMapper mapper;

        private readonly List<string> MediaPlansColumns = new()
        {
            $"LEAST({AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansFlightEnd},{AthenaConsts.CampaignTable}.{AthenaConsts.CampaignEnd}) AS {FlightEndKey}",
            $"GREATEST({AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansFlightStart},{AthenaConsts.CampaignTable}.{AthenaConsts.CampaignStart}) AS {FlightStartKey}",
            $"{AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} AS {EffectiveDateKey}",
            $"{AthenaConsts.CampaignTable}.{AthenaConsts.CampaignLocalCurrencyName} AS {CampaignLocalCurrencyKey}",
        };

        private readonly List<string> MediaBriefsColumns = new()
        {
            $"LEAST({AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansFlightEnd},{AthenaConsts.CampaignTable}.{AthenaConsts.CampaignEnd}) AS {FlightEndKey}",
            $"GREATEST({AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansFlightStart},{AthenaConsts.CampaignTable}.{AthenaConsts.CampaignStart})   AS {FlightStartKey}",
            $"{AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} AS {EffectiveDateKey}",
            $"{AthenaConsts.CampaignTable}.{AthenaConsts.CampaignLocalCurrencyName} AS {CampaignLocalCurrencyKey}",
        };

        // private readonly List<string> MediaPlansColumnsForAthena = new()
        // {
        //    $"date_format({AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansFlightEnd},'%Y-%m-%dT%H:%m:%sZ') AS {FlightEndKey}",
        //    $"date_format({AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansFlightStart},'%Y-%m-%dT%H:%m:%sZ')  AS {FlightStartKey}",
        //    $"date_format({AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate},'%Y-%m-%dT%H:%m:%sZ') AS {EffectiveDateKey}",
        // };

        private readonly OmniAuthConfig omniAuthConfig;
        private readonly IPortalUnitOfWork portal;
        private readonly IAuroraQueryLogic queryLogic;
        private readonly SecurityLogic securityLogic;
        private readonly IConfiguration configuration;
        private readonly IPortalDbContext portalDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataControllerLogic" /> class.
        /// </summary>
        /// <param name="portal">The portal.</param>
        /// <param name="queryLogic">The query logic.</param>
        /// <param name="cache">The cache.</param>
        /// <param name="log">The log.</param>
        /// <param name="omniAuthConfig">The omni authentication configuration.</param>
        /// <param name="securityLogic">The security logic.</param>
        /// <param name="mapper">The mapper.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="configuration">The configuration.</param>
        /// <param name="portalDbContext">The portal database context.</param>"
        public DataControllerLogic(
                IPortalUnitOfWork portal,
                IAuroraQueryLogic queryLogic,
                ICacheLogic cache,
                ILog<DataControllerLogic> log,
                OmniAuthConfig omniAuthConfig,
                SecurityLogic securityLogic,
                IMapper mapper,
                AthenaDataConverter dataConverter,
                IConfiguration configuration,
                IPortalDbContext portalDbContext)
                : base(portal, omniAuthConfig)
        {
            this.portal = portal;
            this.queryLogic = queryLogic;
            this.cache = cache;
            this.log = log;
            this.omniAuthConfig = omniAuthConfig;
            this.securityLogic = securityLogic;
            this.mapper = mapper;
            this.dataConverter = dataConverter;
            this.configuration = configuration;
            this.portalDbContext = portalDbContext;
        }

        /// <summary>
        /// Gets the broadcast calendar asynchronous.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetBroadcastCalendarAsync(long year, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting broadcast calendar for user {userId}.");

            var calendar = (await portal.BroadcastCalendars.GetAsync(cancellationToken, predicate: x => x.Year == year)).FirstOrDefault()
                ?? throw new KeyNotFoundException($"Year {year} not found.");

            return Json.Deserialize<IEnumerable<IEnumerable<CalendarItem>>>(calendar.BroadcastCalendarItems)
                ?? throw new Exception($"Deserialisation failed.");
        }

        /// <summary>
        /// Gets the broadcast calendar asynchronous.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetBroadcastCalendarAsync(DateTime startDate, DateTime endDate, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting broadcast calendar for user {userId}.");

            (endDate > startDate).Throw().IfFalse();

            // find the years touched by the span of startDate and endDate
            var years = Enumerable.Range(startDate.Year, endDate.Year - startDate.Year + 1).DefaultIfEmpty(startDate.Year)
                .Select(x => Convert.ToInt64(x)).ToList();

            // get the annual broadcast calendars for the years identified
            var calendars = await portal.BroadcastCalendars.GetAsync(cancellationToken, predicate: x => years.Contains(x.Year));

            // get all the calendar items from those years
            var items = calendars.Select(x => Json.Deserialize<IEnumerable<IEnumerable<CalendarItem>>>(x.BroadcastCalendarItems));

            // filter out the broadcast weeks ending before the start date and starting
            // after the end date
            var result = items.SelectMany(x => x!).Select(x =>
            {
                // get first day of broadcast week
                var start = Json.Deserialize<DateTime>(x.First(c => c.ColumnName == AthenaConsts.RollPeriodFirstDay).JsonValue);
                // get last day of broadcast week
                var end = Json.Deserialize<DateTime>(x.First(c => c.ColumnName == AthenaConsts.RollPeriodLastDay).JsonValue);

                // ignore weeks before selected range: bw-end < startDate
                if (end < startDate)
                {
                    return null;
                }

                // week 1: bw-start <= startDate && bw-end >= startDate
                if (start <= startDate && end >= startDate)
                {
                    log.Add(LogLevel.Debug, $"Identified first broadcast week matching selection from {start}-{end}");
                    return x;
                }

                // week 1-n: bw-start >= startDate && bw-start <= endDate
                if (start >= startDate && end < endDate)
                {
                    log.Add(LogLevel.Debug, $"Identified broadcast week in range matching selection from {start}-{end}");
                    return x;
                }

                // week n: bw-start <= endDate && bw-end >=endDate
                if (start <= endDate && end >= endDate)
                {
                    log.Add(LogLevel.Debug, $"Identified last broadcast week in range matching selection from {start}-{end}");
                    return x;
                }

                // ignore weeks after selected range: bw-start > endDate
                if (start > endDate)
                {
                    return null;
                }

                log.Add(LogLevel.Debug, $"Dropping broadcast week starting {start}");
                return null;
            }).Where(x => x != null).ToList();

            return result!;
        }

        /// <summary>
        /// Gets the client calendar asynchronous.
        /// </summary>
        /// <param name="year">The year.</param>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetClientCalendarAsync(string year, Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting client calendar for user {userId}.");

            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);

            var clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);

            var calendar = (await portal.ClientCalendars.GetAsync(cancellationToken, count: 1, predicate: x => x.ClientYear == year && x.ClientId == clientId)).FirstOrDefault()
                ?? throw new KeyNotFoundException($"No client calendar found for client {clientId} and year {year}.");

            return Json.Deserialize<IEnumerable<IEnumerable<CalendarItem>>>(calendar.ClientCalendarItems)
                ?? throw new Exception($"Deserialisation failed.");
        }

        /// <summary>
        /// Gets the client calendar asynchronous.
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="omniClientId">The omni client identifier.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<IEnumerable<IEnumerable<CalendarItem>>> GetClientCalendarAsync(DateTime startDate, DateTime endDate, Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting client calendar for user {userId}.");

            await securityLogic.CheckUserHasClientAccessAsync(omniClientId, userId, cancellationToken);

            var clientId = await securityLogic.GetClientIdByGuidAsync(omniClientId, cancellationToken);

            (endDate > startDate).Throw().IfFalse();

            var years = Enumerable.Range(startDate.Year, (endDate.Year - startDate.Year) + 1).DefaultIfEmpty(startDate.Year)
                .Select(x => Convert.ToInt64(x).ToString()).ToList();

            var calendars = await portal.ClientCalendars.GetAsync(cancellationToken, predicate: x => x.ClientId == clientId && years.Contains(x.ClientYear));

            var items = calendars.Select(x => Json.Deserialize<IEnumerable<IEnumerable<CalendarItem>>>(x.ClientCalendarItems));

            var result = items.SelectMany(x => x!).Select(x =>
            {
                var date = Json.Deserialize<DateTime>(x.First(c => c.ColumnName == AthenaConsts.ClientRollPeriodClientWeekDayDate).JsonValue);

                if (date >= startDate && date <= endDate)
                {
                    return x;
                }

                return null;
            }).Where(x => x != null).ToList();

            return result!;
        }

        /// <summary>
        /// Gets the data asynchronous.
        /// </summary>
        /// <param name="dataRequest">The data request.</param>
        /// <param name="includeSourceMediaToolsData">if set to <c>true</c> [include source media tools data].</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<FlowchartData> GetDataAsync(
            DataRequestDTO dataRequest, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting data for user {userId}.");

            dataRequest.FlowchartDefinition.ThrowIfNull();
            dataRequest.FlowchartDefinition.CalendarDefinition.ThrowIfNull();
            dataRequest.FlowchartDefinition.CalendarDefinition.Definition.ThrowIfNull();
            dataRequest.FlowchartDefinition.CalendarDefinition.Definition.Configuration.ThrowIfNull();

            dataRequest.FlowchartDefinition.MediaHierarchyDefinition.ThrowIfNull();
            dataRequest.FlowchartDefinition.MediaHierarchyDefinition.Definition.ThrowIfNull();
            dataRequest.FlowchartDefinition.MediaHierarchyDefinition.Definition.Levels.ThrowIfNull().IfEmpty();

            var dictionaryTables = new List<DataDictionaryTableDetailsDTO>();

            var (startDate, endDate) = await GetDatesAsync(dataRequest.FlowchartDefinition.CalendarDefinition.Definition, dataRequest.OmniClientId, userId, cancellationToken);

            bool showBriefedCTC = dataRequest.ShowBriefedCTC;
            var join = await GetJoinAsync(startDate, endDate, dataRequest, dictionaryTables, includeSourceMediaToolsData, cancellationToken, showBriefedCTC);

            var isSplitQuery = join.Contains($" {AthenaConsts.MediaBriefsTable} ") && join.Contains($" {AthenaConsts.MediaPlansTable} ");

            var result = await cache.GetAsync(
                $"FlowchartData_{join.GetHashCode()}", // maybe something better
                x => queryLogic.QueryAsync(join, x, isSplitQuery), cancellationToken);

            var data = dataConverter.Convert(result);

            return GetFlowchartData(dataRequest.FlowchartDefinition, data);
        }

        private async Task<ICollection<Dictionary<string, object?>>> GetData(DistinctDataDTO distinctData, bool includeSourceMediaToolsData, Guid userId, bool isPreviousLevelDataRequired, CancellationToken cancellationToken)
        {
            var table = mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByIdAsync(distinctData.TableId, cancellationToken));

            var parentTable = distinctData.ParentTableId != null ? mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByIdAsync((Guid)distinctData.ParentTableId, cancellationToken)) : null;

            var column = table.Columns.FirstOrDefault(x => x.Name == distinctData.ColumnName)
                         ?? throw new KeyNotFoundException($"Column {distinctData.ColumnName} not found in table {table.Name}.");

            var tableToJoin = string.Equals(table.Name, AthenaConsts.MediaBriefsTable, StringComparison.CurrentCultureIgnoreCase) ? TableMediaBriefJoin : string.Equals(table.Name, AthenaConsts.MediaPlansTable, StringComparison.CurrentCultureIgnoreCase) ? TableMediaPlanJoin : TableCampaignJoin;

            // Join different table if parent and sublevel tables are different
            string? tableWhere = null;
            if (parentTable != null && parentTable.Name != table.Name)
            {
                switch (parentTable.Name, table.Name)
                {
                    case (AthenaConsts.CampaignTable, AthenaConsts.MediaBriefsTable):
                        tableToJoin = TableCampaignJoin;
                        break;
                    case (AthenaConsts.CampaignTable, AthenaConsts.MediaPlansTable):
                        tableToJoin = TableCampaignMediaPlanJoin;
                        break;
                    case (AthenaConsts.MediaBriefsTable, AthenaConsts.MediaPlansTable):
                        tableToJoin = TableMediaPlanAndMediaBriefJoin;
                        break;
                    case (AthenaConsts.MediaPlansTable, AthenaConsts.CampaignTable):
                        tableToJoin = TableCampaignMediaPlanJoin;
                        tableWhere = AthenaConsts.MediaPlansTable;
                        break;
                    case (AthenaConsts.MediaPlansTable, AthenaConsts.MediaBriefsTable):
                        tableToJoin = TableMediaPlanAndMediaBriefJoin;
                        break;
                    default:
                        break;
                }
            }

            var levelTables = distinctData.Levels?.Select(x => mapper.Map<DataDictionaryTableDetailsDTO>(portal.DataDictionaryTables.GetByIdAsync(x.TableId, cancellationToken).Result)).Select(c => new { c.Id, c.Name }).Distinct().ToList();
            var isSameAsCurrent = (levelTables?.Count == 1 && levelTables.Exists(c => c.Id == table.Id)) || levelTables?.Count(c => tableToJoin.Contains(c.Name)) >= levelTables?.Count;
            if (!isSameAsCurrent)
            {
                var nonJoinedTables = levelTables?.Where(c => !tableToJoin.Contains(c.Name)).Select(c => c.Name).ToList();
                if (nonJoinedTables?.Count > 0)
                {
                    nonJoinedTables.ForEach(c => { tableToJoin += $" INNER JOIN {c} ON {table.Name}.{AthenaConsts.SystemCampaignId}={c}.{AthenaConsts.SystemCampaignId} AND {table.Name}.{AthenaConsts.PlanId}={c}.{AthenaConsts.PlanId} AND {table.Name}.{AthenaConsts.AmdState}={c}.{AthenaConsts.AmdState} AND {table.Name}.{AthenaConsts.DataState}={c}.{AthenaConsts.DataState}"; });
                }
            }
            var previousLevelDetails = distinctData.Levels?.LastOrDefault();
            var query = $"SELECT DISTINCT {table.Name}.{column.Name} AS column1 {tableToJoin}";

            if (previousLevelDetails != null && !string.IsNullOrWhiteSpace(previousLevelDetails.ColumnName) && isPreviousLevelDataRequired)
            {
                var levelTable = mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByIdAsync(previousLevelDetails.TableId, cancellationToken));
                var levelColumn = levelTable.Columns.FirstOrDefault(x => x.Name == previousLevelDetails.ColumnName)
                            ?? throw new KeyNotFoundException($"Column {previousLevelDetails.ColumnName} not found in table {levelTable.Name}.");
                query = $"SELECT DISTINCT {table.Name}.{column.Name} AS {column.Name},{levelTable.Name}.{levelColumn.Name} AS column2  {tableToJoin}";
            }

            var omniClients = await GetOmniClientHierarchy(distinctData.OmniClientId, cancellationToken);
            var clientDetails = await portal.OmniClients.GetByIdAsync(distinctData.OmniClientId, cancellationToken);
            var where = GetOmniClientIdRestrictionsBasedOnTable(omniClients, tableWhere ?? table.Name, true);
            where += $" AND  lower({table.Name}.{AthenaConsts.DataState}) = 'submitted' AND {table.Name}.{AthenaConsts.AmdState} = 'active'";

            if (string.Equals(table.Name, AthenaConsts.CampaignTable) && !string.IsNullOrWhiteSpace(clientDetails?.Client?.Name))
            {
                where += $" AND {table.Name}.client_name={SqlQuote(clientDetails?.Client?.Name)}";
            }
            where += !includeSourceMediaToolsData ? $" AND  lower({table.Name}.{AthenaConsts.Source}) <> 'mediatools'" : string.Empty;

            if (distinctData.StartDate != null && distinctData.EndDate != null)
            {
                if (query.Contains(TableMediaBriefJoin) || query.Contains($"INNER JOIN {AthenaConsts.MediaBriefsTable}"))
                {
                    where += $" AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(distinctData.StartDate?.ToString("yyyy-MM-dd"))})";
                    where += $" AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(distinctData.EndDate?.ToString("yyyy-MM-dd"))})";
                }
                if (query.Contains(TableMediaPlanJoin) || query.Contains($"INNER JOIN {AthenaConsts.MediaPlansTable}"))
                {
                    where += $" AND {AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(distinctData.StartDate?.ToString("yyyy-MM-dd"))})";
                    where += $" AND {AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(distinctData.EndDate?.ToString("yyyy-MM-dd"))})";
                }
            }

            if (!string.IsNullOrWhiteSpace(where))
            {
                query += $" WHERE {where}";
            }

            if (parentTable != null)
            {
                var parentColumn = parentTable.Columns.FirstOrDefault(x => x.Name == distinctData.ParentColumnName)
                            ?? throw new KeyNotFoundException($"Column {distinctData.ParentColumnName} not found in table {parentTable.Name}.");
                var subeLeveleWhere = GetSubLeveleRestriction(parentTable.Name, parentColumn.Name, distinctData.ParentSelectedValue);

                if (!string.IsNullOrWhiteSpace(subeLeveleWhere))
                {
                    if (!string.IsNullOrWhiteSpace(where))
                    {
                        query += $" AND  {subeLeveleWhere}";
                    }
                    else
                    {
                        query += $" Where  {subeLeveleWhere}";
                    }
                }
                query += !includeSourceMediaToolsData ? $" AND  lower({parentTable.Name}.{AthenaConsts.Source}) <> 'mediatools'" : string.Empty;
                if (string.Equals(parentTable.Name, AthenaConsts.CampaignTable) && !string.IsNullOrWhiteSpace(clientDetails?.Client?.Name))
                {
                    query += $" AND {parentTable.Name}.client_name={SqlQuote(clientDetails?.Client?.Name)}";
                }
            }

            if (distinctData.Levels?.Count() > 0)
            {
                foreach (var level in distinctData.Levels?.ToList())
                {
                    var levelTable = mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByIdAsync(level.TableId, cancellationToken));
                    var levelColumn = levelTable.Columns.FirstOrDefault(x => x.Name == level.ColumnName)
                                ?? throw new KeyNotFoundException($"Column {level.ColumnName} not found in table {levelTable.Name}.");
                    query += $" AND {levelTable.Name}.{levelColumn.Name} IN ({string.Join(",", level.SelectedValues.Select(c => SqlQuote(c)))})";
                    query += $" AND  lower({levelTable.Name}.{AthenaConsts.DataState}) = 'submitted' AND {levelTable.Name}.{AthenaConsts.AmdState} = 'active'";

                    query += !includeSourceMediaToolsData ? $" AND  lower({levelTable.Name}.{AthenaConsts.Source}) <> 'mediatools'" : string.Empty;
                    if (string.Equals(levelTable.Name, AthenaConsts.CampaignTable) && !string.IsNullOrWhiteSpace(clientDetails?.Client?.Name))
                    {
                        query += $" AND {levelTable.Name}.client_name={SqlQuote(clientDetails?.Client?.Name)}";
                    }
                    var omniGuidQuery = GetOmniClientIdRestrictionsBasedOnTable(omniClients, levelTable.Name, false);
                    if (!string.IsNullOrWhiteSpace(omniGuidQuery))
                    {
                        query += $" AND {omniGuidQuery}";
                    }
                }
            }

            var result = await cache.GetAsync(
                $"DistinctData_{distinctData.TableId}_{distinctData.ColumnName}_{query.GetHashCode()}",
                x => queryLogic.QueryAsync(query, x),
                cancellationToken);

            var data = dataConverter.Convert(result);
            return data;
        }

        /// <summary>
        /// Gets the distinct data asynchronous.
        /// </summary>
        /// <param name="distinctData">The distinct data.</param>
        /// <param name="includeSourceMediaToolsData">if set to <c>true</c> [include source media tools data].</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ICollection<object?>> GetDistinctDataAsync(DistinctDataDTO distinctData, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting distinct data for user {userId}.");

            var data = await GetData(distinctData, includeSourceMediaToolsData, userId, false, cancellationToken);

            return data.SelectMany(x => x.Values).Where(x => x != null && !string.IsNullOrWhiteSpace(x.ToString())).ToList();
        }


        /// <summary>
        /// Gets the previous level distinct data asynchronous.
        /// </summary>
        /// <param name="distinctData">The previous level distinct data.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<ICollection<DataDTO>> GetPreviousLevelDistinctDataAsync(DistinctDataDTO distinctData, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting previous level distinct data for user {userId}.");

            if (distinctData == null || distinctData.Levels?.Count() == 0)
            {
                return [];
            }
            var data = await GetData(distinctData, includeSourceMediaToolsData, userId, true, cancellationToken);

            return data.Select(x => new DataDTO() { Column1 = Convert.ToString(x.Values.ToList()[0]), Column2 = Convert.ToString(x.Values.ToList()[1]) }).Where(c => !string.IsNullOrWhiteSpace(c.Column1) && !string.IsNullOrWhiteSpace(c.Column2)).ToList();
        }

        /// <summary>
        /// Gets the header data asynchronous.
        /// </summary>
        /// <param name="headerDataRequest">The header data request.</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public async Task<HeaderData> GetHeaderDataAsync(HeaderDataRequestDTO headerDataRequest, bool includeSourceMediaToolsData, Guid userId, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Getting header data for user {userId}.");

            headerDataRequest.FlowchartDefinition.ThrowIfNull();
            headerDataRequest.FlowchartDefinition.HeaderDefinition.ThrowIfNull();
            headerDataRequest.FlowchartDefinition.MediaHierarchyDefinition.ThrowIfNull();
            headerDataRequest.FlowchartDefinition.CalendarDefinition.ThrowIfNull();

            var dictionaryTables = new List<DataDictionaryTableDetailsDTO>();

            var (_, restrictions) = await GetSelectedColumnsAsync(headerDataRequest.FlowchartDefinition, dictionaryTables, cancellationToken, Enum.Parse<Currency>(Currency.LLL.ToString()));
            var (startDate, endDate) = await GetDatesAsync(headerDataRequest.FlowchartDefinition.CalendarDefinition.Definition, headerDataRequest.OmniClientId, userId, cancellationToken);
            var omniClients = await GetOmniClientHierarchy(headerDataRequest.OmniClientId, cancellationToken);
            var omniRestrictions = GetOmniClientIdRestrictions(omniClients);

            var results = new HeaderData { Rows = new Collection<HeaderDataRow>() };

            foreach (var row in headerDataRequest.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Rows.Where(x => x.Type == HeaderRowType.Value))
            {
                await AddHeaderDataRowAsync(headerDataRequest.OmniClientId, includeSourceMediaToolsData, results.Rows, restrictions, headerDataRequest.RunRestrictions, omniRestrictions,
                                            row.TableId!.Value, row.ColumnName!, row.Order, startDate, endDate,
                                            dictionaryTables, cancellationToken);
            }

            if (headerDataRequest.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Details != null)
            {
                results.Details = new HeaderDataDetails { Rows = new Collection<HeaderDataRow>() };

                foreach (var row in headerDataRequest.FlowchartDefinition.HeaderDefinition.Definition.Configuration.Details!.Rows)
                {
                    await AddHeaderDataRowAsync(headerDataRequest.OmniClientId, includeSourceMediaToolsData, results.Details.Rows, restrictions, headerDataRequest.RunRestrictions, omniRestrictions,
                                                    row.TableId!, row.ColumnName!, row.Order, startDate, endDate,
                                                    dictionaryTables, cancellationToken);
                }
            }

            return results;
        }

        /// <summary>
        /// Gets the summary data asynchronous.
        /// </summary>
        /// <param name="summaryDataRequest">The summary data request.</param>
        /// <param name="userId">The user identifier.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        public Task<SummaryData> GetSummaryDataAsync(
            SummaryDataRequestDTO summaryDataRequest, Guid userId, CancellationToken cancellationToken)
        {
            // TODO
            throw new NotImplementedException("Coming soon.");
        }

        private void AddGrandTotals(
            FlowchartData result,
            GrandTotalDefinition grandTotalDefinition,
            List<Dictionary<string, object?>> data)
        {
            grandTotalDefinition.Selections.ThrowIfNull().IfEmpty();

            result.GrandTotals = new Collection<GrandTotalData>();

            foreach (var total in grandTotalDefinition.Selections)
            {
                int[] orders = { total.Order };

                result.GrandTotals.Add(new GrandTotalData
                {
                    FlightRange = total.FlightRange,
                    ColumnName = total.ColumnName,
                    TableId = total.TableId,
                    Order = total.Order,
                    Metrics = GetMetrics(total.ColumnName, GrandTotalsPrefix, orders, null, string.Empty, data),
                    LocalCurrency = data.GroupBy(x => x[CampaignLocalCurrencyKey]).Select(x => x.Key).FirstOrDefault()?.ToString(),
                });
            }
        }

        private async Task AddHeaderDataRowAsync(
            Guid omniClientId,
            bool includeSourceMediaToolsData,
            ICollection<HeaderDataRow> rows,
            List<string> restrictions,
            ICollection<RunRestriction> runRestrictions,
            string omniRestrictions,
            Guid tableId,
            string columnName,
            int order,
            DateTime startDate,
            DateTime endDate,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            CancellationToken cancellationToken)
        {
            var where = string.Empty;
            //bool isLocalCurrency = currency == Currency.LLL;
            //string trimEndForLocal = isLocalCurrency ? "_local" : " ";
            var (table, column) = await GetTableColumnAsync(tableId, columnName, dictionaryTables, cancellationToken);

            var selectedColumns = $"DISTINCT {table.Name}.{column.Name} AS {HeaderPrefix}_{column.Name}";

            var query = $"SELECT {selectedColumns} {TableJoin}";

            where += $"{AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})";
            where += $" AND {AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})";

            where += !includeSourceMediaToolsData ? $" AND {CampaignConstraintsWithSource} AND {MediaBriefsConstraintsWithSource} AND {MediaPlansConstraintsWithSource}" : $" AND {CampaignConstraints} AND {MediaBriefsConstraints} AND {MediaPlansConstraints}";

            where += $" AND {string.Join(" AND ", restrictions)}";

            var runWhere = await GetRunRestrictionsAsync(runRestrictions, dictionaryTables, cancellationToken);

            if (!string.IsNullOrWhiteSpace(runWhere))
            {
                where += $" AND ({runWhere})";
            }

            if (!string.IsNullOrWhiteSpace(omniRestrictions))
            {
                where += $" AND {omniRestrictions}";
            }

            query += $" WHERE {where}";

            var result = await cache.GetAsync(
                $"HeaderData_{table.Id}_{column.Name}_{query.GetHashCode()}",
                x => queryLogic.QueryAsync(query, x),
                cancellationToken);

            var data = dataConverter.Convert(result);

            rows.Add(new HeaderDataRow
            {
                ColumnName = column.Name,
                TableId = table.Id,
                Order = order,
                ValuesJson = data.Select(x => x[$"{HeaderPrefix}_{column.Name}"]).Where(x => x != null).Select(x => Json.Serialize(x)).ToList(),
            });
        }

        private void AddLevel(
            int levelIndex,
            List<int> levelOrder,
            ICollection<FlowchartDataLevel> flowchartDataLevels,
            ICollection<MediaHierarchyLevel> mediaHierarchyLevels,
            TotalsDefinition? totalsDefinition,
            ICollection<Dictionary<string, object?>> bigdata)
        {
            var order = levelOrder[levelIndex];
            var level = mediaHierarchyLevels.First(x => x.Order == order);

            var settings = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order).ToList();

            var data = bigdata.Where(x => x.ContainsKey($"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}")).GroupBy(x => x[$"{MediaHierarchyLevelPrefix}{level.Order}_{level.ColumnName}"]);

            foreach (var setting in settings)
            {
                var settingData = data.FirstOrDefault(x => (x.Key as string) == setting.Name);

                if (settingData != null)
                {
                    var values = settingData.First();

                    ICollection<FlowchartDataMetric>? metrics = null;
                    ICollection<FlowchartDataSubTotal>? subTotals = null;
                    ICollection<FlowchartDataTotal>? totals = null;
                    ICollection<FlowchartDataTotal>? actualTotals = null;
                    ICollection<FlowchartDataSubTotal>? subTotalSummary = null;
                    var list = settingData.ToList();
                    var orders = new int[] { level.Order, setting.Order };
                    var isGrpMetric = setting.MetricColumnName == AthenaConsts.GRPs;
                    if (levelIndex + 1 >= levelOrder.Count && (setting.SubLevels == null || !setting.SubLevels.Any()))
                    {
                        if (setting.MetricColumnName != "None")
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            metrics = GetMetrics(setting.MetricColumnName, LevelMetricPrefix, orders, setting.InflightOverlayColumnName, LevelInflightOverlayPrefix, list, setting.InflightOverlays?.ToList());
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                        else
                        {
                            metrics = GetMetrics(DefaultMetricColumnName, LevelMetricPrefix, orders, setting.InflightOverlayColumnName, LevelInflightOverlayPrefix, list, setting.InflightOverlays?.ToList());
                        }
                        // on lowest levels
                        actualTotals = GetTotals(totalsDefinition, list, isGrpMetric);
                    }
                    // on all the levels
                    totals = GetTotals(totalsDefinition, list, isGrpMetric);
                    // Subtotals on each level
                    subTotals = GetSubTotals(setting.SubTotals, LevelSubTotalPrefix, orders, list);
                    subTotalSummary = GetSubTotalSummary(setting.SubTotalSummary, LevelSubTotalSummaryPrefix, orders, list);
#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8629 // Nullable value type may be null.
                    var newLevel = new FlowchartDataLevel
                    {
                        TableId = level.TableId,
                        ColumnName = level.ColumnName,
                        Name = setting.Name,
                        Order = setting.Order,
                        Metrics = metrics,
                        SubTotals = subTotals,
                        Totals = totals,
                        FlightRange = setting.FlightRange,
                        MetricColumnName = setting.MetricColumnName,
                        MetricTableId = (Guid)setting.MetricTableId,
                        ActualTotals = actualTotals,
                        BriefedCtcTotals = level.ColumnName == AthenaConsts.Channel ? GetBriefedCtcTotals(list) : new List<FlowchartDataTotal>(),
                        LocalCurrency = list.GroupBy(x => x[CampaignLocalCurrencyKey]).Select(x => x.Key).FirstOrDefault()?.ToString(),
                        SubTotalSummary = subTotalSummary,
                    };
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning restore CS8601 // Possible null reference assignment.

                    if (setting.SubLevels != null)
                    {
                        var sublevels = setting.SubLevels.OrderBy(x => x.Order).Select(x => x.Order).ToList();

                        newLevel.SubLevels ??= new Collection<FlowchartDataSubLevel>();

                        AddSubLevel(0, sublevels, level, setting, setting.SubLevels, totalsDefinition, settingData.ToList(), newLevel.SubLevels);
                    }

                    flowchartDataLevels.Add(newLevel);

                    if (levelIndex + 1 < levelOrder.Count)
                    {
                        newLevel.Levels = new Collection<FlowchartDataLevel>();

                        AddLevel(levelIndex + 1, levelOrder, newLevel.Levels, mediaHierarchyLevels, totalsDefinition, settingData.ToList());
                    }
                }
            }
        }

        private void AddSubLevel(
            int levelIndex, List<int> levelOrder,
            MediaHierarchyLevel level,
            MediaHierarchySetting parentSetting,
            ICollection<MediaHierarchySubLevel> subLevels,
            TotalsDefinition? totalsDefinition,
            List<Dictionary<string, object?>> bigdata,
            ICollection<FlowchartDataSubLevel> newLevels)
        {
            var order = levelOrder[levelIndex];
            var sublevel = subLevels.First(x => x.Order == order);

            var data = bigdata.GroupBy(x => x[$"{MediaHierarchySubLevelPrefix}_{level.Order}_{parentSetting.Order}_{sublevel.Order}_{sublevel.ColumnName}"]);

            foreach (var setting in sublevel.Settings.Where(x => x.Enabled).OrderBy(x => x.Order))
            {
                var settingData = data.FirstOrDefault(x => (x.Key as string) == setting.Name);

                if (settingData != null)
                {
                    var values = settingData.First();

                    ICollection<FlowchartDataMetric>? metrics = null;
                    ICollection<FlowchartDataSubTotal>? subTotals = null;
                    ICollection<FlowchartDataTotal>? totals = null;

                    if (levelIndex + 1 >= levelOrder.Count)
                    {
                        var isGrpMetric = setting.MetricColumnName == AthenaConsts.GRPs;
                        // on the lowest level only
                        var list = settingData.ToList();

                        var orders = new int[] { level.Order, parentSetting.Order, sublevel.Order, setting.Order };

                        if (setting.MetricColumnName != "None")
                        {
#pragma warning disable CS8604 // Possible null reference argument.
                            metrics = GetMetrics(setting.MetricColumnName, SubLevelMetricPrefix, orders, setting.InflightOverlayColumnName, SubLevelInflightOverlayPrefix, list);
#pragma warning restore CS8604 // Possible null reference argument.
                        }
                        else
                        {
                            metrics = GetMetrics(DefaultMetricColumnName, SubLevelMetricPrefix, orders, setting.InflightOverlayColumnName, SubLevelInflightOverlayPrefix, list);
                        }
                        subTotals = GetSubTotals(setting.SubTotals, SubLevelSubTotalPrefix, orders, list);
                        totals = GetTotals(totalsDefinition, list, isGrpMetric);
                    }

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8629 // Nullable value type may be null.
                    var newSub = new FlowchartDataSubLevel
                    {
                        ColumnName = sublevel.ColumnName,
                        Order = setting.Order,
                        TableId = sublevel.TableId,
                        Totals = totals,
                        SubTotals = subTotals,
                        Name = setting.Name,
                        Metrics = metrics,
                        FlightRange = setting.FlightRange,
                        MetricColumnName = setting.MetricColumnName,
                        MetricTableId = (Guid)setting.MetricTableId,
                        BriefedCtcTotals = sublevel.ColumnName == AthenaConsts.Channel ? GetBriefedCtcTotals(settingData.ToList()) : new List<FlowchartDataTotal>(),
                    };
#pragma warning restore CS8629 // Nullable value type may be null.
#pragma warning restore CS8601 // Possible null reference assignment.

                    newLevels.Add(newSub);

                    if (levelIndex + 1 < levelOrder.Count)
                    {
                        newSub.SubLevels = new Collection<FlowchartDataSubLevel>();

                        AddSubLevel(levelIndex + 1, levelOrder, level, parentSetting, subLevels, totalsDefinition, settingData.ToList(), newSub.SubLevels);
                    }
                }
            }
        }

        private async Task<(DateTime StartDate, DateTime EndDate)> GetBroadcastCalendarDatesAsync(int year, Guid userId, CancellationToken cancellationToken)
        {
            var broadcastCalendar = await GetBroadcastCalendarAsync(year, userId, cancellationToken);

            var startDate = broadcastCalendar.SelectMany(x => x).Where(x => x.ColumnName == AthenaConsts.RollPeriodFirstDay)
                            .Select(x => Json.Deserialize<DateTime>(x.JsonValue)).Min();

            var endDate = broadcastCalendar.SelectMany(x => x).Where(x => x.ColumnName == AthenaConsts.RollPeriodLastDay)
                            .Select(x => Json.Deserialize<DateTime>(x.JsonValue)).Max();

            return (startDate, endDate);
        }

        private async Task<(DateTime StartDate, DateTime EndDate)> GetClientCalendarDatesAsync(int year, Guid omniClientId, Guid userId, CancellationToken cancellationToken)
        {
            var clientCalendar = await GetClientCalendarAsync(year.ToString(), omniClientId, userId, cancellationToken);

            var select = clientCalendar.SelectMany(x => x).Where(x => x.ColumnName == AthenaConsts.ClientRollPeriodClientWeekDayDate)
                            .Select(x => Json.Deserialize<DateTime>(x.JsonValue));

            return (select.Min(), select.Max());
        }

        private async Task<(DateTime StartDate, DateTime EndDate)> GetDatesAsync(
            CalendarDefinition calendarDefinition,
            Guid omniClientId,
            Guid userId,
            CancellationToken cancellationToken)
        {
            DateTime startDate;
            DateTime endDate;

            if (!calendarDefinition.Configuration.IsReportingTimeFrame)
            {
                calendarDefinition.Configuration.CustomStartDate.ThrowIfNull();
                calendarDefinition.Configuration.CustomEndDate.ThrowIfNull();

                startDate = calendarDefinition.Configuration.CustomStartDate.Value;
                endDate = calendarDefinition.Configuration.CustomEndDate.Value;
            }
            else
            {
                calendarDefinition.Configuration.ReportingTimeFrame.ThrowIfNull();

                (startDate, endDate) = calendarDefinition.Configuration.ReportingTimeFrame.Value switch
                {
                    CalendarReportingTimeFrame.CurrentYear => calendarDefinition.Configuration.Type switch
                    {
                        CalendarType.Broadcast => await GetBroadcastCalendarDatesAsync(DateTime.UtcNow.Year, userId, cancellationToken),
                        CalendarType.Client => await GetClientCalendarDatesAsync(DateTime.UtcNow.Year, omniClientId, userId, cancellationToken),
                        CalendarType.Standard => (new DateTime(DateTime.UtcNow.Year, 1, 1), new DateTime(DateTime.UtcNow.Year, 12, 31)),
                        _ => throw new ArgumentException($"Calendar type {calendarDefinition.Configuration.Type} is not supported."),
                    },
                    CalendarReportingTimeFrame.LastYear => calendarDefinition.Configuration.Type switch
                    {
                        CalendarType.Broadcast => await GetBroadcastCalendarDatesAsync(DateTime.UtcNow.AddYears(-1).Year, userId, cancellationToken),
                        CalendarType.Client => await GetClientCalendarDatesAsync(DateTime.UtcNow.AddYears(-1).Year, omniClientId, userId, cancellationToken),
                        CalendarType.Standard => (new DateTime(DateTime.UtcNow.AddYears(-1).Year, 1, 1), new DateTime(DateTime.UtcNow.AddYears(-1).Year, 12, 31)),
                        _ => throw new ArgumentException($"Calendar type {calendarDefinition.Configuration.Type} is not supported."),
                    },
                    _ => throw new NotSupportedException($"Reporting time frame {calendarDefinition.Configuration.ReportingTimeFrame} is not supported."),
                };
            }

            return (startDate, endDate);
        }

        private FlowchartData GetFlowchartData(
            FlowchartDefinition flowchartDefinition,
            ICollection<Dictionary<string, object?>> data)
        {
            var result = new FlowchartData()
            {
                Levels = new Collection<FlowchartDataLevel>(),
            };

            var levels = flowchartDefinition!.MediaHierarchyDefinition!.Definition.Levels.OrderBy(x => x.Order).Select(x => x.Order).ToList();

            AddLevel(0, levels, result.Levels, flowchartDefinition!.MediaHierarchyDefinition!.Definition.Levels, flowchartDefinition.TotalsDefinition?.Definition, data);

            if (flowchartDefinition.GrandTotalDefinition?.Definition != null)
            {
                AddGrandTotals(result, flowchartDefinition.GrandTotalDefinition.Definition, data.ToList());
            }

            return result;
        }

        private async Task<string> GetJoinAsync(
            DateTime startDate, DateTime endDate,
            DataRequestDTO dataRequest,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            bool includeSourceMediaToolsData,
            CancellationToken cancellationToken,
            bool showBriefedCTC)
        {
            var legendColumn = dataRequest.FlowchartDefinition?.ThemeDefinition?.Definition?.LegendTheme?.ColumnName;
            var where = string.Empty;
            bool isLocalCurrency = dataRequest.Currency == Currency.LLL.ToString();

            var selectedColumns = DefaultColumns;

            var runWhere = await GetRunRestrictionsAsync(dataRequest.RunRestrictions, dictionaryTables, cancellationToken);

            var (selected, restrictions) = await GetSelectedColumnsAsync(dataRequest.FlowchartDefinition, dictionaryTables, cancellationToken, Enum.Parse<Currency>(dataRequest.Currency ?? Currency.LLL.ToString()));

            selectedColumns += $", {string.Join(",", selected)}";

            // Regex is used to check if channel is selected in media hierarchy at level or sublevel

            var regexChannel = new Regex(@$"{MediaHierarchyLevelPrefix}\d*_channel*");
            var regexSubLevelChannel = new Regex(@$"{MediaHierarchySubLevelPrefix}_\d*_\d*_\d*_channel*");  // media_plans.channel AS sblvl_0_7_0_channel
            bool isChannelSelected = selected.Exists(x => regexChannel.IsMatch(x) || regexSubLevelChannel.IsMatch(x));

            if (isChannelSelected && showBriefedCTC)
            {
                string briefedCtc = isLocalCurrency ? $"{AthenaConsts.BriefedCtc}_local" : AthenaConsts.BriefedCtc;
                selectedColumns += $", {AthenaConsts.MediaBriefsTable}.{briefedCtc} AS {TotalsPrefix}_{AthenaConsts.BriefedCtc}";
            }
            if (!string.IsNullOrWhiteSpace(legendColumn))
            {
                var table = await portal.DataDictionaryTables.GetByIdAsync((Guid)dataRequest.FlowchartDefinition?.ThemeDefinition?.Definition?.LegendTheme?.TableId, cancellationToken);
                selectedColumns += $", {table.Name}.{legendColumn} AS {LegendPrefix}";
            }

            bool? mediaBriefRequired = ((selectedColumns.Contains(AthenaConsts.MediaBriefsTable) || (!string.IsNullOrWhiteSpace(runWhere) && runWhere.Contains(AthenaConsts.MediaBriefsTable))) && (selectedColumns.Contains(AthenaConsts.MediaPlansTable) || (!string.IsNullOrWhiteSpace(runWhere) && runWhere.Contains(AthenaConsts.MediaPlansTable)))) ? true : (selectedColumns.Contains(AthenaConsts.MediaPlansTable) || (!string.IsNullOrWhiteSpace(runWhere) && runWhere.Contains(AthenaConsts.MediaPlansTable))) ? false : null;

            var tableToJoin = mediaBriefRequired.HasValue ? mediaBriefRequired.Value ? TableJoin : TableJoinMediaPlans : TableJoinMediaBriefs;

            if (!mediaBriefRequired.HasValue)
            {
                selectedColumns += $", {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaBriefId} AS {MediaBriefIdKey}";
                selectedColumns += $", {string.Join(",", MediaBriefsColumns)}";
            }
            else if (mediaBriefRequired.Value)
            {
                // Add additional columns from MediaPlans table if they exist in MediaBriefs table
                // Add additional columns from MediaBriefs table if they exist in MediaPlans table
                var additionalMediaPlansColumns = await GetSelectedColumnToAddForJoin(AthenaConsts.MediaBriefsTable, AthenaConsts.MediaPlansTable, selected, dictionaryTables, cancellationToken);
                var additionalMediaBriefColumns = await GetSelectedColumnToAddForJoin(AthenaConsts.MediaPlansTable, AthenaConsts.MediaBriefsTable, selected, dictionaryTables, cancellationToken);
                if (additionalMediaPlansColumns.Any()) { selectedColumns += $", {string.Join(",", additionalMediaPlansColumns)}"; }
                if (additionalMediaBriefColumns.Any()) { selectedColumns += $", {string.Join(",", additionalMediaBriefColumns)}"; }

                selectedColumns += $", {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaBriefId} AS {MediaBriefIdKey}";
                selectedColumns += $", {string.Join(",", MediaBriefsColumns)}";
                selectedColumns += $", {string.Join(",", MediaPlansColumns)}";

                // Add additional where from MediaPlans table if they exist in MediaBriefs table
                // Add additional where from MediaBriefs table if they exist in MediaPlans table
                var additionalMediaPlansWhereClause = GetAdditionalWhereConditions(AthenaConsts.MediaBriefsTable, AthenaConsts.MediaPlansTable, restrictions, dictionaryTables, cancellationToken);
                var additionalMediaBriefWhereClause = GetAdditionalWhereConditions(AthenaConsts.MediaPlansTable, AthenaConsts.MediaBriefsTable, restrictions, dictionaryTables, cancellationToken);
                if (additionalMediaPlansWhereClause.Any()) { restrictions.AddRange(additionalMediaPlansWhereClause); }
                if (additionalMediaBriefWhereClause.Any()) { restrictions.AddRange(additionalMediaBriefWhereClause); }
            }
            else
            {
                selectedColumns += $", {string.Join(",", MediaPlansColumns)}";
            }

            var query = $"SELECT {selectedColumns} {tableToJoin}";
            if (!mediaBriefRequired.HasValue)
            {
                where += $"{AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})";
                where += $" AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})";
            }
            else
            {
                if (mediaBriefRequired.Value)
                {
                    where += $"{AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})";
                    where += $" AND {AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})";
                    where += $" AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})";
                    where += $" AND {AthenaConsts.MediaBriefsTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})";
                }
                else
                {
                    where += $"{AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} >= DATE({SqlQuote(startDate.ToString("yyyy-MM-dd"))})";
                    where += $" AND {AthenaConsts.MediaPlansTable}.{AthenaConsts.MediaPlansEffectiveDate} <= DATE({SqlQuote(endDate.ToString("yyyy-MM-dd"))})";
                }
            }

            where += !includeSourceMediaToolsData ? $" AND {CampaignConstraintsWithSource}" : $" AND {CampaignConstraints}";

            if (!mediaBriefRequired.HasValue)
            {
                where += !includeSourceMediaToolsData ? $" AND {MediaBriefsConstraintsWithSource}" : $" AND {MediaBriefsConstraints}";
            }
            else if (mediaBriefRequired.Value)
            {
                where += !includeSourceMediaToolsData ? $" AND {MediaBriefsConstraintsWithSource} AND {MediaPlansConstraintsWithSource}" : $" AND {MediaBriefsConstraints} AND {MediaPlansConstraints}";
            }
            else
            {
                where += !includeSourceMediaToolsData ? $" AND {MediaPlansConstraintsWithSource}" : $" AND {MediaPlansConstraints}";
            }

            where += $" AND {string.Join(" AND ", restrictions)}";

            if (!string.IsNullOrWhiteSpace(runWhere))
            {
                where += $" AND ({runWhere})";
            }
            var omniClients = await GetOmniClientHierarchy(dataRequest.OmniClientId, cancellationToken);
            var omniWhere = GetOmniClientIdRestrictionsWithData(omniClients, mediaBriefRequired);

            if (!string.IsNullOrWhiteSpace(omniWhere))
            {
                where += $" AND {omniWhere}";
            }

            var clientDetails = await portal.OmniClients.GetByIdAsync(dataRequest.OmniClientId, cancellationToken);
            if (!string.IsNullOrWhiteSpace(clientDetails?.Client?.Name) && selected.Contains($"{AthenaConsts.CampaignTable}."))
            {
                where += $" AND {AthenaConsts.CampaignTable}.client_name={SqlQuote(clientDetails.Client.Name)}";
            }

            query += $" WHERE {where}";

            log.Add(LogLevel.Information, query);

            return query;
        }

        private async Task<List<string>> GetSelectedColumnToAddForJoin(string sourceTable, string destinationTable, List<string> selectedColumns, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken)
        {
            List<string> additionnalColumnSelected = new();

            if (string.IsNullOrWhiteSpace(sourceTable) || string.IsNullOrWhiteSpace(destinationTable))
            {
                return additionnalColumnSelected;
            }
            try
            {
                // Regex levelColumnMatch = new Regex(@"^lvl[0-9]+_");  // commented will check if duplicate columns will add any extra metrics

                foreach (var c in selectedColumns.Where(c => c.Trim().ToLower().StartsWith(sourceTable)))
                {
                    var columnName = c.Split(" AS ").Length > 1 ? c.Split(" AS ")[0].Split('.')[1] : c.Split('.')[1];
                    var isColumnExist = false;
                    try
                    {
                        var tableData = await GetTableByNameAsync(destinationTable, dictionaryTables, cancellationToken);
                        await GetTableColumnAsync(tableData.Id, columnName, dictionaryTables, cancellationToken);
                        isColumnExist = true;
                    }
                    catch (Exception)
                    {
                        isColumnExist = false;
                    }
                    if (isColumnExist)
                    {
                        additionnalColumnSelected.Add(c.Replace($"{sourceTable}.{columnName}", $"{destinationTable}.{columnName}"));
                    }
                }
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, ex.Message, ex);
            }
            return additionnalColumnSelected;
        }

        private List<string> GetAdditionalWhereConditions(string sourceTable, string destinationTable, List<string> where, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken)
        {
            List<string> additionnalWhereClause = new();

            if (where == null || where.Count == 0 || string.IsNullOrWhiteSpace(sourceTable) || string.IsNullOrWhiteSpace(destinationTable))
            {
                return additionnalWhereClause;
            }
            try
            {
                where.Select(c => c.Trim()).Where(c => c.Trim().ToLower().StartsWith(sourceTable)).ToList().ForEach(async c =>
                {
                    var columnName = c.Split(" IN ")[0].Split('.')[1];
                    var isColumnExist = false;
                    try
                    {
                        var tableData = await GetTableByNameAsync(destinationTable, dictionaryTables, cancellationToken);
                        await GetTableColumnAsync(tableData.Id, columnName, dictionaryTables, cancellationToken);
                        isColumnExist = true;
                    }
                    catch (Exception)
                    {
                        isColumnExist = false;
                    }
                    if (isColumnExist)
                    {
                        additionnalWhereClause.Add(c.Replace($"{sourceTable}.{columnName}", $"{destinationTable}.{columnName}"));
                    }
                });
            }
            catch (Exception ex)
            {
                log.Add(LogLevel.Error, ex.Message, ex);
            }
            return additionnalWhereClause;
        }

        private ICollection<FlowchartDataMetric> GetMetrics(
            string metricColumnName, string metricPrefix,
            int[]? orders,
            string? inflightOverlayColumnName, string inflightOverlayPrefix,
            List<Dictionary<string, object?>> data, List<MediaHierarchyInflightOverlay>? mediahierarchyInflightOverlays = null)
        {
            var additional = orders == null ? string.Empty : $"_{string.Join("_", orders)}";
            var metricField = $"{metricPrefix}{additional}_{metricColumnName}";
            var result = new Collection<FlowchartDataMetric>();

            foreach (var row in data)
            {
                if (row.ContainsKey(metricField) && row[metricField] != null)
                {
                    var inflightOverlays = new List<string>();

                    var effectiveDate = DateOnly.FromDateTime(DateTime.Parse(row[EffectiveDateKey]!.ToString()));
                    var flightEnd = (row.ContainsKey(FlightEndKey) && row[FlightEndKey] != null) ? DateOnly.FromDateTime(DateTime.Parse(row[FlightEndKey]!.ToString())) : DateOnly.MaxValue;
                    var flightStart = (row.ContainsKey(FlightStartKey) && row[FlightStartKey] != null) ? DateOnly.FromDateTime(DateTime.Parse(row[FlightStartKey]!.ToString())) : DateOnly.MinValue;
                    // existing inflight overlay
                    //if (!string.IsNullOrWhiteSpace(inflightOverlayColumnName) && inflightOverlayColumnName != "None")
                    //{
                    //    var column = $"{inflightOverlayPrefix}{additional}_{inflightOverlayColumnName}";

                    //    if (row.ContainsKey(column) && row[column] != null)
                    //    {
                    //        inflightOverlays.Add(Json.Serialize(row[column]));
                    //    }
                    //}
                    var legend = row.ContainsKey(LegendPrefix) ? row[LegendPrefix] : null;
                    if (mediahierarchyInflightOverlays != null)
                    {
                        foreach (var mediaHierarchyInflightOverlay in mediahierarchyInflightOverlays)
                        {
                            if (mediaHierarchyInflightOverlay.ColumnName == CustomFlightRange)
                            {
                                inflightOverlays.Add(Json.Serialize($"{flightStart.ToString(DateFormat)} - {flightEnd.ToString(DateFormat)}"));
                            }
                            else
                            {
                                var overlayColumn = $"{inflightOverlayPrefix}{additional}_{mediaHierarchyInflightOverlay.Order}_{mediaHierarchyInflightOverlay.ColumnName}";

                                if (row.ContainsKey(overlayColumn) && row[overlayColumn] != null)
                                {
                                    inflightOverlays.Add(Json.Serialize(row[overlayColumn]));
                                }
                                else
                                {
                                    inflightOverlays.Add(Json.Serialize(null));
                                }
                            }
                        }
                    }

#pragma warning disable CS8604 // Possible null reference argument.
                    result.Add(new FlowchartDataMetric
                    {
                        EffectiveDate = effectiveDate,
                        FlightEnd = flightEnd,
                        FlightStart = flightStart,
                        ValueJson = Json.Serialize(row[metricField]),
                        InflightOverlays = inflightOverlays,
                        Legend = legend != null ? Json.Serialize(legend) : null,
                    });
#pragma warning restore CS8604 // Possible null reference argument.
                }
            }
            return result;
        }


        private string GetSubLeveleRestriction(string? tableName, string? columnName, string? value)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return string.Empty;
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                return $"{tableName}.{columnName}={SqlQuote(Convert.ToString(value.Split("|")[0]))}";
#pragma warning restore CS8604 // Possible null reference argument.
            }
        }

        private async Task<string> GetRunRestrictionsAsync(
            ICollection<RunRestriction> restrictions,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            CancellationToken cancellationToken)
        {
            var result = string.Empty;

            // group by tableid and column to format the query with multiple conditions
            var restrictionGroup = (from r in restrictions
                                    group r by new { r.TableId, r.ColumnName } into groups
                                    select new
                                    {
                                        TableId = groups.Key,
                                        Items = groups.Select(c => c),
                                    }).ToList();

            foreach (var restrictionsGroup in restrictionGroup)
            {
                if (!string.IsNullOrEmpty(result))
                {
                    result += " AND ";
                }
                var subResult = string.Empty;

                foreach (var restriction in restrictionsGroup.Items)
                {
                    var (table, column) = await GetTableColumnAsync(restriction.TableId, restriction.ColumnName, dictionaryTables, cancellationToken);

                    if (!string.IsNullOrEmpty(subResult))
                    {
                        subResult += " OR ";
                    }

                    var value = column.Type switch
                    {
                        ColumnType.Boolean => SqlQuote(Json.Deserialize<bool>(restriction.ValueJson).ToString()),
                        ColumnType.Date => $"DATE({SqlQuote(Json.Deserialize<DateTime>(restriction.ValueJson).ToString("yyyy-MM-dd"))})",
                        ColumnType.Decimal => SqlQuote(Json.Deserialize<decimal>(restriction.ValueJson).ToString()!.Replace(",", ".")),
                        ColumnType.LongInteger => SqlQuote(Json.Deserialize<long>(restriction.ValueJson).ToString()),
                        ColumnType.String => SqlQuote(Json.Deserialize<string>(restriction.ValueJson)!),
                        _ => throw new NotSupportedException($"Column type {column.Type} is not supported."),
                    };
                    subResult += $"({table.Name}.{column.Name} = {value})";
                }
                result += $"({subResult})";
            }

            return result;
        }

        private async Task<(List<string> Selected, List<string> Where)> GetSelectedColumnsAsync(
            FlowchartDefinition flowchartDefinition,
            List<DataDictionaryTableDetailsDTO> dictionaryTables,
            CancellationToken cancellationToken,
            Currency currency)
        {
            var selected = new List<string>();
            var where = new List<string>();
            bool isLocalCurrency = currency == Currency.LLL;
            string trimEndForLocal = isLocalCurrency ? "_local" : " ";
            var noneMetricTableGuid = Guid.Parse("00000000-0000-0000-0000-000000000001");
            foreach (var level in flowchartDefinition.MediaHierarchyDefinition!.Definition.Levels.OrderBy(x => x.Order))
            {
                level.Settings.ThrowIfNull().IfEmpty(x => x.Where(c => c.Enabled));

                var (table, column) = await GetTableColumnAsync(level.TableId, level.ColumnName, dictionaryTables, cancellationToken);

                selected.Add($"{table.Name}.{column.Name} AS {MediaHierarchyLevelPrefix}{level.Order}_{column.Name}");

                var settings = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                var settingsMetric = level.Settings.Where(x => x.Enabled).OrderBy(x => x.Order).Select(c =>
                {
                    c.MetricTableId = c.MetricTableId == noneMetricTableGuid ? DefaultMetricTableId : c.MetricTableId;
                    return c;
                }).GroupBy(c => c.MetricTableId);

                foreach (var metricSelected in settingsMetric)
                {
                    var (settingsMetricTable, settingsMetricColumn) = (table, column);
                    try
                    {
#pragma warning disable CS8629 // Nullable value type may be null.
                        (settingsMetricTable, settingsMetricColumn) = await GetTableColumnAsync(tableId: (Guid)metricSelected.Key, level.ColumnName, dictionaryTables, cancellationToken);
#pragma warning restore CS8629 // Nullable value type may be null.
                    }
                    catch (Exception ex)
                    {
                        log.Add(LogLevel.Error, $"Error getting table and column for metric table id {metricSelected.Key}", ex);
                    }
                    var values = string.Join(",", metricSelected.Select(x => SqlQuote(x.Name)));
                    if (string.Equals(column.Name, "supplierfreetext", StringComparison.InvariantCultureIgnoreCase)) // Supplier can be null for channel
                    {
                        where.Add($"({settingsMetricTable.Name}.{column.Name} IN ({values}) OR {settingsMetricTable.Name}.{column.Name} IS NULL)");
                    }
                    else
                    {
                        where.Add($"{settingsMetricTable.Name}.{column.Name} IN ({values})");
                    }
                }
                foreach (var setting in settings)
                {
                    if (setting.MetricColumnName != "None")
                    {
#pragma warning disable CS8629 // Nullable value type may be null.
                        (table, column) = await GetTableColumnAsync((Guid)setting.MetricTableId, setting.MetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
#pragma warning restore CS8629 // Nullable value type may be null.

                        selected.Add($"{table.Name}.{column.Name} AS {LevelMetricPrefix}_{level.Order}_{setting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                    }
                    else
                    {
                        (table, column) = await GetTableColumnAsync(DefaultMetricTableId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                        selected.Add($"{table.Name}.{column.Name} AS {LevelMetricPrefix}_{level.Order}_{setting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                    }

                    /*                    if (setting.InflightOverlayTableId != null && setting.InflightOverlayTableId.ToString() != "00000000-0000-0000-0000-000000000001")
                                        {
                                            setting.InflightOverlayColumnName.ThrowIfNull().IfEmpty();

                                            (table, column) = await GetTableColumnAsync(setting.InflightOverlayTableId.Value, setting.InflightOverlayColumnName, dictionaryTables, cancellationToken);

                                            selected.Add($"{table.Name}.{column.Name} AS {LevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{column.Name}");
                                        }*/

                    /*                    if (setting.InflightOverlays != null)
                                        {
                                            foreach (var inflightOverlay in setting.InflightOverlays.Where(io => io.ColumnName != "none").OrderBy(io => io.ColumnName))
                                            {
                                                (var retrievedTable, var retrievedColumn ) = await GetTableColumnAsync(inflightOverlay.TableId, inflightOverlay.ColumnName, dictionaryTables, cancellationToken);

                                                var tableName = table.Name;
                                                var columnName = column.Name;
                                                var settingOrder = setting.Order;

                                                selected.Add($"{tableName}.{columnName} AS {LevelInflightOverlayPrefix}{level.Order}{settingOrder}_{columnName}");
                                            }
                                        }*/

                    if (setting.InflightOverlays != null)
                    {
                        var validInflightOverlays = setting.InflightOverlays
                            .Where(overlay => overlay.ColumnName != "None" && overlay.ColumnName != CustomFlightRange)
                            .OrderBy(overlay => overlay.Order);

                        foreach (var inflightOverlay in validInflightOverlays)
                        {
                            if (inflightOverlay.TableId != Guid.Empty)
                            {
                                var (retrievedTable, retrievedColumn) = await GetTableColumnAsync(inflightOverlay.TableId, inflightOverlay.ColumnName, dictionaryTables, cancellationToken);

                                if (retrievedTable != null && retrievedColumn != null)
                                {
                                    var tableName = retrievedTable.Name;
                                    var columnName = retrievedColumn.Name;
                                    var settingOrder = setting.Order;

                                    selected.Add($"{tableName}.{columnName} AS {LevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{inflightOverlay.Order}_{columnName}");
                                }
                            }
                        }
                    }

                    if (setting.SubTotals != null)
                    {
                        setting.SubTotals.Throw().IfEmpty();

                        foreach (var total in setting.SubTotals)
                        {
                            if (total.ColumnName != "None")
                            {
                                (table, column) = await GetTableColumnAsync(total.TableId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                                selected.Add($"{table.Name}.{column.Name} AS {LevelSubTotalPrefix}_{level.Order}_{setting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                            }
                            else
                            {
                                (table, column) = await GetTableColumnAsync(DefaultMetricTableId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                selected.Add($"{table.Name}.{column.Name} AS {LevelSubTotalPrefix}_{level.Order}_{setting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                            }
                        }
                    }

                    if (setting.SubLevels != null)
                    {
                        setting.SubLevels.Throw().IfEmpty();

                        foreach (var sublevel in setting.SubLevels)
                        {
                            sublevel.Settings.ThrowIfNull().IfEmpty(x => x.Where(c => c.Enabled));

                            (table, column) = await GetTableColumnAsync(sublevel.TableId, sublevel.ColumnName, dictionaryTables, cancellationToken);

                            selected.Add($"{table.Name}.{column.Name} AS {MediaHierarchySubLevelPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{column.Name}");

                            var subsettings = sublevel.Settings.Where(x => x.Enabled).OrderBy(x => x.Order);

                            foreach (var subsetting in subsettings)
                            {
                                if (subsetting.MetricColumnName != "None")
                                {
#pragma warning disable CS8629 // Nullable value type may be null.
                                    (table, column) = await GetTableColumnAsync((Guid)subsetting.MetricTableId, subsetting.MetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
#pragma warning restore CS8629 // Nullable value type may be null.

                                    selected.Add($"{table.Name}.{column.Name} AS {SubLevelMetricPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                                }
                                else
                                {
                                    (table, column) = await GetTableColumnAsync(DefaultMetricTableId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                                    selected.Add($"{table.Name}.{column.Name} AS {SubLevelMetricPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                                }
                                if (subsetting.InflightOverlayTableId != null && subsetting.InflightOverlayTableId.ToString() != "00000000-0000-0000-0000-000000000001")
                                {
                                    subsetting.InflightOverlayColumnName.ThrowIfNull().IfEmpty();

                                    (table, column) = await GetTableColumnAsync(subsetting.InflightOverlayTableId.Value, subsetting.InflightOverlayColumnName, dictionaryTables, cancellationToken);

                                    selected.Add($"{table.Name}.{column.Name} AS {SubLevelInflightOverlayPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{column.Name}");
                                }

                                if (subsetting.SubTotals != null)
                                {
                                    subsetting.SubTotals.Throw().IfEmpty();

                                    foreach (var total in subsetting.SubTotals)
                                    {
                                        (table, column) = await GetTableColumnAsync(total.TableId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                                        selected.Add($"{table.Name}.{column.Name} AS {SubLevelSubTotalPrefix}_{level.Order}_{setting.Order}_{sublevel.Order}_{subsetting.Order}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                                    }
                                }
                            }
                        }
                    }


                    if (setting.SubTotalSummary != null)
                    {
                        setting.SubTotalSummary.Throw().IfEmpty();

                        foreach (var total in setting.SubTotalSummary)
                        {

                            total.Settings.Throw().IfEmpty();

                            foreach (var subTotalSetting in total.Settings)
                            {
                                (table, column) = await GetTableColumnAsync(subTotalSetting.TableId, subTotalSetting.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                                selected.Add($"{table.Name}.{column.Name} AS {LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotalSetting.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                            }

                            total.SubTotals.Throw().IfEmpty();

                            foreach (var subTotal in total.SubTotals)
                            {
                                if (subTotal.ColumnName != "None")
                                {
                                    (table, column) = await GetTableColumnAsync(subTotal.TableId, subTotal.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                                    selected.Add($"{table.Name}.{column.Name} AS {LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotal.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                                }
                                else
                                {
                                    (table, column) = await GetTableColumnAsync(DefaultMetricTableId, DefaultMetricColumnName, dictionaryTables, cancellationToken, isLocalCurrency);
                                    selected.Add($"{table.Name}.{column.Name} AS {LevelSubTotalSummaryPrefix}_{level.Order}_{setting.Order}_{total.Order}_{subTotal.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                                }
                            }
                        }
                    }
                }
            }

            if (flowchartDefinition.TotalsDefinition?.Definition != null)
            {
                foreach (var total in flowchartDefinition.TotalsDefinition.Definition.Columns)
                {
                    var (table, column) = await GetTableColumnAsync(total.TableId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                    selected.Add($"{table.Name}.{column.Name} AS {TotalsPrefix}{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                }
            }

            if (flowchartDefinition.GrandTotalDefinition?.Definition != null)
            {
                flowchartDefinition.GrandTotalDefinition.Definition.Selections.ThrowIfNull().IfEmpty();

                foreach (var total in flowchartDefinition.GrandTotalDefinition.Definition.Selections)
                {
                    var (table, column) = await GetTableColumnAsync(total.TableId, total.ColumnName, dictionaryTables, cancellationToken, isLocalCurrency);

                    selected.Add($"{table.Name}.{column.Name} AS {GrandTotalsPrefix}_{total.Order}_{column.Name.Replace(trimEndForLocal, string.Empty)}");
                }
            }

            return (Selected: selected, Where: where);
        }

        private ICollection<FlowchartDataSubTotal>? GetSubTotals(
            ICollection<MediaHierarchySubTotal>? subTotals,
            string subTotalPrefix,
            int[] orders,
            List<Dictionary<string, object?>> data)
        {
            if (subTotals == null || !subTotals.Any())
            {
                return null;
            }

            var result = new Collection<FlowchartDataSubTotal>();

            foreach (var subtotal in subTotals.OrderBy(x => x.Order))
            {
                var order = new List<int>(orders)
                {
                    subtotal.Order,
                };

                result.Add(new FlowchartDataSubTotal
                {
                    ColumnName = subtotal.ColumnName,
                    TableId = subtotal.TableId,
                    Order = subtotal.Order,
                    Metrics = subtotal.ColumnName != "None" ? GetMetrics(subtotal.ColumnName, subTotalPrefix, order.ToArray(), null, string.Empty, data) : GetMetrics(DefaultMetricColumnName, subTotalPrefix, order.ToArray(), null, string.Empty, data),
                    FlightRange = subtotal.FlightRange,
                });
            }

            return result;
        }

        private ICollection<FlowchartDataSubTotal>? GetSubTotalSummary(
            ICollection<MediaHierarchySubTotalSummary>? subTotalSummary,
            string subTotalSummaryPrefix,
            int[] orders,
            List<Dictionary<string, object?>> bigdata)
        {
            Stopwatch sw = new();
            sw.Start();

            if (subTotalSummary == null || subTotalSummary.Count == 0)
            {
                return null;
            }

            var result = new Collection<FlowchartDataSubTotal>();

            foreach (var subtotal in subTotalSummary.OrderBy(x => x.Order))
            {
                var order = new List<int>(orders)
                {
                    subtotal.Order,
                };

                var data = bigdata;
                var latestData = new Dictionary<string, List<Dictionary<string, object?>>>();
                for (var i = 0; i < subtotal.Settings.Count; i++)
                {
                    var setting = subtotal.Settings.ToList()[i];
                    var order1 = new List<int>(order)
                        {
                             setting.Order,
                        };
                    var additional = order1 == null ? string.Empty : $"_{string.Join("_", order1)}";
                    var settingField = $"{subTotalSummaryPrefix}{additional}_{setting.ColumnName}";
                    var values = setting.Values.Split(";");
                    var valuesJson = new Dictionary<string, string>();
                    if (i == 0)
                    {
                        var subData = data.Where(x => x.ContainsKey(settingField)).GroupBy(x => x[settingField]);
                        foreach (var value in values)
                        {
                            var settingData = subData.FirstOrDefault(x => (x.Key as string) == value);

                            if (settingData != null)
                            {
                                var list = settingData.ToList();
                                latestData.Add(value, list);
                            }
                        }
                    }
                    else
                    {
                        var newLatestData = new Dictionary<string, List<Dictionary<string, object?>>>();
                        foreach (var dict in latestData)
                        {
                            var subData = dict.Value.Where(x => x.ContainsKey(settingField)).GroupBy(x => x[settingField]);
                            foreach (var value in values)
                            {
                                var settingData = subData.FirstOrDefault(x => (x.Key as string) == value);

                                if (settingData != null)
                                {
                                    var list = settingData.ToList();
                                    newLatestData.Add($"{value} {dict.Key}", list);
                                }
                            }
                        }
                        latestData = newLatestData;
                    }
                }

                for (var metricsCount = 0; metricsCount < subtotal.SubTotals.Count; metricsCount++)
                {
                    var subTotal = subtotal.SubTotals.ToList()[metricsCount];
                    foreach (var dict in latestData)
                    {
                        var order1 = new List<int>(order)
                        {
                           subTotal.Order,
                        };
                        var additional = order1 == null ? string.Empty : $"_{string.Join("_", order1)}";
                        var metricField = $"{subTotalSummaryPrefix}{additional}_{subTotal.ColumnName}";
                        var metric = GetMetrics(subTotal.ColumnName, subTotalSummaryPrefix, order1.ToArray(), null, string.Empty, dict.Value);

                        result.Add(new FlowchartDataSubTotal
                        {
                            ColumnName = subTotal.ColumnName,
                            TableId = subTotal.TableId,
                            Order = subtotal.Order,
                            Metrics = metric,
                            FlightRange = subTotal.FlightRange,
                            DisplayName = dict.Key,
                        });
                    }
                }
            }

            sw.Stop();
            log.Add(LogLevel.Information, $"GetSubTotalSummary took {sw.ElapsedMilliseconds} ms");
            return result;
        }

        private async Task<DataDictionaryTableDetailsDTO> GetTableAsync(Guid id, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken)
        {
            var table = dictionaryTables.Find(x => x.Id == id);

            if (table == null)
            {
                table = mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByIdAsync(id, cancellationToken));
                dictionaryTables.Add(table);
            }

            return table;
        }

        private async Task<DataDictionaryTableDetailsDTO> GetTableByNameAsync(string name, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken)
        {
            var table = dictionaryTables.Find(x => x.Name == name);

            if (table == null)
            {
                table = mapper.Map<DataDictionaryTableDetailsDTO>(await portal.DataDictionaryTables.GetByNameAsync(name, cancellationToken));
                dictionaryTables.Add(table);
            }

            return table;
        }
        private async Task<(DataDictionaryTableDetailsDTO Table, DataDictionaryColumnDetailsDTO Column)>
                                    GetTableColumnAsync(Guid tableId, string columnName, List<DataDictionaryTableDetailsDTO> dictionaryTables, CancellationToken cancellationToken, bool isLocalCurrency = false)
        {
            var table = await GetTableAsync(tableId, dictionaryTables, cancellationToken);

            var column = table.Columns?.FirstOrDefault(x => x.Name == columnName)
                ?? throw new KeyNotFoundException($"Column {columnName} not found in table {table.Name}");

            return isLocalCurrency ? GetLocalMetricColumnAsync(table, column) : (table, column);
        }

        private ICollection<FlowchartDataTotal>? GetTotals(
            TotalsDefinition? totalsDefinition,
            List<Dictionary<string, object?>> data, bool isGrpMetric = false)
        {
            if (totalsDefinition == null)
            {
                return null;
            }
            var result = new Collection<FlowchartDataTotal>();

            var first = data.First();

            foreach (var total in totalsDefinition.Columns)
            {
                result.Add(new FlowchartDataTotal
                {
                    ColumnName = total.ColumnName,
                    Name = total.Name,
                    Order = total.Order,
                    TableId = total.TableId,
                    ValuesJson = isGrpMetric && MetricCurrencyColumns.Contains(total.ColumnName) ? new List<string>() : data.Where(x => x.ContainsKey($"{TotalsPrefix}{total.Order}_{total.ColumnName}")).Select(x => x[$"{TotalsPrefix}{total.Order}_{total.ColumnName}"]).Where(x => x != null).Select(x => Json.Serialize(x)).ToList(),
                });
            }

            return result;
        }

        // Below method is used to get the BriefedCtc when in Media Hierarchy Channel is selected
        private ICollection<FlowchartDataTotal>? GetBriefedCtcTotals(
            List<Dictionary<string, object?>> data)
        {
            var result = new Collection<FlowchartDataTotal>
            {
                new()
                {
                    ColumnName = AthenaConsts.BriefedCtc,
                    Name = AthenaConsts.Channel,
                    Order = 0, // not used
                    TableId = Guid.NewGuid(), // not used
                    ValuesJson = data.Where(x => x.ContainsKey($"{TotalsPrefix}_{AthenaConsts.BriefedCtc}")).Select(x => x[$"{TotalsPrefix}_{AthenaConsts.BriefedCtc}"]).Where(x => x != null).Select(x => Json.Serialize(x)).ToList(),
                },
            };

            return result;
        }

        private (DataDictionaryTableDetailsDTO Table, DataDictionaryColumnDetailsDTO Column)
                                    GetLocalMetricColumnAsync(DataDictionaryTableDetailsDTO table, DataDictionaryColumnDetailsDTO column)
        {
            try
            {
                column = table.Columns?.FirstOrDefault(x => x.Name == $"{column.Name}_local")
                 ?? throw new KeyNotFoundException($"Column {column.Name} not found in table {table.Name}");
            }
            catch (Exception)
            {
                log.Add(LogLevel.Error, $"Error getting local currency column for {column.Name}");
            }
            return (table, column);
        }

        /// <summary>
        /// Check the omni guid exists in Planit AMD database Async.
        /// </summary>
        /// <param name="omniClientId">The omniClientId</param>
        /// <param name="includeSourceMediaToolsData">The includeSourceMediaToolsData</param>
        /// <param name="cancellationToken">The cancellation token. Token</param>
        public async Task<bool> CheckOmniGuidExistsAsync(Guid omniClientId, bool includeSourceMediaToolsData, CancellationToken cancellationToken)
        {
            log.Add(LogLevel.Information, $"Check the omni guid exists in Planit AMD database.");
            var omniClients = await GetOmniClientHierarchy(omniClientId, cancellationToken);
            var briefedQuery = $"SELECT 1 as OmniGuidCount FROM {AthenaConsts.MediaPlansTable} WHERE lower({AthenaConsts.OmniGuid}) IN {omniClients} AND  lower({AthenaConsts.DataState}) = 'submitted' AND {AthenaConsts.AmdState} = 'active'";

            briefedQuery += !includeSourceMediaToolsData ? $" AND lower({AthenaConsts.Source}) <> 'mediatools'" : string.Empty;

            var query = $"SELECT 1 as OmniGuidCount FROM {AthenaConsts.MediaBriefsTable} WHERE lower({AthenaConsts.OmniGuid}) IN {omniClients} AND  lower({AthenaConsts.DataState}) = 'submitted' AND {AthenaConsts.AmdState} = 'active'";

            query += !includeSourceMediaToolsData ? $" AND lower({AthenaConsts.Source}) <> 'mediatools'" : string.Empty;

            query += $" union all {briefedQuery} limit 1";

            var result = await queryLogic.QueryAsync(query, cancellationToken);
            return result.Rows.Any();
        }
    }
}