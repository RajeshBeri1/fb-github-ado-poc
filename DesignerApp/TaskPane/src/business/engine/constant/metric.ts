export type TMetricColumn = {
    MetricColumnName: string;
    MetricTableId: string;
};

export const DefaultMetric: TMetricColumn = {
    MetricColumnName: 'netmedia',
    MetricTableId: '0d771867-cc9a-40a5-900e-f57528f91d0e',
};

export const DefaultMetricMediaBrief: TMetricColumn = {
    MetricColumnName: 'briefedctc',
    MetricTableId: 'e5cdf0ab-0455-43f7-953e-6b230513c3e6',
};

export const UsdMetrics: TMetricColumn[] = [
    {
        MetricColumnName: 'totalnet',
        MetricTableId: '0d771867-cc9a-40a5-900e-f57528f91d0e',
    },
    {
        MetricColumnName: 'grossmedia',
        MetricTableId: '0d771867-cc9a-40a5-900e-f57528f91d0e',
    },
];
export type DefaultMetrics = {
    Briefed: TMetricColumn;
    NonBriefed: TMetricColumn;
    UsdMetrics: TMetricColumn[];
}
