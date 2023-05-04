
namespace ParserGUI
{
    partial class Window
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Window));
            this.TopPanel = new System.Windows.Forms.Panel();
            this.ButtonFile = new System.Windows.Forms.Button();
            this.TextBoxChoose = new System.Windows.Forms.TextBox();
            this.TextBoxSave = new System.Windows.Forms.TextBox();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.ButtonChooseFile = new System.Windows.Forms.Button();
            this.LabelChoose = new System.Windows.Forms.Label();
            this.LabelSave = new System.Windows.Forms.Label();
            this.DataTable = new System.Windows.Forms.DataGridView();
            this.ParamColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.RangeColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DescColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TopPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataTable)).BeginInit();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.ButtonFile);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(800, 24);
            this.TopPanel.TabIndex = 0;
            // 
            // ButtonFile
            // 
            this.ButtonFile.BackColor = System.Drawing.SystemColors.Control;
            this.ButtonFile.FlatAppearance.BorderSize = 0;
            this.ButtonFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonFile.Location = new System.Drawing.Point(12, 3);
            this.ButtonFile.Name = "ButtonFile";
            this.ButtonFile.Size = new System.Drawing.Size(75, 24);
            this.ButtonFile.TabIndex = 0;
            this.ButtonFile.Text = "Файл";
            this.ButtonFile.UseVisualStyleBackColor = false;
            this.ButtonFile.Click += new System.EventHandler(this.ButtonFile_Click);
            // 
            // TextBoxChoose
            // 
            this.TextBoxChoose.AccessibleRole = System.Windows.Forms.AccessibleRole.TitleBar;
            this.TextBoxChoose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxChoose.Location = new System.Drawing.Point(116, 30);
            this.TextBoxChoose.Multiline = true;
            this.TextBoxChoose.Name = "TextBoxChoose";
            this.TextBoxChoose.ReadOnly = true;
            this.TextBoxChoose.Size = new System.Drawing.Size(672, 24);
            this.TextBoxChoose.TabIndex = 1;
            this.TextBoxChoose.TextChanged += new System.EventHandler(this.TextBoxChoose_TextChanged);
            // 
            // TextBoxSave
            // 
            this.TextBoxSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxSave.Location = new System.Drawing.Point(299, 418);
            this.TextBoxSave.Multiline = true;
            this.TextBoxSave.Name = "TextBoxSave";
            this.TextBoxSave.ReadOnly = true;
            this.TextBoxSave.Size = new System.Drawing.Size(489, 24);
            this.TextBoxSave.TabIndex = 5;
            this.TextBoxSave.TextChanged += new System.EventHandler(this.TextBoxSave_TextChanged);
            // 
            // ButtonStart
            // 
            this.ButtonStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonStart.BackColor = System.Drawing.Color.PaleGreen;
            this.ButtonStart.FlatAppearance.BorderSize = 0;
            this.ButtonStart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonStart.Location = new System.Drawing.Point(12, 418);
            this.ButtonStart.MinimumSize = new System.Drawing.Size(75, 24);
            this.ButtonStart.Name = "ButtonStart";
            this.ButtonStart.Size = new System.Drawing.Size(75, 24);
            this.ButtonStart.TabIndex = 7;
            this.ButtonStart.Text = "Запуск";
            this.ButtonStart.UseVisualStyleBackColor = false;
            this.ButtonStart.Click += new System.EventHandler(this.ButtonStart_Click);
            // 
            // ButtonChooseFile
            // 
            this.ButtonChooseFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ButtonChooseFile.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.ButtonChooseFile.FlatAppearance.BorderSize = 0;
            this.ButtonChooseFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonChooseFile.Location = new System.Drawing.Point(218, 418);
            this.ButtonChooseFile.MinimumSize = new System.Drawing.Size(75, 24);
            this.ButtonChooseFile.Name = "ButtonChooseFile";
            this.ButtonChooseFile.Size = new System.Drawing.Size(75, 24);
            this.ButtonChooseFile.TabIndex = 8;
            this.ButtonChooseFile.Text = "Выбрать";
            this.ButtonChooseFile.UseVisualStyleBackColor = false;
            this.ButtonChooseFile.Click += new System.EventHandler(this.ButtonChooseFile_Click);
            // 
            // LabelChoose
            // 
            this.LabelChoose.AutoSize = true;
            this.LabelChoose.Location = new System.Drawing.Point(12, 33);
            this.LabelChoose.Name = "LabelChoose";
            this.LabelChoose.Size = new System.Drawing.Size(98, 13);
            this.LabelChoose.TabIndex = 9;
            this.LabelChoose.Text = "Выбранный файл:";
            // 
            // LabelSave
            // 
            this.LabelSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.LabelSave.AutoSize = true;
            this.LabelSave.Location = new System.Drawing.Point(111, 424);
            this.LabelSave.Name = "LabelSave";
            this.LabelSave.Size = new System.Drawing.Size(101, 13);
            this.LabelSave.TabIndex = 0;
            this.LabelSave.Text = "Сохранить в файл:";
            // 
            // DataTable
            // 
            this.DataTable.AccessibleRole = System.Windows.Forms.AccessibleRole.None;
            this.DataTable.AllowUserToAddRows = false;
            this.DataTable.AllowUserToDeleteRows = false;
            this.DataTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataTable.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.DisplayedCells;
            this.DataTable.BackgroundColor = System.Drawing.SystemColors.HighlightText;
            this.DataTable.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataTable.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParamColumn,
            this.RangeColumn,
            this.DescColumn});
            this.DataTable.Location = new System.Drawing.Point(15, 69);
            this.DataTable.Name = "DataTable";
            this.DataTable.ReadOnly = true;
            this.DataTable.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.DataTable.Size = new System.Drawing.Size(773, 332);
            this.DataTable.TabIndex = 10;
            // 
            // ParamColumn
            // 
            this.ParamColumn.HeaderText = "Название параметра";
            this.ParamColumn.Name = "ParamColumn";
            this.ParamColumn.ReadOnly = true;
            this.ParamColumn.Width = 138;
            // 
            // RangeColumn
            // 
            this.RangeColumn.HeaderText = "Диапазон";
            this.RangeColumn.Name = "RangeColumn";
            this.RangeColumn.ReadOnly = true;
            this.RangeColumn.Width = 80;
            // 
            // DescColumn
            // 
            this.DescColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DescColumn.HeaderText = "Описание";
            this.DescColumn.Name = "DescColumn";
            this.DescColumn.ReadOnly = true;
            // 
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 452);
            this.Controls.Add(this.DataTable);
            this.Controls.Add(this.ButtonChooseFile);
            this.Controls.Add(this.LabelSave);
            this.Controls.Add(this.LabelChoose);
            this.Controls.Add(this.ButtonStart);
            this.Controls.Add(this.TextBoxSave);
            this.Controls.Add(this.TextBoxChoose);
            this.Controls.Add(this.TopPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(320, 150);
            this.Name = "Window";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "DocParser";
            this.LocationChanged += new System.EventHandler(this.Window_LocationChanged);
            this.TopPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataTable)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Button ButtonFile;
        private System.Windows.Forms.TextBox TextBoxChoose;
        private System.Windows.Forms.TextBox TextBoxSave;
        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.Button ButtonChooseFile;
        private System.Windows.Forms.Label LabelChoose;
        private System.Windows.Forms.Label LabelSave;
        private System.Windows.Forms.DataGridView DataTable;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParamColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn RangeColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DescColumn;
    }
}

