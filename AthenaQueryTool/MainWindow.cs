using Amazon.Athena.Model;
using AutoMapper;
using Lib.Athena.Business;
using Lib.Athena.Business.Interfaces;
using Lib.Athena.Models;
using Lib.Common.Business;
using Lib.WebAPI.Extensions;
using Lib.WebAPI.Models;
using Throw;

namespace AthenaQueryTool
{
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : Form
    {
        private readonly IAthenaQueryLogic athena;
        private readonly AthenaDataConverter dataConverter;
        private readonly IMapper mapper;
        private AthenaQueryResult? queryResult;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow" /> class.
        /// </summary>
        /// <param name="athena">The athena.</param>
        /// <param name="dataConverter">The data converter.</param>
        /// <param name="mapper">The mapper.</param>
        public MainWindow(IAthenaQueryLogic athena, AthenaDataConverter dataConverter, IMapper mapper)
        {
            this.athena = athena;
            this.dataConverter = dataConverter;
            this.mapper = mapper;

            InitializeComponent();
            CheckBoxVertical_CheckedChanged(default!, default!);
        }

        private void AddAsColumns(AthenaQueryResult result)
        {
            textBoxStatus.Text += $"Adding {result.ColumnInfo.Count} columns";

            Application.DoEvents();

            for (int i = 0; i < result.ColumnInfo.Count; i++)
            {
                var x = result.ColumnInfo[i];

                var props = string.Empty;
                if (checkBoxProps.Checked)
                {
                    props = $"{Environment.NewLine}(T: {x.Type}, P: {x.Precision}, S: {x.Scale}, N: {x.Nullable})";
                }

                dataGrid.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell())
                {
                    Name = $"{x.Name}{props}",
                });

                if (i % 200 == 0)
                {
                    textBoxStatus.Text += ".";
                    Application.DoEvents();
                }
            }

            textBoxStatus.Text += $"{Environment.NewLine}";

            textBoxStatus.Text += $"Adding {result.Rows.Count} rows...{Environment.NewLine}";

            Application.DoEvents();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                dataGrid.Rows.Add(result.Rows[i].Data.Select(x => x.VarCharValue).ToArray());
            }
        }

        private void AddAsRows(AthenaQueryResult result)
        {
            textBoxStatus.Text += $"Adding {result.Rows.Count} columns...{Environment.NewLine}";

            Application.DoEvents();

            for (int i = 0; i < result.Rows.Count; i++)
            {
                dataGrid.Columns.Add(new DataGridViewColumn(new DataGridViewTextBoxCell()));
            }

            textBoxStatus.Text += $"{Environment.NewLine}";

            textBoxStatus.Text += $"Adding {result.ColumnInfo.Count} rows...{Environment.NewLine}";

            Application.DoEvents();

            var items = new List<string>();

            for (int c = 0; c < result.ColumnInfo.Count; c++)
            {
                items.Clear();

                var x = result.ColumnInfo[c];

                for (int r = 0; r < result.Rows.Count; r++)
                {
                    var firstRow = !result.Rows[r].Data.Select(x => x.VarCharValue).Except(result.ColumnInfo.Select(x => x.Name)).Any();
                    if (firstRow)
                    {
                        items.Add(result.Rows[r].Data[c].VarCharValue);
                    }
                    else
                    {
                        try
                        {
                            var value = dataConverter.Convert(result.Rows[r].Data[c], x);
                            items.Add(value?.ToString() ?? "NULL");
                        }
                        catch (Exception ex)
                        {
                            textBoxStatus.Text += $"Data conversion error: {result.Rows[r].Data[c].VarCharValue} is not a valid {x.Type}, column: {x.Name}. {ex.Message}{Environment.NewLine}";
                        }
                    }
                }

                var props = string.Empty;
                if (checkBoxProps.Checked)
                {
                    props = $"{Environment.NewLine}(T: {x.Type}, P: {x.Precision}, S: {x.Scale}, N: {x.Nullable})";
                }

                dataGrid.Rows.Add(items.ToArray());
                dataGrid.Rows[^1].HeaderCell.Value = $"{x.Name}{props}";
            }
        }

        private async void ButtonExecute_Click(object sender, EventArgs e)
        {
            await RunQueryAsync(textBoxCommand.Text);
        }

        private async void ButtonExportData_Click(object sender, EventArgs e)
        {
            await ExportDataAsync();
        }

        private async void ButtonExportDataDictionary_Click(object sender, EventArgs e)
        {
            await ExportDataDictionaryAsync();
        }

        private async void ButtonShowTables_Click(object sender, EventArgs e)
        {
            await RunQueryAsync("SHOW TABLES");
        }

        private void CheckBoxProps_CheckedChanged(object sender, EventArgs e)
        {
            DisplayData();
        }

        private void CheckBoxVertical_CheckedChanged(object sender, EventArgs e)
        {
            dataGrid.Columns.Clear();
            dataGrid.Rows.Clear();

            if (checkBoxVertical.Checked)
            {
                dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                dataGrid.ColumnHeadersVisible = false;

                dataGrid.RowHeadersVisible = true;
                dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllHeaders;
            }
            else
            {
                dataGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
                dataGrid.RowHeadersVisible = false;

                dataGrid.ColumnHeadersVisible = true;
                dataGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.ColumnHeader;
            }

            DisplayData();
        }

        private void DataGrid_ColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (dataGrid.ColumnHeadersVisible)
            {
                e.Column.FillWeight = 1;
            }
        }

        private void DisplayData()
        {
            dataGrid.Columns.Clear();
            dataGrid.Rows.Clear();

            if (queryResult == null)
            {
                return;
            }

            if (dataGrid.ColumnHeadersVisible)
            {
                AddAsColumns(queryResult);
            }
            else
            {
                AddAsRows(queryResult);
            }
        }

        private async Task ExportDataAsync()
        {
            try
            {
                textBoxExportTable.Text.Throw().IfNotValidSql();
                textBoxStatus.Text += $"Getting data from table {textBoxExportTable.Text}{Environment.NewLine}";

                using var source = new CancellationTokenSource();

                var result = await athena.QueryAsync($"SELECT * FROM {textBoxExportTable.Text}", source.Token);

                var data = dataConverter.Convert(result);

                var json = Json.Serialize(data);

                var path = Path.Combine(AppContext.BaseDirectory, $"data_{textBoxExportTable.Text}.json");

                textBoxStatus.Text += $"Writing data to {path}{Environment.NewLine}";

                await File.WriteAllTextAsync(path, json, source.Token);

                textBoxStatus.Text += $"Done!{Environment.NewLine}";
            }
            catch (Exception ex)
            {
                textBoxStatus.Text += ex.Message;
            }
            finally
            {
            }
        }

        private async Task ExportDataDictionaryAsync()
        {
            try
            {
                textBoxExportTable.Text.Throw().IfNotValidSql();
                textBoxStatus.Text += $"Getting data dictionary for table {textBoxExportTable.Text}{Environment.NewLine}";

                using var source = new CancellationTokenSource();

                var columnInfos = await athena.GetTableColumnInfosAsync(textBoxExportTable.Text, source.Token);

                var list = new List<DataDictionaryColumn>();

                foreach (var column in columnInfos)
                {
                    var data = mapper.Map<DataDictionaryColumn>(column);
                    list.Add(data);
                }

                var json = Json.Serialize(list);

                var path = Path.Combine(AppContext.BaseDirectory, $"datadictionary_{textBoxExportTable.Text}.json");

                textBoxStatus.Text += $"Writing data dictionary to {path}{Environment.NewLine}";

                await File.WriteAllTextAsync(path, json, source.Token);

                textBoxStatus.Text += $"Done!{Environment.NewLine}";
            }
            catch (Exception ex)
            {
                textBoxStatus.Text += ex.Message;
            }
            finally
            {
            }
        }

        private AthenaQueryResult GetSampleData()
        {
            var columns = new List<ColumnInfo>();
            var rows = new List<Datum>();

            for (int i = 0; i < 100; i++)
            {
                columns.Add(new ColumnInfo { Name = $"Column {i}" });
                rows.Add(new Datum { VarCharValue = $"Row {i}" });
            }

            return new AthenaQueryResult
            {
                ColumnInfo = columns,
                Rows = new List<Row>()
                {
                    new Row
                    {
                        Data = rows,
                    },
                },
            };
        }

        private async Task RunQueryAsync(string query)
        {
            try
            {
                textBoxStatus.Text = $"Running {query}...{Environment.NewLine}";

                buttonShowTables.Enabled = false;
                buttonExecute.Enabled = false;

                dataGrid.Columns.Clear();
                dataGrid.Rows.Clear();

                Application.DoEvents();

                queryResult = await athena.QueryAsync(query, default);

                // queryResult = GetSampleData();

                DisplayData();

                textBoxStatus.Text += $"Done!{Environment.NewLine}";
            }
            catch (Exception ex)
            {
                textBoxStatus.Text += $"Failed: {ex.Message}{Environment.NewLine}{ex.StackTrace}{Environment.NewLine}";
            }
            finally
            {
                buttonShowTables.Enabled = true;
                buttonExecute.Enabled = true;
            }
        }
    }
}