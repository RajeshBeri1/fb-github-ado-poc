using System.Diagnostics.CodeAnalysis;

namespace Lib.Annalect.Models
{
    /// <summary>
    /// Project
    /// </summary>
    [ExcludeFromCodeCoverage]
    public class Project
    {
        /*
        /// <summary>
        /// Gets or sets the accessed when.
        /// </summary>
        /// <value>The accessed when.</value>
        public string? AccessedWhen { get; set; }
        */

        /// <summary>
        /// Gets or sets the client identifier.
        /// </summary>
        /// <value>The client identifier.</value>
        public string ClientId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the created by.
        /// </summary>
        /// <value>The created by.</value>
        public string CreatedBy { get; set; } = default!;

        /// <summary>
        /// Gets or sets the data period current.
        /// </summary>
        /// <value>The data period current.</value>
        public string DataPeriodCurrent { get; set; } = default!;

        /// <summary>
        /// Gets or sets the external identifier.
        /// </summary>
        /// <value>The external identifier.</value>
        public string ExternalIdentifier { get; set; } = default!;

        /// <summary>
        /// Gets or sets a value indicating whether this instance is automated PDF
        /// enabled.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is automated PDF enabled; otherwise,
        /// <c>false</c>.
        /// </value>
        public bool IsAutomatedPdfEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is omni force reload.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is omni force reload; otherwise, <c>false</c>.
        /// </value>
        public bool IsOmniForceReload { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether [i is omni full redirect].
        /// </summary>
        /// <value>
        /// <c>true</c> if [i is omni full redirect]; otherwise, <c>false</c>.
        /// </value>
        public bool IsOmniFullRedirect { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is omni multi client.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is omni multi client; otherwise, <c>false</c>.
        /// </value>
        public bool IsOmniMultiClient { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is omni popup.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is omni popup; otherwise, <c>false</c>.
        /// </value>
        public bool IsOmniPopup { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is omni prefer minimized
        /// header.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is omni prefer minimized header; otherwise,
        /// <c>false</c>.
        /// </value>
        public bool IsOmniPreferMinimizedHeader { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is omni synchronize
        /// iframe source attribute.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is omni synchronize iframe source attribute;
        /// otherwise, <c>false</c>.
        /// </value>
        public bool IsOmniSyncIframeSrcAttr { get; set; }

        /// <summary>
        /// Gets or sets the large hero.
        /// </summary>
        /// <value>The large hero.</value>
        public string LargeHero { get; set; } = default!;

        /// <summary>
        /// Gets or sets the medium hero.
        /// </summary>
        /// <value>The medium hero.</value>
        public string MediumHero { get; set; } = default!;

        /// <summary>
        /// Gets or sets the omni group key default.
        /// </summary>
        /// <value>The omni group key default.</value>
        public string OmniGroupKeyDefault { get; set; } = default!;

        /// <summary>
        /// Gets or sets the omni iframe hide top px.
        /// </summary>
        /// <value>The omni iframe hide top px.</value>
        public long OmniIframeHideTopPx { get; set; }

        /// <summary>
        /// Gets or sets the omni querystring parameter expr.
        /// </summary>
        /// <value>The omni querystring parameter expr.</value>
        public string OmniQuerystringParamExpr { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the org.
        /// </summary>
        /// <value>The name of the org.</value>
        public string OrgName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the person identifier created.
        /// </summary>
        /// <value>The person identifier created.</value>
        public string PersonIdCreated { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the product.
        /// </summary>
        /// <value>The name of the product.</value>
        public string ProductName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>The project identifier.</value>
        public string ProjectId { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the project.
        /// </summary>
        /// <value>The name of the project.</value>
        public string ProjectName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the project status.
        /// </summary>
        /// <value>The project status.</value>
        public string ProjectStatus { get; set; } = default!;

        /// <summary>
        /// Gets or sets the release status identifier.
        /// </summary>
        /// <value>The release status identifier.</value>
        public long ReleaseStatusId { get; set; }

        /// <summary>
        /// Gets or sets the release status key.
        /// </summary>
        /// <value>The release status key.</value>
        public string ReleaseStatusKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the name of the release status.
        /// </summary>
        /// <value>The name of the release status.</value>
        public string ReleaseStatusName { get; set; } = default!;

        /// <summary>
        /// Gets or sets the small hero.
        /// </summary>
        /// <value>The small hero.</value>
        public string SmallHero { get; set; } = default!;

        /// <summary>
        /// Gets or sets the start when.
        /// </summary>
        /// <value>The start when.</value>
        public string StartWhen { get; set; } = default!;

        /// <summary>
        /// Gets or sets the type key.
        /// </summary>
        /// <value>The type key.</value>
        public string TypeKey { get; set; } = default!;

        /// <summary>
        /// Gets or sets the URL logo.
        /// </summary>
        /// <value>The URL logo.</value>
        public string UrlLogo { get; set; } = default!;

        /// <summary>
        /// Gets or sets the user roles.
        /// </summary>
        /// <value>The user roles.</value>
        public string[] UserRoles { get; set; } = default!;

        /// <summary>
        /// Gets or sets the x large hero.
        /// </summary>
        /// <value>The x large hero.</value>
        public string XLargeHero { get; set; } = default!;

        /// <summary>
        /// Gets or sets the x small hero.
        /// </summary>
        /// <value>The x small hero.</value>
        public string XSmallHero { get; set; } = default!;

        /*
        /// <summary>
        /// Gets or sets the project parent identifier.
        /// </summary>
        /// <value>The project parent identifier.</value>
        public string? ProjectParentId { get; set; }
        */

        /*
        /// <summary>
        /// Gets or sets the data refresh when.
        /// </summary>
        /// <value>The data refresh when.</value>
        public string? DataRefreshWhen { get; set; }
        */
    }
}