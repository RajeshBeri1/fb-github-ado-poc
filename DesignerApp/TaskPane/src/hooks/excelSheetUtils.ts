// excelSheetUtils.ts
// Utility functions for Excel worksheet operations

export async function deleteSplitWorksheets(prevSplitColumnName: string) {
    if (!prevSplitColumnName) return;
    await Excel.run(async (context) => {
        const sheets = context.workbook.worksheets;
        sheets.load("items/name");
        await context.sync();
        sheets.items.forEach(s => {
            if (s.name.startsWith(`${prevSplitColumnName}-`)) {
                try { s.delete(); } catch { /* ignore individual delete errors */ }
            }
        });
        await context.sync();
    });
}

export async function deleteNamedSplitWorksheets(splitColumnName: string, flowchartData: any[]) {
    if (!splitColumnName || !flowchartData?.length) return;
    const namesToDelete = new Set(
        flowchartData
            .filter(l => l?.ColumnName && l?.Name)
            .map(l => `${splitColumnName}-${l.Name}`)
    );
    await Excel.run(async (context) => {
        const sheets = context.workbook.worksheets;
        sheets.load("items/name");
        await context.sync();
        sheets.items.forEach(s => {
            if (namesToDelete.has(s.name)) {
                try { s.delete(); } catch { /* ignore individual delete errors */ }
            }
        });
        await context.sync();
    });
}

export async function deleteAllWorksheetsExceptReport() {
    await Excel.run(async (context) => {
        const sheets = context.workbook.worksheets;
        sheets.load("items/name");
        await context.sync();
        if (sheets.items.length > 1) {
            for (let i = 1; i < sheets.items.length; i++) {
                try {
                    if (sheets.items[i].name === "Report") continue;
                    sheets.items[i].delete();
                } catch (e) {}
            }
            await context.sync();
        } else if (sheets.items.length === 1) {
            sheets.items[0].getUsedRange().clear();
            await context.sync();
        }
    });
}
