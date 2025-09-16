using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using Amazon.Athena.Model;
using AutoMapper;
using DocumentFormat.OpenXml.Office2010.Excel;
using Lib.Athena.Business;
using Lib.Common.Business;
using Lib.WebAPI.DbModels;
using Lib.WebAPI.DTOs;
using Lib.WebAPI.Models;
using Lib.WebAPI.Models.Flowchart;
using Lib.WebAPI.Models.Flowchart.Enumerations;

namespace Lib.WebAPI.Configuration
{
    /// <summary>
    /// AutomapperConfiguration
    /// </summary>
    [ExcludeFromCodeCoverage]
    public static class AutomapperConfiguration
    {
        /// <summary>
        /// Gets the mapper.
        /// </summary>
        public static IMapper GetMapper()
        {
            return new MapperConfiguration(config =>
            {
                config.CreateMap<FlowchartTemplate, FlowchartTemplateInfoDTO>();

                config.CreateMap<FlowchartTemplate, FlowchartTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.FlowchartDefinition) ?
                            null : Json.Deserialize<FlowchartDefinition>(s.FlowchartDefinition))
                    );

                config.CreateMap<FlowchartTemplateUpdateDTO, FlowchartTemplate>()
                    .ForMember(
                        d => d.FlowchartDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<User, UserDetailsDTO>();
                config.CreateMap<User, UserInfoDTO>();
                config.CreateMap<UserDetailsDTO, UserInfoDTO>();

                config.CreateMap<Exception, ExceptionDTO>()
                    .ForMember(
                        d => d.StackTrace,
                        o => o.MapFrom((s, d) => s.StackTrace?.Split(Environment.NewLine).Select(x => x.Trim()).ToArray())
                    );

                config.CreateMap<DataDictionaryTable, DataDictionaryTableDetailsDTO>()
                    .ForMember(
                        d => d.Columns,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.DataDictionaryColumns) ?
                            null : Json.Deserialize<Collection<DataDictionaryColumn>>(s.DataDictionaryColumns))
                    );

                config.CreateMap<DataDictionaryColumn, DataDictionaryColumnDetailsDTO>();

                config.CreateMap<CalendarTemplate, CalendarTemplateInfoDTO>();
                config.CreateMap<CalendarTemplate, CalendarTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.CalendarDefinition) ?
                            null : Json.Deserialize<CalendarDefinition>(s.CalendarDefinition))
                    );

                config.CreateMap<CalendarTemplateCreateDTO, CalendarTemplate>()
                    .ForMember(
                        d => d.CalendarDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<CalendarTemplateUpdateDTO, CalendarTemplate>()
                    .ForMember(
                        d => d.CalendarDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<MediaHierarchyTemplate, MediaHierarchyTemplateInfoDTO>();
                config.CreateMap<MediaHierarchyTemplate, MediaHierarchyTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.MediaHierarchyDefinition) ?
                            null : Json.Deserialize<MediaHierarchyDefinition>(s.MediaHierarchyDefinition))
                    ).ForMember(
                        d => d.Currency,
                        o => o.MapFrom((s, d) => s.Currency ?? Currency.LLL.ToString())
                    );

                config.CreateMap<MediaHierarchyTemplateCreateDTO, MediaHierarchyTemplate>()
                    .ForMember(
                        d => d.MediaHierarchyDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<MediaHierarchyTemplateUpdateDTO, MediaHierarchyTemplate>()
                    .ForMember(
                        d => d.MediaHierarchyDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<HeaderTemplate, HeaderTemplateInfoDTO>();
                config.CreateMap<HeaderTemplate, HeaderTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.HeaderDefinition) ?
                            null : Json.Deserialize<HeaderDefinition>(s.HeaderDefinition))
                    );

                config.CreateMap<HeaderTemplateCreateDTO, HeaderTemplate>()
                    .ForMember(
                        d => d.HeaderDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<HeaderTemplateUpdateDTO, HeaderTemplate>()
                    .ForMember(
                        d => d.HeaderDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<FooterTemplate, FooterTemplateInfoDTO>();
                config.CreateMap<FooterTemplate, FooterTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.FooterDefinition) ?
                            null : Json.Deserialize<FooterDefinition>(s.FooterDefinition))
                    );

                config.CreateMap<FooterTemplateCreateDTO, FooterTemplate>()
                    .ForMember(
                        d => d.FooterDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<FooterTemplateUpdateDTO, FooterTemplate>()
                    .ForMember(
                        d => d.FooterDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<ColumnInfo, DataDictionaryColumn>()
                    .ForMember(
                        d => d.Type,
                        o => o.MapFrom((s, d) => AthenaDataConverter.GetColumnType(s))
                    );

                config.CreateMap<GrandTotalTemplate, GrandTotalTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.GrandTotalDefinition) ?
                            null : Json.Deserialize<GrandTotalDefinition>(s.GrandTotalDefinition))
                    );

                config.CreateMap<GrandTotalTemplate, GrandTotalTemplateInfoDTO>();
                config.CreateMap<GrandTotalTemplateCreateDTO, GrandTotalTemplate>()
                    .ForMember(
                        d => d.GrandTotalDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<GrandTotalTemplateUpdateDTO, GrandTotalTemplate>()
                    .ForMember(
                        d => d.GrandTotalDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<ThemeTemplateCreateDTO, ThemeTemplate>()
                    .ForMember(
                        d => d.ThemeDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<ThemeTemplate, ThemeTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.ThemeDefinition) ?
                            null : Json.Deserialize<ThemeDefinition>(s.ThemeDefinition))
                    );

                config.CreateMap<ThemeTemplate, ThemeTemplateInfoDTO>();
                config.CreateMap<ThemeTemplateUpdateDTO, ThemeTemplate>()
                    .ForMember(
                        d => d.ThemeDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<TotalsTemplateCreateDTO, TotalsTemplate>()
                    .ForMember(
                        d => d.TotalsDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<TotalsTemplate, TotalsTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.TotalsDefinition) ?
                            null : Json.Deserialize<TotalsDefinition>(s.TotalsDefinition))
                    );

                config.CreateMap<TotalsTemplate, TotalsTemplateInfoDTO>();
                config.CreateMap<TotalsTemplateUpdateDTO, TotalsTemplate>()
                    .ForMember(
                        d => d.TotalsDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<CalendarOverlayTemplateCreateDTO, CalendarOverlayTemplate>()
                    .ForMember(
                        d => d.CalendarOverlayDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<CalendarOverlayTemplate, CalendarOverlayTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.CalendarOverlayDefinition) ?
                            null : Json.Deserialize<CalendarOverlayDefinition>(s.CalendarOverlayDefinition))
                    );

                config.CreateMap<CalendarOverlayTemplate, CalendarOverlayTemplateInfoDTO>();
                config.CreateMap<CalendarOverlayTemplateUpdateDTO, CalendarOverlayTemplate>()
                    .ForMember(
                        d => d.CalendarOverlayDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<RunConfigurationCreateDTO, RunConfiguration>()
                    .ForMember(
                        d => d.RunRestrictions,
                        o => o.MapFrom((s, d) => s.RunRestrictions == null ? null : Json.Serialize(s.RunRestrictions))
                    )
                     .ForMember(
                        d => d.CalendarDefinition,
                        o => o.MapFrom((s, d) => s.CalendarDefinition == null ? null : Json.Serialize(s.CalendarDefinition))
                    )
                    .ForMember(
                        d => d.MediaHierarchyLevels,
                        o => o.MapFrom((s, d) => s.MediaHierarchyLevels == null ? null : Json.Serialize(s.MediaHierarchyLevels))
                    );

                config.CreateMap<RunConfiguration, RunConfigurationInfoDTO>();
                config.CreateMap<RunConfiguration, RunConfigurationDetailsDTO>()
                    .ForMember(
                        d => d.RunResctrictions,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.RunRestrictions) ?
                            null : Json.Deserialize<Collection<RunRestriction>>(s.RunRestrictions))
                    )
                    .ForMember(
                        d => d.CalendarDefinition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.CalendarDefinition) ?
                            null : Json.Deserialize<CalendarTemplateDetailsDTO>(s.CalendarDefinition))
                    )
                    .ForMember(
                        d => d.MediaHierarchyLevels,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.MediaHierarchyLevels) ?
                            null : Json.Deserialize<Collection<MediaHierarchyLevelBase>>(s.MediaHierarchyLevels))
                    );

                config.CreateMap<RunConfigurationUpdateDTO, RunConfiguration>()
                    .ForMember(
                        d => d.RunRestrictions,
                        o => o.MapFrom((s, d) => s.RunRestrictions == null ? null : Json.Serialize(s.RunRestrictions))
                    )
                    .ForMember(
                        d => d.CalendarDefinition,
                        o => o.MapFrom((s, d) => s.CalendarDefinition == null ? null : Json.Serialize(s.CalendarDefinition))
                    )
                    .ForMember(
                        d => d.MediaHierarchyLevels,
                        o => o.MapFrom((s, d) => s.MediaHierarchyLevels == null ? null : Json.Serialize(s.MediaHierarchyLevels))
                    );

                config.CreateMap<MediaHierarchyDefinitionDetailsDTO, MediaHierarchyTemplate>()
                    .ForMember(
                        d => d.MediaHierarchyDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                     );
                config.CreateMap<MediaHierarchyTemplate, MediaHierarchyDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.MediaHierarchyDefinition) ?
                            null : Json.Deserialize<MediaHierarchyDefinition>(s.MediaHierarchyDefinition))
                    );

                config.CreateMap<GrandTotalDefintionDetailsDTO, GrandTotalTemplate>()
                    .ForMember(
                        d => d.GrandTotalDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                     );
                config.CreateMap<GrandTotalTemplate, GrandTotalDefintionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.GrandTotalDefinition) ?
                            null : Json.Deserialize<GrandTotalDefinition>(s.GrandTotalDefinition))
                    );

                config.CreateMap<RightHandTotalsDefinitionDetailsDTO, TotalsTemplate>()
                    .ForMember(
                        d => d.TotalsDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                     );
                config.CreateMap<TotalsTemplate, RightHandTotalsDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.TotalsDefinition) ?
                            null : Json.Deserialize<TotalsDefinition>(s.TotalsDefinition))
                    );

                config.CreateMap<HeaderDefinitionDetailsDTO, HeaderTemplate>()
                    .ForMember(
                        d => d.HeaderDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                     );
                config.CreateMap<HeaderTemplate, HeaderDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.HeaderDefinition) ?
                            null : Json.Deserialize<HeaderDefinition>(s.HeaderDefinition))
                    );

                config.CreateMap<FlowchartDefinitionDetailsDTO, FlowchartTemplate>()
                    .ForMember(
                        d => d.FlowchartDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                     );
                config.CreateMap<FlowchartTemplate, FlowchartDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.FlowchartDefinition) ?
                            null : Json.Deserialize<FlowchartDefinition>(s.FlowchartDefinition))
                    );

                config.CreateMap<FlowchartTemplatesVersionHistortiesDefinitionDetailsDTO, FlowchartTemplatesVersionHistorty>()
                    .ForMember(
                        d => d.FlowchartDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );
                config.CreateMap<FlowchartTemplatesVersionHistorty, FlowchartTemplatesVersionHistortiesDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.FlowchartDefinition) ?
                            null : Json.Deserialize<FlowchartDefinition>(s.FlowchartDefinition))
                    );

                config.CreateMap<RunConfigurationDefinitionDetailsDTO, RunConfiguration>()
                    .ForMember(
                        d => d.RunRestrictions,
                        o => o.MapFrom((s, d) => s.RunRestrictions == null ? null : Json.Serialize(s.RunRestrictions))
                    )
                    .ForMember(
                        d => d.MediaHierarchyLevels,
                        o => o.MapFrom((s, d) => s.MediaHierarchyLevels == null ? null : Json.Serialize(s.MediaHierarchyLevels))
                    );
                config.CreateMap<RunConfiguration, RunConfigurationDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.RunRestrictions,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.RunRestrictions) ?
                            null : Json.Deserialize<Collection<RunRestriction>>(s.RunRestrictions))
                    )
                    .ForMember(
                        d => d.MediaHierarchyLevels,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.MediaHierarchyLevels) ?
                            null : Json.Deserialize<Collection<MediaHierarchyLevelBase>>(s.MediaHierarchyLevels))
                    );

                config.CreateMap<ThemeDefinitionDetailsDTO, ThemeTemplate>()
                    .ForMember(
                        d => d.ThemeDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                     );
                config.CreateMap<ThemeTemplate, ThemeDefinitionDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.ThemeDefinition) ?
                            null : Json.Deserialize<ThemeDefinition>(s.ThemeDefinition))
                    );

                config.CreateMap<Report, ReportInfoDTO>();

                config.CreateMap<SummaryTemplateCreateDTO, SummaryTemplate>()
                    .ForMember(
                        d => d.SummaryDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<SummaryTemplate, SummaryTemplateDetailsDTO>()
                    .ForMember(
                        d => d.Definition,
                        o => o.MapFrom((s, d) => string.IsNullOrWhiteSpace(s.SummaryDefinition) ?
                            null : Json.Deserialize<SummaryDefinition>(s.SummaryDefinition))
                    );

                config.CreateMap<SummaryTemplate, SummaryTemplateInfoDTO>();
                config.CreateMap<SummaryTemplateUpdateDTO, SummaryTemplate>()
                    .ForMember(
                        d => d.SummaryDefinition,
                        o => o.MapFrom((s, d) => s.Definition == null ? null : Json.Serialize(s.Definition))
                    );

                config.CreateMap<MediaHierarchyLevelBase, MediaHierarchyLevel>();
                config.CreateMap<MediaHierarchySettingBase, MediaHierarchySetting>();
                config.CreateMap<MediaHierarchySubLevelBase, MediaHierarchySubLevel>();
                config.CreateMap<MediaHierarchySubLevelSettingBase, MediaHierarchySubLevelSetting>();
                config.CreateMap<MediaHierarchySubTotalBase, MediaHierarchySubTotal>();
                config.CreateMap<MediaHierarchyInflightOverlayBase, MediaHierarchyInflightOverlay>();
                config.CreateMap<OmniClient, OmniClientInfoDTO>();

                config.CreateMap<Client, ClientInfoDTO>();
                config.CreateMap<Client, ClientDTO>();
                config.CreateMap<FieldInfo, FieldInfoDto>();
                config.CreateMap<FieldInfoDto, FieldInfo>().ForMember(
                        d => d.Id,
                        o => o.Ignore());
                config.CreateMap<PlannedMediaMigrationColumnMapping, CreatePlannedMediaMigrationDTO>();
                config.CreateMap<CreatePlannedMediaMigrationDTO, PlannedMediaMigrationColumnMapping>().ForMember(
                        d => d.Id,
                        o => o.Ignore());
                config.CreateMap<DefaultTemplate, DefaultTemplateDTO>();
                config.CreateMap<DefaultTemplateDTO, DefaultTemplate>().ForMember(
                        d => d.Id,
                        o => o.Ignore());
                config.CreateMap<ClientMapping, ClientMappingCreateDTO>();
                config.CreateMap<ClientMappingCreateDTO, ClientMapping>().ForMember(
                        d => d.Id,
                        o => o.Ignore());
                config.CreateMap<MediaHierarchySubTotalSummaryBase, MediaHierarchySubTotalSummary>();
            }).CreateMapper();
        }
    }
}