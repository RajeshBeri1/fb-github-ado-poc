import { findIndex, isEmpty, isEqual, range, uniq } from 'lodash';

import { RenderEngine } from '../models/render-engine';
import { AddressConverter } from '../../address-converter';
import { Block } from '../models/block';
import { Styles } from '../models/styles';
import Tools from '../../tools';
import SpacerBlock from '../blocks/calendar/spacer-block';
import FlightBarWallSubTotalBlock from '../blocks/media-hierarchy/flight-bar-wall-sub-total-block';
import ValueBlock from '../blocks/right-hand-totals/value-block';
import FlightBarWallBarBlock from '../blocks/media-hierarchy/flight-bar-wall-bar-block';
import FlightBarWallInflightOverlayBlock from '../blocks/media-hierarchy/flight-bar-wall-inflight-overlay-block';
import TotalBlock from '../blocks/right-hand-totals/total-block';
import GTValueBlock from '../blocks/grand-totals/value-block';
export type CombinedStyle = {
    styles: Styles;
    ranges: string[];
};

export class ExcelRenderer extends RenderEngine {
    ac = new AddressConverter();
    isGettingRendered: boolean = false;

    setupWorksheet = async (
        context: Excel.RequestContext,
        name: string = 'Report',
        deleteSheet: boolean = false
    ): Promise<Excel.Worksheet> => {
        let app = context.workbook.application;
        let sheet = context.workbook.worksheets.getItemOrNullObject(name);

        await context.sync();

        if (sheet.isNullObject) {
            sheet = context.workbook.worksheets.add(name);
        }

        if (deleteSheet) {
            sheet.delete();
            sheet = context.workbook.worksheets.add(name);
        }

        await context.sync();

        sheet.activate();
        app.suspendScreenUpdatingUntilNextSync();

        return sheet;
    };

    setupStylesheet = async (
        context: Excel.RequestContext,
        name: string = 'Styles'
    ): Promise<Excel.Worksheet> => {
        let app = context.workbook.application;
        let sheet = context.workbook.worksheets.getItemOrNullObject(name);

        await context.sync();

        if (sheet.isNullObject) {
            sheet = context.workbook.worksheets.add(name);
        }

        sheet.visibility = Excel.SheetVisibility.hidden;

        app.suspendScreenUpdatingUntilNextSync();

        return sheet;
    };

    render = async (
        blockRows: Block[][],
        {
            name,
            levelName,
            columnStart = 0,
            rowStart = 0,
            deleteSheet,
            weekly,
            precisedValue,
            isDecimal,
            setCurrentExcelContext,
            onWorkSheetFormatChanged
        }: {
            name?: string;
            levelName?: string;
            columnStart?: number;
            rowStart?: number;
            deleteSheet?: boolean;
            weekly?: boolean;
            precisedValue?: boolean;
            isDecimal?: boolean;
            setCurrentExcelContext?: (x: Excel.RequestContext) => void;
            onWorkSheetFormatChanged?: (event: Excel.WorksheetFormatChangedEventArgs) => Promise<any>;
        } = {}
    ) => {
        const start = Date.now();

        if (this.isGettingRendered) {
            return;
        }

        const value = sessionStorage.getItem('isGettingRendered');
        if (value && value === "true") {
            return;
        }

        this.isGettingRendered = true;

        try {
            let combinedStyles: CombinedStyle[] = [];
            let combinedValueStyles: CombinedStyle[] = [];
            let mergeRanges = [];
            await Excel.run(async (context) => {
                const numberFormatRanges = new Map<string, { format: string, length: number }>();
                const worksheet = await this.setupWorksheet(
                    context,
                    name,
                    deleteSheet
                );
                worksheet.onFormatChanged.remove(onWorkSheetFormatChanged);
                const stylesheet = await this.setupStylesheet(context);

                let imageData = [];
                let currentRow = rowStart;
                const currentColumnRow: {
                    [key: string]: { value: number; touched: boolean };
                } = {};
                blockRows.map((rows, rowIndex) => {
                    currentRow += 1;

                    let currentColumn = columnStart;
                    rows.map(async (block, columnIndex) => {
                        const {
                            columnStart: blockColumnStart,
                            rowStart: blockRowStart,
                            columnLength,
                            rowLength,
                            columnAfter,
                            rowAfter,
                            merge,
                            containsImage,
                            imageMaxHeight,
                            autofitColumns,
                        } = block;
                        currentColumn += 1;

                        // Set current column row index
                        const currentColumnRowIndex =
                            currentColumn + blockColumnStart;

                        // Set minimum column row
                        let minColumnRow;
                        Array.from({ length: columnLength }).forEach(
                            (_c, columnIndex) => {
                                const finalColumnRowIndex =
                                    currentColumnRowIndex + columnIndex;

                                if (
                                    currentColumnRow[finalColumnRowIndex] !==
                                    undefined &&
                                    (minColumnRow === undefined ||
                                        currentColumnRow[finalColumnRowIndex]
                                            .value > minColumnRow)
                                ) {
                                    minColumnRow =
                                        currentColumnRow[finalColumnRowIndex].value;
                                }
                            }
                        );

                        // Set current column row start
                        Array.from({ length: columnLength }).forEach(
                            (_c, columnIndex) => {
                                const finalColumnRowIndex =
                                    currentColumnRowIndex + columnIndex;

                                if (
                                    currentColumnRow[finalColumnRowIndex] ===
                                    undefined
                                ) {
                                    currentColumnRow[finalColumnRowIndex] = {
                                        value:
                                            minColumnRow !== undefined
                                                ? minColumnRow + blockRowStart
                                                : blockRowStart,
                                        touched: true,
                                    };
                                } else {
                                    if (minColumnRow !== undefined) {
                                        currentColumnRow[
                                            finalColumnRowIndex
                                        ].value = minColumnRow;
                                    }
                                    currentColumnRow[finalColumnRowIndex].value +=
                                        blockRowStart;
                                    currentColumnRow[finalColumnRowIndex].touched =
                                        true;
                                }
                            }
                        );

                        // Get range address
                        const { text: rangeStart } = this.ac.getAddressFromCoords(
                            currentColumn + blockColumnStart,
                            currentRow +
                            currentColumnRow[currentColumnRowIndex].value
                        );
                        currentColumn += blockColumnStart + columnLength - 1;
                        const { text: rangeEnd } = this.ac.getAddressFromCoords(
                            currentColumn,
                            currentRow +
                            currentColumnRow[currentColumnRowIndex].value +
                            rowLength -
                            1
                        );
                        const rangeAddress = `${rangeStart}:${rangeEnd}`;

                        // Get value data
                        // Todo SG: Check isSame
                        const { isEmpty, isSame, values } =
                            this.getValueData(block);

                        // Get styles
                        // Todo SG: Check style implementation

                        // Merge style from general to specific definition.
                        const generalStyle = Tools.deepCopy(
                            block.getGeneralStyles()
                        );
                        const componentStyle = Tools.deepCopy(
                            block.getComponentStyles()
                        );
                        const directStyles = Tools.deepCopy(
                            block.getDirectStyles()
                        );
                        const legendStyles = Tools.deepCopy(
                            block.getLegendStyles()
                        );

                        if (!(block instanceof SpacerBlock)) {
                            let mergedStyle = {};

                            mergedStyle = Tools.mergeObjects(
                                generalStyle,
                                componentStyle
                            );

                            mergedStyle = Tools.mergeObjects(
                                mergedStyle,
                                legendStyles
                            );

                            mergedStyle = Tools.mergeObjects(
                                mergedStyle,
                                directStyles
                            );
                            const metricBlock = (block instanceof FlightBarWallSubTotalBlock) || (block instanceof ValueBlock) || (block instanceof FlightBarWallBarBlock);
                            let styles = precisedValue || isDecimal || !metricBlock ? block.getStyles() : {
                                ...block.getStyles(),
                                columnWidth: block instanceof ValueBlock ?
                                    Math.max(
                                        block.value?.toString().length * 12,  // Increased multiplier for better spacing
                                        (block.getValue()?.toString() || '').length * 12  // Account for actual values
                                    ) : 50
                            };
                            mergedStyle = Tools.mergeObjects(
                                mergedStyle,
                                styles,
                            );

                            block.styles = mergedStyle;
                        }


                        // commented as casuing issue in formatting
                        if (!isEmpty) {
                            combinedValueStyles = this.combineStyles(
                                block,
                                rangeAddress,
                                combinedValueStyles
                            );
                        } else {
                            combinedStyles = this.combineStyles(
                                block,
                                rangeAddress,
                                combinedStyles
                            );
                        }

                        // Mark block for merge if needed
                        if (merge && (columnLength > 1 || rowLength > 1)) {
                            mergeRanges.push(rangeAddress);
                        }

                        // Only render values, if not empty
                        if (!isEmpty) {
                            // Create range
                            let range = worksheet.getRange(rangeAddress);

                            // Set values
                            if (!containsImage) {
                                range.values = values;
                                const isMetricBlock = block instanceof FlightBarWallSubTotalBlock
                                    || block instanceof ValueBlock
                                    || block instanceof FlightBarWallBarBlock
                                    || block instanceof FlightBarWallInflightOverlayBlock
                                    || block instanceof TotalBlock
                                    || block instanceof GTValueBlock;
                                if (isMetricBlock && values.length > 0 && values[0].length > 0) {
                                    try {
                                        const inferredFormat = this.inferExcelNumberFormat(values[0][0]);
                                        numberFormatRanges.set(rangeAddress, { format: inferredFormat, length: values.length });

                                    } catch (e) {
                                        console.log('error while fetching the formatting')
                                    }

                                }
                            } else {
                                //passing of the image content into an object
                                const { content, width, height } = JSON.parse(values[0][0]);
                                const scaledWidth = width * (1 + 1 / 100);
                                const scaledHeight = height * (1 + 1 / 100);
                                //to scale after the image is drawn
                                const img = worksheet.shapes.addImage(content);
                                img.width = scaledWidth;
                                img.height = scaledHeight;
                                const { text: imageRangeEnd } =
                                    // Todo SG + ZZ: Adapt ranges (assume that it no longer works correctly with my update)
                                    this.ac.getAddressFromCoords(
                                        currentColumn,
                                        currentRow +
                                        blockRowStart +
                                        imageMaxHeight -
                                        1
                                    );

                                const imageRangeAddress = `${rangeStart}:${imageRangeEnd}`;
                                img.load();

                                imageData.push({
                                    rangeAddress: imageRangeAddress,
                                    img,
                                });

                            }
                            // Auto fit columns
                            autofitColumns && range.format.autofitColumns();
                            range.format.autofitRows();
                            range.untrack();
                        }

                        // Update current column and current column row
                        currentColumn += columnAfter;
                        currentColumnRow[currentColumnRowIndex].value +=
                            rowLength - 1 + rowAfter;
                    });

                    // Correct untouched columnRow values
                    Object.entries(currentColumnRow).forEach(([, columnRow]) => {
                        if (!columnRow.touched) {
                            columnRow.value -= 1;
                            if (columnRow.value < 0) columnRow.value = 0;
                        } else {
                            columnRow.touched = false;
                        }
                    });
                });

                combinedStyles.forEach(({ styles, ranges }) => {
                    if (ranges.length) {
                        const firstRange = `A1:A1`;
                        let range = stylesheet.getRange(firstRange);

                        range.delete(Excel.DeleteShiftDirection.up);
                        range = stylesheet.getRange(firstRange);
                        range.setCellProperties([[{ format: { ...styles } }]]);

                        const targetRanges = worksheet.getRanges(ranges.join(','));
                        targetRanges.copyFrom(
                            `Styles!${firstRange}`,
                            Excel.RangeCopyType.formats
                        );

                        targetRanges.format.columnWidth = styles?.columnWidth;
                        targetRanges.format.rowHeight = styles?.rowHeight;
                        targetRanges.format.wrapText = styles?.wrapText;

                        // Add untrack for the combined target ranges
                        targetRanges.untrack();
                        range.untrack();
                    }
                });

                combinedValueStyles.forEach(({ styles, ranges }) => {
                    if (ranges.length) {
                        const firstRange = `A1:A1`;
                        let range = stylesheet.getRange(firstRange);

                        range.delete(Excel.DeleteShiftDirection.up);
                        range = stylesheet.getRange(firstRange);
                        range.setCellProperties([[{ format: { ...styles } }]]);

                        const targetRanges = worksheet.getRanges(ranges.join(','));
                        targetRanges.copyFrom(
                            `Styles!${firstRange}`,
                            Excel.RangeCopyType.formats
                        );

                        targetRanges.format.columnWidth = styles?.columnWidth;
                        targetRanges.format.rowHeight = styles?.rowHeight;
                        targetRanges.format.wrapText = styles?.wrapText;

                        // Add untrack for the combined target ranges
                        targetRanges.untrack();
                        range.untrack();
                    }
                });

                // Apply merges
                mergeRanges.forEach((rangeAddress) => {
                    const range = worksheet.getRange(rangeAddress);
                    range.merge(false);
                    range.untrack();
                });

                // Prepare number format operations
                //const numberFormatOperations: { range: Excel.Range; format: string }[] = [];
                numberFormatRanges.forEach((format, rangeAddress) => {
                    const range = worksheet.getRange(rangeAddress);
                    const formats: string[][] = Array(1).fill(null).map(() =>
                        Array(format.length).fill(format.format)
                    );
                    range.numberFormat = formats;
                    range.untrack();
                });


                // 1. get the image range position, now that all rows and columns have their final size
                const images = imageData.map(({ img, rangeAddress }) => {
                    const imageRange = worksheet.getRange(rangeAddress);
                    imageRange.load();
                    return { img: img as Excel.Shape, range: imageRange as Excel.Range, defaultWidthPercentage: 4, defaultHeightPercentage: 4 };
                });
                // 2. ensure that (range)data can be accessed
                await context.sync();

                // 3. scale and draw all images
                this.drawImages(images);

                const end = Date.now();
                // console.log(`EXCEL RENDER DURATION: ${end - start}ms`);
                setCurrentExcelContext && setCurrentExcelContext(context);
            });
        } catch (e) {
            console.log('Rendering session:', e);
        } finally {
            this.isGettingRendered = false;
            sessionStorage.setItem('isGettingRendered', "false");
        }
    };

    merge = (dst, src) => {
        Object.keys(src).forEach((key) => {
            if (!dst[key]) {
                dst[key] = src[key];
            } else if (
                typeof src[key] === 'object' &&
                src[key] !== null &&
                typeof dst[key] === 'object' &&
                dst[key] !== null
            ) {
                this.merge(dst[key], src[key]);
            }
        });
    };

    drawImages = async (
        images: {
            img: Excel.Shape;
            range: Excel.Range;
            defaultWidthPercentage: number;
            defaultHeightPercentage: number;
        }[]
    ) => {
        images.forEach(async ({ img, range, defaultWidthPercentage, defaultHeightPercentage }, index) => {
            //Interpretation of input 1 to 1% of image width and height increase 
            const scaleFactor = Math.trunc(9 + defaultWidthPercentage / 100);

            // Calculate scaled width and height
            const truncatedWidth = Math.floor(img.width);
            const scaledWidth = Math.floor(truncatedWidth * scaleFactor - 4) || defaultWidthPercentage;
            const truncatedHeight = Math.floor(img.height);
            const scaledHeight = Math.floor(truncatedHeight * scaleFactor - 4) || defaultHeightPercentage;

            // Set position for the image
            const left = range.left;
            const top = range.top;

            img.set({
                name: `image${index}`,
                left: left,
                top: top,
                width: scaledWidth,
                height: scaledHeight,
            });
        });
    };

    combineStyles = (
        block: Block,
        range: string,
        combinedStyles: CombinedStyle[] = []
    ): CombinedStyle[] => {
        const finalCombinedStyles = [...combinedStyles];

        const address = this.ac.getRangeFromString(range);
        const coords = this.ac.getCoordsFromAddress(
            address.start.column,
            address.start.row
        );

        this.getStyles(block).forEach((rows, rowIndex) => {
            // Check if all styles of the block are the same
            let isSame = true;
            let lastStyles = null;
            rows.forEach((styles) => {
                if (isSame) {
                    if (lastStyles && !isEqual(lastStyles, styles))
                        isSame = false;

                    lastStyles = styles;
                }
            });

            // If all styles of the block are the same, combine style for complete range
            if (isSame) {
                // modified find index condition for FB-291 
                let stylesIndex = findIndex(finalCombinedStyles, (combinedStyles) =>
                    combinedStyles.styles == lastStyles
                );
                if (stylesIndex === -1) {
                    finalCombinedStyles.push({
                        styles: lastStyles,
                        ranges: [range],
                    });
                    stylesIndex = finalCombinedStyles.length - 1;
                } else {
                    finalCombinedStyles[stylesIndex].ranges.push(range);
                }

                finalCombinedStyles[stylesIndex].ranges = uniq(
                    finalCombinedStyles[stylesIndex].ranges
                );
            }
            // Otherwise, combine style for each column of the range separately
            else {
                rows.forEach((styles, columnIndex) => {
                    const { x, y } = { x: coords.x + columnIndex, y: coords.y };
                    const { text: columnAddress } =
                        this.ac.getAddressFromCoords(x, y);
                    const columnRange = `${columnAddress}:${columnAddress}`;

                    let stylesIndex = findIndex(finalCombinedStyles, {
                        styles,
                    });
                    if (stylesIndex === -1) {
                        finalCombinedStyles.push({
                            styles,
                            ranges: [columnRange],
                        });
                        stylesIndex = finalCombinedStyles.length - 1;
                    } else {
                        finalCombinedStyles[stylesIndex].ranges.push(
                            columnRange
                        );
                    }

                    // finalCombinedStyles[stylesIndex].ranges = uniq(
                    //     finalCombinedStyles[stylesIndex].ranges
                    // );
                });
            }
        });

        return finalCombinedStyles;
    };



    getAppliedStyles = async (savedStyles: Array<{ address: string, format: Excel.RangeFormat }>) => {
        // Create an array to store cell formats
        const cellFormats: Array<{ address: string, format: Excel.RangeFormat }> = savedStyles;


        const value = sessionStorage.getItem('isGettingRendered');


        this.isGettingRendered = true;

        try {


        } catch (e) {
            console.log('Rendering session:', e);
        } finally {
            this.isGettingRendered = false;
            sessionStorage.setItem('isGettingRendered', "false");
        }
        return cellFormats;

    };
    applySavedStyles = async (savedStyles: Array<{ address: string, format: Excel.RangeFormat }>) => {
        // apply styles to the sheet
        try {

            await Excel.run(async (context) => {
                let sheet = context.workbook.worksheets.getItemOrNullObject("Report");
                let app = context.workbook.application;
                await context.sync();

                if (!sheet.isNullObject) {
                    const usedRange = sheet.getUsedRangeOrNullObject();
                    await context.sync();

                    if (!usedRange.isNullObject) {
                        usedRange.load("address, rowCount, columnCount");
                        await context.sync();

                        const rowCount = usedRange.rowCount;
                        const columnCount = usedRange.columnCount;

                        const cells = [];
                        for (let i = 0; i < rowCount; i++) {
                            for (let j = 0; j < columnCount; j++) {
                                const cell = usedRange.getCell(i, j);
                                cell.load(["address", "format"]);
                                cells.push(cell);
                            }
                        }
                        await context.sync();

                        const propertiesToLoadInFormat = [
                            'fill', 'font', 'borders', 'columnWidth', 'horizontalAlignment',
                            'indentLevel', 'rowHeight', 'verticalAlignment', 'wrapText'
                        ];

                        for (const cell of cells) {
                            cell.format.load(propertiesToLoadInFormat);
                        }
                        await context.sync();
                        app.suspendScreenUpdatingUntilNextSync();
                        const changeBorder = (sourceBorder: Excel.RangeBorderCollection, destinationBorder: Excel.RangeBorder[], sideIndex: Excel.BorderIndex) => {
                            const border = sourceBorder.items.find((c) => c.sideIndex == sideIndex);
                            if (border) {
                                const destBorder = destinationBorder.find(x => x.sideIndex === sideIndex);
                                if (destBorder) {
                                    border.style = destBorder.style;
                                    border.weight = destBorder.weight;
                                    border.color = destBorder?.color;
                                }
                            }
                        };

                        for (const cell of cells) {
                            const cellAddress = cell.address;
                            const cellFormat = cell.format;
                            const cellStyles = savedStyles.filter(x => x.address === cellAddress);
                            let cellStyle1: {};

                            cellStyles.forEach((cell, index) => {
                                if (index > 0) {
                                    cellStyle1 = Object.assign(cellStyle1, cell.format);
                                } else {
                                    cellStyle1 = cell.format;
                                }
                            });
                            if (cellStyle1) {
                                const cellStyle = cellStyle1 as Excel.RangeFormat;
                                if (!cellStyle.fill?.isNullObject) {
                                    cellFormat.fill.color = cellStyle.fill.color;
                                }
                                if (!cellStyle.font?.isNullObject) {
                                    Object.assign(cellFormat.font, {
                                        bold: cellStyle.font?.bold,
                                        color: cellStyle.font?.color,
                                        italic: cellStyle.font?.italic,
                                        name: cellStyle.font?.name,
                                        size: cellStyle.font?.size,
                                        underline: cellStyle.font?.underline
                                    });
                                }
                                if (!cellStyle.borders?.isNullObject && !Array.isArray(cellStyle.borders)) {
                                    const borderItems = cellStyle.borders.items;
                                    if (!isEmpty(borderItems)) {
                                        for (const sideIndex of [Excel.BorderIndex.edgeBottom, Excel.BorderIndex.edgeTop, Excel.BorderIndex.edgeLeft, Excel.BorderIndex.edgeRight]) {
                                            changeBorder(cellFormat.borders, borderItems, sideIndex);
                                        }
                                    }
                                }
                                if (cellStyle?.columnWidth != null) {
                                    cellFormat.columnWidth = cellStyle.columnWidth;
                                }
                                if (cellStyle?.rowHeight != null) {
                                    cellFormat.rowHeight = cellStyle.rowHeight;
                                }
                                if (cellStyle?.indentLevel != null) {
                                    cellFormat.indentLevel = cellStyle.indentLevel;
                                }
                                if (cellStyle?.horizontalAlignment != null) {
                                    cellFormat.horizontalAlignment = cellStyle.horizontalAlignment;
                                }
                                if (cellStyle?.verticalAlignment != null) {
                                    cellFormat.verticalAlignment = cellStyle.verticalAlignment;
                                }
                                if (cellStyle?.wrapText != null) {
                                    cellFormat.wrapText = cellStyle.wrapText;
                                }
                            }
                        }
                    }
                    await context.sync();
                }
            });

        } catch (e) {
            console.log(e);
        }
    }
    setUpEvents = async (onWorkSheetFormatChanged: (event: Excel.WorksheetFormatChangedEventArgs) => Promise<any>) => {
        try {
            await Excel.run(async (context) => {
                let sheet = context.workbook.worksheets.getItemOrNullObject("Report");
                await context.sync();
                if (!sheet.isNullObject) {
                    sheet.onFormatChanged.add(onWorkSheetFormatChanged);
                    await context.sync()
                }
            });
        }
        catch (e) {
            console.log(e);
        }

    }

    getLocaleCurrencySymbol = (): string => {
        try {
            // Create a NumberFormat object with currency style for the user's default locale
            const formatter = new Intl.NumberFormat(undefined, {
                style: 'currency',
                currency: 'USD', // Placeholder currency, the symbol will be locale-dependent
                minimumFractionDigits: 0,
                maximumFractionDigits: 0,
            });

            // Format a dummy number (e.g., 0) and extract the symbol
            const parts = formatter.formatToParts(0);
            const currencyPart = parts.find(part => part.type === 'currency');
            return currencyPart ? currencyPart.value : '';
        } catch (e) {
            console.warn("Could not determine locale currency symbol using Intl.NumberFormat:", e);
            return '';
        }
    }

    /**
     * Infers a suitable Excel number format string from a given numeric or currency string.
     * This function attempts to detect currency symbols, thousands separators, and decimal places.
     *
     * @param inputString The string representation of a number or currency (e.g., "1,234,567", "$1,234,567.89", "500", "Mex$100").
     * @returns An Excel number format string (e.g., "$#,##0.00", "#,##0", "0.00") or "General" if no specific format can be inferred.
     */
    inferExcelNumberFormat = (inputString: string): string => {
        if (!inputString || typeof inputString !== 'string') {
            return "General"; // Return general for invalid input
        }

        // Trim whitespace
        let cleanedString = inputString.trim();

        // Prioritize longer symbols first to correctly match "Mex$" before "$"
        const commonCurrencySymbols = ["F  CFA", "AED", "ARS", "AZN", "BGN", "BYR", "CA$",
            "CHF", "CLP", "CN¥", "COP", "CZK", "DKK", "EGP", "GHS", "HK$", "HUF", "IDR",
            "KZT", "MX$", "MYR", "NGN", "NOK", "NZ$", "PEN", "PLN", "PYG", "RON", "RSD",
            "SAR", "SEK", "SGD", "THB", "TRY", "NT$", "UAH", "UYU", "ZAR", "A$", "R$",
            "€", "£", "₹", "¥", "₩", "₱", "$", "₫"].sort((a, b) => a.length - b.length);

        // Add the user's locale currency symbol if available and not already in the list
        const localeCurrencySymbol = this.getLocaleCurrencySymbol();
        if (localeCurrencySymbol && !commonCurrencySymbols.includes(localeCurrencySymbol)) {
            commonCurrencySymbols.unshift(localeCurrencySymbol);
        }

        let isCurrency = false;
        let currencySymbol = '';
        let symbolIsPrefix = true;
        let localeInfo = {
            locale: 'en-US',
            thousandsSeparator: ',',
            decimalSeparator: '.',
            largeNumberThreshold: 1000,
            numberPattern: '#,##0'
        };

        // Detect currency symbol and get locale info
        for (const symbol of commonCurrencySymbols) {
            if (cleanedString.startsWith(symbol)) {
                isCurrency = true;
                currencySymbol = symbol;
                cleanedString = cleanedString.substring(symbol.length).trim();
                symbolIsPrefix = true;
                localeInfo = this.getLocaleInfoFromCurrencySymbol(symbol);
                break;
            }
            if (cleanedString.endsWith(symbol) && parseFloat(cleanedString.substring(0, cleanedString.length - symbol.length).trim()).toString() === cleanedString.substring(0, cleanedString.length - symbol.length).trim()) {
                isCurrency = true;
                currencySymbol = symbol;
                cleanedString = cleanedString.substring(0, cleanedString.length - symbol.length).trim();
                symbolIsPrefix = false;
                localeInfo = this.getLocaleInfoFromCurrencySymbol(symbol);
                break;
            }
        }


        // Handle parentheses for negative numbers

        let isNegativeParentheses = false;

        if (cleanedString.startsWith('(') && cleanedString.endsWith(')')) {

            isNegativeParentheses = true;

            cleanedString = cleanedString.substring(1, cleanedString.length - 1).trim();

        }

        // Remove thousands separators based on locale
        cleanedString = cleanedString.replace(new RegExp(`\\${localeInfo.thousandsSeparator}`, 'g'), '');

        // Check for explicit negative sign

        let hasNegativeSign = cleanedString.startsWith('-');

        if (hasNegativeSign) {

            cleanedString = cleanedString.substring(1);

        }

        // Determine decimal places using locale-specific decimal separator
        const decimalIndex = cleanedString.indexOf(localeInfo.decimalSeparator);
        let decimalPlaces = 0;
        let hasDecimal = false;

        if (decimalIndex !== -1) {
            hasDecimal = true;
            decimalPlaces = cleanedString.length - 1 - decimalIndex;
        }

        // Convert decimal separator for parsing if needed
        const normalizedString = cleanedString.replace(localeInfo.decimalSeparator, '.');
        const numericValue = parseFloat(normalizedString);

        // Construct the format string using dynamically generated pattern
        let format = '';
        const originalHasThousandsSeparator = inputString.includes(localeInfo.thousandsSeparator);

        // Use locale-specific threshold and dynamically generated pattern
        if (originalHasThousandsSeparator || (numericValue >= localeInfo.largeNumberThreshold || numericValue <= -localeInfo.largeNumberThreshold)) {
            // Use the dynamically generated number pattern
            format = localeInfo.numberPattern;
        } else {
            format = '0';
        }

        // Add decimal places
        if (hasDecimal) {
            format += '.' + '0'.repeat(decimalPlaces);
        }

        // Add currency symbol
        if (isCurrency) {
            if (symbolIsPrefix) {
                format = currencySymbol + format;
            } else {
                format = format + currencySymbol;
            }
        }

        if (isNegativeParentheses) {

            let negativeFormatPart = format;

            if (isCurrency) {

                negativeFormatPart = format.replace(currencySymbol, '');

            }

            negativeFormatPart = `(${negativeFormatPart})`;

            if (isCurrency && symbolIsPrefix) {

                negativeFormatPart = currencySymbol + negativeFormatPart;

            } else if (isCurrency && !symbolIsPrefix) {

                negativeFormatPart = negativeFormatPart + currencySymbol;

            }

            format = `${format}_);${negativeFormatPart}`;

        }

        // Dynamic threshold based on locale
        if (!originalHasThousandsSeparator && numericValue < localeInfo.largeNumberThreshold && numericValue > -localeInfo.largeNumberThreshold && !isCurrency) {
            if (hasDecimal) {
                format = '0.' + '0'.repeat(decimalPlaces);
            } else {
                format = '0';
            }
        }

        // Final validation
        if (format === '' || format === localeInfo.numberPattern || format === '0') {
            if (hasDecimal) {
                format = (format === '' ? '0' : format) + '.' + '0'.repeat(decimalPlaces);
            } else if (format === '') {
                format = '0';
            }
        }

        return format;
    }
    getLocaleInfoFromCurrencySymbol = (currencySymbol: string): {
        locale: string;
        thousandsSeparator: string;
        decimalSeparator: string;
        largeNumberThreshold: number;
        numberPattern: string;
    } => {
        // Currency symbol to locale mapping (no hardcoded patterns)
        const currencyLocaleMap: { [key: string]: string } = {
            '$': 'en-US',
            'US$': 'en-US',
            'CA$': 'en-CA',
            'A$': 'en-AU',
            'NZ$': 'en-NZ',
            'HK$': 'en-HK',
            'MX$': 'es-MX',
            'R$': 'pt-BR',
            '€': 'de-DE',
            '£': 'en-GB',
            '¥': 'ja-JP',
            'CN¥': 'zh-CN',
            '₹': 'hi-IN',
            '₩': 'ko-KR',
            '₱': 'fil-PH',
            '₫': 'vi-VN',
            'CHF': 'de-CH',
            'SEK': 'sv-SE',
            'NOK': 'nb-NO',
            'DKK': 'da-DK',
            'PLN': 'pl-PL',
            'CZK': 'cs-CZ',
            'HUF': 'hu-HU',
            'RON': 'ro-RO',
            'BGN': 'bg-BG',
            'TRY': 'tr-TR',
            'ZAR': 'en-ZA',
            'THB': 'th-TH',
            'SGD': 'en-SG',
            'MYR': 'ms-MY',
            'IDR': 'id-ID',
            'AED': 'ar-AE',
            'SAR': 'ar-SA',
            'EGP': 'ar-EG',
            'UAH': 'uk-UA',
            'RSD': 'sr-RS'
        };

        const locale = currencyLocaleMap[currencySymbol] || 'en-US';

        try {
            // Test with different numbers to understand the locale's number formatting pattern
            const formatter = new Intl.NumberFormat(locale);

            // Test numbers to analyze patterns
            const testNumbers = [1234567, 12345, 1234, 123];
            const formattedNumbers = testNumbers.map(num => formatter.format(num));

            let thousandsSeparator = ',';
            let decimalSeparator = '.';
            let numberPattern = '#,##0';

            // Analyze the formatted numbers to extract patterns
            for (const formatted of formattedNumbers) {
                // Find thousands separator by looking for non-digit characters that repeat
                const separators = formatted.match(/\D/g) || [];
                if (separators.length > 0) {
                    // The most common separator is likely the thousands separator
                    const separatorCounts = separators.reduce((acc, sep) => {
                        acc[sep] = (acc[sep] || 0) + 1;
                        return acc;
                    }, {} as { [key: string]: number });

                    const mostCommonSeparator = Object.keys(separatorCounts)
                        .reduce((a, b) => separatorCounts[a] > separatorCounts[b] ? a : b);

                    if (mostCommonSeparator && mostCommonSeparator !== '.' && mostCommonSeparator !== ',') {
                        thousandsSeparator = mostCommonSeparator;
                    } else if (formatted.includes(',') && !formatted.endsWith(',')) {
                        thousandsSeparator = ',';
                    } else if (formatted.includes('.') && formatted.indexOf('.') < formatted.length - 3) {
                        thousandsSeparator = '.';
                    } else if (formatted.includes(' ')) {
                        thousandsSeparator = ' ';
                    }
                }
            }

            // Test decimal formatting
            const decimalTest = formatter.format(1234.56);
            const decimalMatch = decimalTest.match(/(\D)(\d{1,2})$/);
            if (decimalMatch) {
                decimalSeparator = decimalMatch[1];
            }

            // Generate dynamic pattern based on actual formatting
            numberPattern = this.generateNumberPattern(formattedNumbers, thousandsSeparator);

            // Different locales have different thresholds for using thousands separators
            const largeNumberThreshold = this.determineThreshold(formattedNumbers, thousandsSeparator);

            return {
                locale,
                thousandsSeparator,
                decimalSeparator,
                largeNumberThreshold,
                numberPattern
            };
        } catch (e) {
            // Fallback to US format
            return {
                locale: 'en-US',
                thousandsSeparator: ',',
                decimalSeparator: '.',
                largeNumberThreshold: 1000,
                numberPattern: '#,##0'
            };
        }
    }

    generateNumberPattern = (formattedNumbers: string[], thousandsSeparator: string): string => {
        // Analyze the longest formatted number to understand the pattern
        const longestNumber = formattedNumbers.reduce((a, b) => a.length > b.length ? a : b);

        // Remove any non-digit, non-separator characters for analysis
        const cleanNumber = longestNumber.replace(/[^\d\s.,'-]/g, '');

        // Count separator occurrences and positions
        const separatorPositions: number[] = [];
        for (let i = 0; i < cleanNumber.length; i++) {
            if (cleanNumber[i] === thousandsSeparator) {
                separatorPositions.push(i);
            }
        }

        if (separatorPositions.length === 0) {
            return '#,##0'; // Return standard pattern instead of just '0'
        }

        // Handle special cases like Indian numbering (12,34,567)
        if (separatorPositions.length > 1) {
            // Check if it's Indian style numbering
            const segments = cleanNumber.split(thousandsSeparator);
            if (segments.length > 2) {
                const lastSegmentLength = segments[segments.length - 1].length;
                const secondLastSegmentLength = segments[segments.length - 2].length;

                if (lastSegmentLength === 3 && secondLastSegmentLength === 2) {
                    // Indian numbering system
                    return `#${thousandsSeparator}##${thousandsSeparator}##0`;
                }
            }
        }

        // Create a standard pattern based on the thousands separator
        // The key fix: Always use # for optional digits, only use 0 for required digits
        const segments = cleanNumber.split(thousandsSeparator);

        if (segments.length >= 2) {
            // Standard pattern: #,##0 (where # represents optional digits, 0 represents required digits)
            return `#${thousandsSeparator}##0`;
        }

        // Fallback to standard comma-separated format
        return '#,##0';
    }

    determineThreshold = (formattedNumbers: string[], thousandsSeparator: string): number => {
        // Find the smallest number that has a thousands separator
        const numbersWithSeparators = formattedNumbers.filter(formatted =>
            formatted.includes(thousandsSeparator)
        );

        if (numbersWithSeparators.length === 0) {
            return 1000; // Default threshold
        }

        // Extract numeric values and find the minimum
        const numericValues = numbersWithSeparators.map(formatted => {
            const cleanNumber = formatted.replace(new RegExp(`\\${thousandsSeparator}`, 'g'), '');
            return parseInt(cleanNumber.replace(/\D/g, ''), 10);
        });

        const minWithSeparator = Math.min(...numericValues);

        // Common thresholds: 1000, 10000
        if (minWithSeparator >= 10000) {
            return 10000;
        } else if (minWithSeparator >= 1000) {
            return 1000;
        } else {
            return 1000; // Default
        }
    }

}
