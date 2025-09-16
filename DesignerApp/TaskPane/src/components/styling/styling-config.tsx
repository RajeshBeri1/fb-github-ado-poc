import { Styling } from '@omniflow/omni-webapi';
import React, { KeyboardEvent, useState, useContext } from 'react';
import { useForm } from 'react-hook-form';
import { Icon } from '../../omni/icon';
import { Title } from '../form/title';
import { AlignmentConfig } from './alignment-config';
import { BorderConfig } from './border-config';
import { FontConfig } from './font-config';
import { Input } from '../form/input'
import { ClickableIcon } from '../utils/clickable-icon';
import { Tooltip } from "../../../src/omni/tooltip";
import { AppContext } from '../../taskpane/contexts/AppContext';
import { Icon as OmniIconElement } from '../../omni/icon'
import { ExcelRenderer } from '../../business/engine/renderer/excel-renderer';
import { ErrorMessage } from '../../components/form/error-message';
import { convertLetterToNumber } from '../../business/convert-letter-to-number';
import { precisionValue } from '../../enums/precision-value.enum';
import '../../pages/common-styles.css';
import { OmniDropDownInput } from '../../omni/dropdown';
import { OmniCheckBoxInput } from '../../omni/checkbox';
export enum EditSection {
    Alignment = 'Alignment',
    Font = 'Font',
    Border = 'Border',
    Fill = 'Fill',
    HeaderRowStyling = 'HeaderRowStyling',
}

interface IInputProps {
    styling: Styling,
    formKey?: string;
    showInherited?,
    onChange?: (attr, e) => void;
    onKeyUp?: (event: KeyboardEvent<HTMLInputElement>) => void;
    showPosition?: boolean;
}

export const StylingConfig: React.FC<IInputProps> = React.memo(({
    styling,
    formKey,
    showInherited = true,
    onChange,
    showPosition = false,
}) => {
    const [editSectionOpen, setEditSectionOpen] = useState<EditSection>(
        EditSection.Alignment
    );

    const [stylingState, setStyling] = useState(styling);
    const [isPrecisionValue, setIsPrecisionValue]= useState<any>()
    const [isMetric, setIsMetric] = useState(true);
    const [isNonMetric, setIsNonMetric] = useState(true);
    const {
        register,
        setValue,
        formState: { isDirty }, getValues } =
        useForm({
            mode: 'onChange',
            reValidateMode: 'onChange',
            defaultValues: {
                Styling: stylingState
            },
        });
    const resetToNull = (e) => {
        e.preventDefault();
        e.stopPropagation();

        const s = stylingState;
        s.Font.Background = null;
        s.Font.BackgroundAlternate = null;
        s.Font.Color = null;
        s.Font.ColorAlternate = null;
        s.Font.Family = null;
        s.Font.Size = null;
        s.Font.Weight = null;
        s.Font.Underline = null;
        setStyling(s);
        setValue('Styling.Font', s.Font);
    }
    const appContext = useContext(AppContext);
    const startPostionValue = styling?.Position?.Start;
    const isNotValidCellAddress = startPostionValue && !(startPostionValue || '').match(/^[A-Z]+\d+$/g);
    const locateCell = async () => {
        if (appContext.currentExcelContext) {
            const positions = (appContext?.currentThemeTemplateDetails?.Definition?.Styling?.Position?.Start || '').split(/^([A-Z]+)([0-9]+)$/);
            if (positions.length > 1) {
                const columnStart = convertLetterToNumber(positions[1]);
                const rowStart = parseInt(positions[2]) - 1;

                const excelRenderer = new ExcelRenderer();
                const worksheet = await excelRenderer.setupWorksheet(
                    appContext.currentExcelContext,
                )

                const cellToHighlight = worksheet.getCell(rowStart, columnStart);
                cellToHighlight.select();

                await appContext.currentExcelContext.sync();

            }
        }
    };
    const precisionValues = [
        {
            value: `${precisionValue.Actual}(15,354)`,
            id: precisionValue.Actual,
        },
        {
            value: `${precisionValue.Summarized}(15K)`,
            id: precisionValue.Summarized,
        },
        {
            value: `${precisionValue.Decimal}(15,354.67)`,
            id: precisionValue.Decimal,
        }
    ]

    const handlePrecisionChange = (e) => {
        onChange(formKey + '.Precision.PrecisionValue', e.detail.id);
        setValue("Styling.Precision.PrecisionValue", e.detail.id);
    }


    const handleShowBriefedCTC = (event) => {
        event.preventDefault();
        
        onChange(formKey + '.ShowBriefedCTC.ShowBriefedCTCValue', !getValues("Styling.ShowBriefedCTC.ShowBriefedCTCValue"));
        setValue("Styling.ShowBriefedCTC.ShowBriefedCTCValue", !getValues("Styling.ShowBriefedCTC.ShowBriefedCTCValue"));

    }
/*    const handleCustomStyleChange = (event) => {
        event.preventDefault();
        const keepManualFormattingClickValue = !getValues("Styling.CustomStyleSetting.CustomStyleSettingValue");
        onChange(formKey + '.CustomStyleSetting.CustomStyleSettingValue', keepManualFormattingClickValue);
        setValue("Styling.CustomStyleSetting.CustomStyleSettingValue", keepManualFormattingClickValue);
        appContext.setIsKeepManualFormatting(keepManualFormattingClickValue );
    }*/
    const handleIsMetric = (e) => {
        const isNonMetricChecked = getValues("Styling.Precision.ApplyToNonCurrencyMetric");
        const isMetricChecked = getValues("Styling.Precision.ApplyToCurrencyMetric");

        if (!isNonMetricChecked && isMetricChecked) {
            // Prevent unchecking ApplyToCurrencyMetric if ApplyToNonCurrencyMetric is unchecked
            e.preventDefault();
            return;
        }

        const newValue = !isMetricChecked;
        setValue("Styling.Precision.ApplyToCurrencyMetric", newValue);
        onChange?.(formKey + '.Precision.ApplyToCurrencyMetric', newValue);
    };
const handleIsNonMetric = (e) => {
    const isNonMetricChecked = getValues("Styling.Precision.ApplyToNonCurrencyMetric");
    const isMetricChecked = getValues("Styling.Precision.ApplyToCurrencyMetric");

    if (isNonMetricChecked && !isMetricChecked) {
        // Prevent unchecking ApplyToCurrencyMetric if ApplyToNonCurrencyMetric is unchecked
        e.preventDefault();
        return;
    }

    const newValue = !isNonMetricChecked;
    onChange?.(formKey + '.Precision.ApplyToNonCurrencyMetric', newValue);
    setValue("Styling.Precision.ApplyToNonCurrencyMetric", newValue);
        
    }
    return (

        <>
            {showPosition && (
                <>
                    <div className="d-flex">
                        <small className="text-md mr-2 ml-2">Start position</small>
                        <sup>
                            <Tooltip>
                                <Icon icon-id="omni:informative:info" slot="invoker" className="f-14"></Icon>
                                <div slot="content">The location the template will be rendered from</div>
                            </Tooltip>
                        </sup>

                    </div>
                    <div className="d-flex w-60 is-align-items-center">
                        <Input
                            label=""
                            labelStyle=""
                            onChange={(value) => {
                                onChange(formKey + '.Position.Start', value);

                            }}
                            value={appContext?.currentThemeTemplateDetails?.Definition?.Styling?.Position?.Start || ''}
                            breakLine

                        /><span className="ml-15pixel">
                            <ClickableIcon
                                iconId="omni:informative:target"
                                slot="invoker"
                                onClick={locateCell}
                                className="f-24"
                            />
                        </span>

                    </div>
                    {isNotValidCellAddress &&
                        <ErrorMessage message='Please enter a valid excel cell address.'></ErrorMessage>
                    }
                    <div className="d-flex is-align-items-center mt-2 mb-3">
                        <OmniDropDownInput label="Precision" className="w-53" hidefooter
                            options={precisionValues} value={[{ id: getValues("Styling.Precision.PrecisionValue"), value: precisionValues.find((x) => getValues("Styling.Precision.PrecisionValue") === x.id)?.value }]} onValueChange={handlePrecisionChange} >
                        </OmniDropDownInput>
                        {(getValues("Styling.Precision.PrecisionValue") === "Decimal" )&&
                            <>
                            <div className="mx-5">
                                <OmniCheckBoxInput checked={getValues("Styling.Precision.ApplyToCurrencyMetric")} onChange={(e) => handleIsMetric(e)} disabled={!getValues("Styling.Precision.ApplyToNonCurrencyMetric")}> <label className="mb-0 text-core-dark">Metric</label></OmniCheckBoxInput>
                            </div>
                            <div>
                                <OmniCheckBoxInput checked={getValues("Styling.Precision.ApplyToNonCurrencyMetric")} onChange={(e) => handleIsNonMetric(e)} disabled={!getValues("Styling.Precision.ApplyToCurrencyMetric")}> <label className="mb-0 text-core-dark">Non-Metric</label></OmniCheckBoxInput>
                            </div>
                            </>
                       
                        }
                    </div>
                    <div className="is-align-items-center mb-3">
                        <OmniCheckBoxInput checked={getValues("Styling.ShowBriefedCTC.ShowBriefedCTCValue")} onClick={(e) => handleShowBriefedCTC(e)} className="breifctc-checkbox"> <label className="mb-0 text-core-dark">Show Briefed CTC</label></OmniCheckBoxInput>
                    </div>
                    {/*<div className="is-flex-direction-row is-align-items-center">
                        <OmniCheckBoxInput checked={getValues("Styling.CustomStyleSetting.CustomStyleSettingValue")} onClick={(e) => handleCustomStyleChange(e)} className="keep-formatting"> <label className="text-core-dark">Keep manual formatting</label></OmniCheckBoxInput>
                        <p className="font-bl-12 pl-20pixel"> Any manual changes to the Excel file will be kept.Any un - rendered changes to the Excel file will be lost.</p>
                    </div>*/}
                </>
            )}
            <div className="d-flex flex-column theme-section-container p-3">
                <div
                    className="d-flex theme-section"
                    onClick={() =>
                        setEditSectionOpen(
                            editSectionOpen ===
                                EditSection.Alignment
                                ? null
                                : EditSection.Alignment
                        )
                    }>
                    <Icon
                        icon-id={"omni:interactive:" + (editSectionOpen === EditSection.Alignment ? 'up' : 'down')}
                        className="me-2"></Icon>
                    <p className="is-size-7 text-uppercase text-core-dark">Alignment</p>
                </div>
                <div className="d-flex theme-section">
                    {editSectionOpen ===
                        EditSection.Alignment ? (
                        <div className="d-flex w-100">
                            <AlignmentConfig
                                alignment={styling.Alignment}
                                formKey={
                                    'Alignment'
                                }
                                showInherited={showInherited}
                                onChange={(key, value) => {
                                    onChange(formKey + '.' + key, value);
                                }}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </div>
            </div>
            <div className="d-flex flex-column theme-section-container p-3 border-top">
                <div
                    className="d-flex theme-section"
                    onClick={() =>
                        setEditSectionOpen(
                            editSectionOpen === EditSection.Font
                                ? null
                                : EditSection.Font
                        )
                    }>
                    <Icon
                        icon-id={"omni:interactive:" + (editSectionOpen === EditSection.Font ? 'up' : 'down')}
                        className="me-2"></Icon>
                    <p className="is-size-7 text-uppercase text-core-dark">Font</p>
                    {/* <button onClick={(e) => resetToNull(e)}>Reset</button> */}
                </div>
                <div className="d-flex theme-section">
                    {editSectionOpen === EditSection.Font ? (
                        <div className="w-100">

                            <FontConfig
                                font={styling.Font}
                                formKey={
                                    'Font'
                                }
                                showInherited={showInherited}
                                onChange={(key, value) => {
                                    onChange(formKey + '.' + key, value);
                                }}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </div>
            </div>
            <div className="d-flex flex-column theme-section-container p-3 border-top">
                <div
                    className="d-flex theme-section"
                    onClick={() =>
                        setEditSectionOpen(
                            editSectionOpen ===
                                EditSection.Border
                                ? null
                                : EditSection.Border
                        )
                    }>
                    <Icon
                        icon-id={"omni:interactive:" + (editSectionOpen === EditSection.Border ? 'up' : 'down')}
                        className="me-2"></Icon>
                    <p className="is-size-7 text-uppercase text-core-dark">Border</p>
                </div>
                <div className="d-flex theme-section">
                    {editSectionOpen === EditSection.Border ? (
                        <div className="d-flex w-100 flex-wrap">
                            <BorderConfig
                                border={styling.Border}
                                formKey={
                                    'Border'
                                }
                                showInherited={showInherited}
                                onChange={(key, value) => {
                                    onChange(formKey + '.' + key, value);
                                }}
                            />
                        </div>
                    ) : (
                        <></>
                    )}
                </div>
            </div>
        </>
    );
});