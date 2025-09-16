namespace AthenaQueryTool
{
    partial class MainWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.panelTop = new System.Windows.Forms.Panel();
            this.buttonExportData = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxExportTable = new System.Windows.Forms.TextBox();
            this.buttonExportDataDictionary = new System.Windows.Forms.Button();
            this.checkBoxProps = new System.Windows.Forms.CheckBox();
            this.checkBoxVertical = new System.Windows.Forms.CheckBox();
            this.buttonShowTables = new System.Windows.Forms.Button();
            this.buttonExecute = new System.Windows.Forms.Button();
            this.textBoxCommand = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.dataGrid = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.textBoxStatus = new System.Windows.Forms.TextBox();
            this.panelTop.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).BeginInit();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelTop
            // 
            this.panelTop.Controls.Add(this.buttonExportData);
            this.panelTop.Controls.Add(this.label2);
            this.panelTop.Controls.Add(this.textBoxExportTable);
            this.panelTop.Controls.Add(this.buttonExportDataDictionary);
            this.panelTop.Controls.Add(this.checkBoxProps);
            this.panelTop.Controls.Add(this.checkBoxVertical);
            this.panelTop.Controls.Add(this.buttonShowTables);
            this.panelTop.Controls.Add(this.buttonExecute);
            this.panelTop.Controls.Add(this.textBoxCommand);
            this.panelTop.Controls.Add(this.label1);
            this.panelTop.Dock = System.Windows.Forms.DockStyle.Top;
            this.panelTop.Location = new System.Drawing.Point(0, 0);
            this.panelTop.Name = "panelTop";
            this.panelTop.Size = new System.Drawing.Size(1207, 155);
            this.panelTop.TabIndex = 0;
            // 
            // buttonExportData
            // 
            this.buttonExportData.Location = new System.Drawing.Point(963, 57);
            this.buttonExportData.Name = "buttonExportData";
            this.buttonExportData.Size = new System.Drawing.Size(146, 23);
            this.buttonExportData.TabIndex = 9;
            this.buttonExportData.Text = "Export Data";
            this.buttonExportData.UseVisualStyleBackColor = true;
            this.buttonExportData.Click += new System.EventHandler(this.ButtonExportData_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(749, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 15);
            this.label2.TabIndex = 8;
            this.label2.Text = "Export Table:";
            // 
            // textBoxExportTable
            // 
            this.textBoxExportTable.Location = new System.Drawing.Point(840, 21);
            this.textBoxExportTable.Name = "textBoxExportTable";
            this.textBoxExportTable.Size = new System.Drawing.Size(100, 23);
            this.textBoxExportTable.TabIndex = 7;
            this.textBoxExportTable.Text = "media_plans";
            // 
            // buttonExportDataDictionary
            // 
            this.buttonExportDataDictionary.Location = new System.Drawing.Point(960, 19);
            this.buttonExportDataDictionary.Name = "buttonExportDataDictionary";
            this.buttonExportDataDictionary.Size = new System.Drawing.Size(149, 25);
            this.buttonExportDataDictionary.TabIndex = 6;
            this.buttonExportDataDictionary.Text = "Export Data Dictionary";
            this.buttonExportDataDictionary.UseVisualStyleBackColor = true;
            this.buttonExportDataDictionary.Click += new System.EventHandler(this.ButtonExportDataDictionary_Click);
            // 
            // checkBoxProps
            // 
            this.checkBoxProps.AutoSize = true;
            this.checkBoxProps.Location = new System.Drawing.Point(665, 121);
            this.checkBoxProps.Name = "checkBoxProps";
            this.checkBoxProps.Size = new System.Drawing.Size(111, 19);
            this.checkBoxProps.TabIndex = 5;
            this.checkBoxProps.Text = "Show properties";
            this.checkBoxProps.UseVisualStyleBackColor = true;
            this.checkBoxProps.CheckedChanged += new System.EventHandler(this.CheckBoxProps_CheckedChanged);
            // 
            // checkBoxVertical
            // 
            this.checkBoxVertical.AutoSize = true;
            this.checkBoxVertical.Location = new System.Drawing.Point(541, 121);
            this.checkBoxVertical.Name = "checkBoxVertical";
            this.checkBoxVertical.Size = new System.Drawing.Size(96, 19);
            this.checkBoxVertical.TabIndex = 4;
            this.checkBoxVertical.Text = "Show vertical";
            this.checkBoxVertical.UseVisualStyleBackColor = true;
            this.checkBoxVertical.CheckedChanged += new System.EventHandler(this.CheckBoxVertical_CheckedChanged);
            // 
            // buttonShowTables
            // 
            this.buttonShowTables.Location = new System.Drawing.Point(85, 110);
            this.buttonShowTables.Name = "buttonShowTables";
            this.buttonShowTables.Size = new System.Drawing.Size(100, 23);
            this.buttonShowTables.TabIndex = 3;
            this.buttonShowTables.Text = "Show Tables";
            this.buttonShowTables.UseVisualStyleBackColor = true;
            this.buttonShowTables.Click += new System.EventHandler(this.ButtonShowTables_Click);
            // 
            // buttonExecute
            // 
            this.buttonExecute.Location = new System.Drawing.Point(537, 20);
            this.buttonExecute.Name = "buttonExecute";
            this.buttonExecute.Size = new System.Drawing.Size(75, 23);
            this.buttonExecute.TabIndex = 2;
            this.buttonExecute.Text = "Execute";
            this.buttonExecute.UseVisualStyleBackColor = true;
            this.buttonExecute.Click += new System.EventHandler(this.ButtonExecute_Click);
            // 
            // textBoxCommand
            // 
            this.textBoxCommand.Location = new System.Drawing.Point(85, 19);
            this.textBoxCommand.Multiline = true;
            this.textBoxCommand.Name = "textBoxCommand";
            this.textBoxCommand.Size = new System.Drawing.Size(437, 85);
            this.textBoxCommand.TabIndex = 1;
            this.textBoxCommand.Text = "SELECT * FROM pepsico LIMIT 1";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "Command:";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.dataGrid);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 155);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(1207, 433);
            this.panel1.TabIndex = 3;
            // 
            // dataGrid
            // 
            this.dataGrid.AllowUserToAddRows = false;
            this.dataGrid.AllowUserToDeleteRows = false;
            this.dataGrid.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGrid.Location = new System.Drawing.Point(0, 0);
            this.dataGrid.Name = "dataGrid";
            this.dataGrid.RowTemplate.Height = 25;
            this.dataGrid.Size = new System.Drawing.Size(1207, 433);
            this.dataGrid.TabIndex = 2;
            this.dataGrid.ColumnAdded += new System.Windows.Forms.DataGridViewColumnEventHandler(this.DataGrid_ColumnAdded);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.textBoxStatus);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 588);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(1207, 100);
            this.panel2.TabIndex = 0;
            // 
            // textBoxStatus
            // 
            this.textBoxStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textBoxStatus.Location = new System.Drawing.Point(0, 0);
            this.textBoxStatus.Multiline = true;
            this.textBoxStatus.Name = "textBoxStatus";
            this.textBoxStatus.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBoxStatus.Size = new System.Drawing.Size(1207, 100);
            this.textBoxStatus.TabIndex = 3;
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1207, 688);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panelTop);
            this.Name = "MainWindow";
            this.Text = "Athena Query Tool";
            this.panelTop.ResumeLayout(false);
            this.panelTop.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGrid)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panelTop;
        private Button buttonShowTables;
        private Button buttonExecute;
        private TextBox textBoxCommand;
        private Label label1;
        private Panel panel1;
        private DataGridView dataGrid;
        private Panel panel2;
        private TextBox textBoxStatus;
        private CheckBox checkBoxVertical;
        private CheckBox checkBoxProps;
        private Button buttonExportDataDictionary;
        private Label label2;
        private TextBox textBoxExportTable;
        private Button buttonExportData;
    }
}