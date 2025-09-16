import { useState, useEffect, useContext } from 'react';
import { dataDictionaryApi, mediaopsToFlowChartApi } from '../lib/api';
import getColumnsFromDataDictionary, {
    TColumn,
} from '../lib/utils/getColumnsFromDataDictonary';
import { AppContext } from '../taskpane/contexts/AppContext';
import * as API from '@omniflow/omni-webapi';
import { plainToClass } from 'class-transformer';
import { MediaopsColumnSearchDTO, ColumnType } from '@omniflow/omni-webapi';
import metricFieldsFilteringListInfo from '../../assets/datadictionary/metricFieldsFilteringListInfo.json'
import stringFieldsFilteringListInfo from '../../assets/datadictionary/stringFieldsFilteringListInfo.json'

const useApiColumns = () => {
    const [isLoading, setIsLoading] = useState(false);
    const [columns, setColumns] = useState<TColumn[]>([]);
    const appContext = useContext(AppContext);

    useEffect(() => {
        let isSubscribed = true;
        setIsLoading(true);

        const loadDataDictionary = async () => {
            if (appContext.clientVersion > 1) {
                const { data } = await mediaopsToFlowChartApi.mediaopsToFlowChartGet(
                    plainToClass(API.MediaopsColumnSearchDTO, {
                        CreatedAt: null,
                        CreatedBy: null,
                        EndDate: null,
                        Id: null,
                        Name: null,
                        StartDate: null,
                        Status: "Active",
                        UpdatedAt: null,
                        UpdatedBy: null,
                        ClientId: appContext.flowchartTemplateDefinition.OmniClientId,
                    })
                );
                if (data && isSubscribed) {
                    const columns: TColumn[] = data.map(column => ({
                        DisplayName: column.DisplayName ?? column.ColumnName,
                        IsMetric: column.IsMetric,
                        IsCurrency: column.IsCurrency,
                        IsCommon: column.IsCommon,
                        TableName: column.TableName,
                        TableId: column.TableId,
                        Name: column.ColumnName,
                        Type: column.ColumnType as ColumnType
                    })).sort((a, b) => a.DisplayName.localeCompare(b.DisplayName))
                        // Sort columns by IsCommon
                        .sort((a, b) =>
                            a.IsCommon === b.IsCommon
                                ? 0
                                : a.IsCommon === true
                                    ? -1
                                    : 1
                        );
                    setColumns(columns);
                }
            }
            else {
                const { data } = await dataDictionaryApi.dataDictionaryGet(appContext.flowchartTemplateDefinition.OmniClientId);

                if (data && isSubscribed) {
                    setColumns(getColumnsFromDataDictionary(data).filter(x => x.TableName !== 'media_briefs' || appContext.mediaBriefValidColumns.includes(x.Name.toLowerCase())).filter(val => (val.IsMetric && metricFieldsFilteringListInfo.includes(val.Name.toLowerCase())) || (!val.IsMetric && stringFieldsFilteringListInfo.includes(val.Name.toLowerCase()))));
                }
            }

        };
        loadDataDictionary().catch(console.error);

        setIsLoading(false);
        return () => {
            isSubscribed = false;
        };
    }, []);

    return { columns, setColumns, isLoading };
};

export default useApiColumns;
