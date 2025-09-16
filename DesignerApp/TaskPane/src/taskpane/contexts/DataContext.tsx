import React, { createContext, JSX, PropsWithChildren, useState } from 'react';
import * as API from '@omniflow/omni-webapi';

import { THeaderRowData } from '../../business/engine/helpers/header-helper';
import { TFlowchartDataLevelFlat } from '../../business/engine/helpers/level-helper';

export type TDataContextValue = {
    currentHeaderRowData: THeaderRowData;
    setCurrentHeaderRowData: React.Dispatch<
        React.SetStateAction<THeaderRowData>
    >;
    currentCalendarData: any[];
    setCurrentCalendarData: React.Dispatch<React.SetStateAction<any[]>>;
    currentLevelFlowchartData: API.FlowchartData;
    setCurrentLevelFlowchartData: React.Dispatch<
        React.SetStateAction<API.FlowchartData>
    >;
    currentFlatLevelData: TFlowchartDataLevelFlat[];
    setCurrentFlatLevelData: React.Dispatch<
        React.SetStateAction<TFlowchartDataLevelFlat[]>
    >;
    briefedctcColumnSelected: boolean;
    setBriefedctcColumnSelected: React.Dispatch<
        React.SetStateAction<boolean>>;
    isGlobalRendering: boolean;
    setIsGlobalRendering: React.Dispatch<React.SetStateAction<boolean>>;
};

export const DataContext = createContext<TDataContextValue>({
    currentHeaderRowData: null,
    setCurrentHeaderRowData: (value: THeaderRowData): void => undefined,
    currentCalendarData: null,
    setCurrentCalendarData: (value: any[]): void => undefined,
    currentLevelFlowchartData: null,
    setCurrentLevelFlowchartData: (value: API.FlowchartData): void => undefined,
    currentFlatLevelData: null,
    setCurrentFlatLevelData: (value: TFlowchartDataLevelFlat[]): void =>
        undefined,
    briefedctcColumnSelected: false,
    setBriefedctcColumnSelected: (value: boolean): void => undefined,
    isGlobalRendering: false,
    setIsGlobalRendering: (value: boolean): void => undefined,
});

export type TDataContextProviderProps = PropsWithChildren<{}>;

function DataContextProvider({
    children,
}: TDataContextProviderProps): JSX.Element {
    const [currentHeaderRowData, setCurrentHeaderRowData] =
        useState<THeaderRowData>(null);
    const [currentCalendarData, setCurrentCalendarData] = useState<any[]>(null);
    const [currentLevelFlowchartData, setCurrentLevelFlowchartData] =
        useState<API.FlowchartData>(null);
    const [currentFlatLevelData, setCurrentFlatLevelData] =
        useState<TFlowchartDataLevelFlat[]>(null);
    const [briefedctcColumnSelected, setBriefedctcColumnSelected] = useState<boolean>(currentFlatLevelData && currentFlatLevelData.findIndex(l => l.ColumnName.toLowerCase() == 'channel') > -1);
    const [isGlobalRendering, setIsGlobalRendering] = useState<boolean>(false);

    return (
        <DataContext.Provider
            value={{
                currentHeaderRowData,
                setCurrentHeaderRowData,
                currentCalendarData,
                setCurrentCalendarData,
                currentLevelFlowchartData,
                setCurrentLevelFlowchartData,
                currentFlatLevelData,
                setCurrentFlatLevelData,
                briefedctcColumnSelected,
                setBriefedctcColumnSelected,
                isGlobalRendering,
                setIsGlobalRendering
            }}>
            {children}
        </DataContext.Provider>
    );
}

if (process.env.NODE_ENV !== 'production') {
    DataContext.displayName = 'DataContext';
}

export default DataContextProvider;
