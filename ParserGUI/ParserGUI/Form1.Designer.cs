
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
            this.ButtonHelp = new System.Windows.Forms.Button();
            this.ButtonSettings = new System.Windows.Forms.Button();
            this.ButtonFile = new System.Windows.Forms.Button();
            this.TextBoxChoose = new System.Windows.Forms.TextBox();
            this.MiddlePanel = new System.Windows.Forms.Panel();
            this.RTBOutput = new System.Windows.Forms.RichTextBox();
            this.TextBoxSave = new System.Windows.Forms.TextBox();
            this.ButtonStart = new System.Windows.Forms.Button();
            this.ButtonChooseFile = new System.Windows.Forms.Button();
            this.LabelChoose = new System.Windows.Forms.Label();
            this.LabelSave = new System.Windows.Forms.Label();
            this.TopPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // TopPanel
            // 
            this.TopPanel.Controls.Add(this.ButtonHelp);
            this.TopPanel.Controls.Add(this.ButtonSettings);
            this.TopPanel.Controls.Add(this.ButtonFile);
            this.TopPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TopPanel.Location = new System.Drawing.Point(0, 0);
            this.TopPanel.Name = "TopPanel";
            this.TopPanel.Size = new System.Drawing.Size(800, 24);
            this.TopPanel.TabIndex = 0;
            // 
            // ButtonHelp
            // 
            this.ButtonHelp.FlatAppearance.BorderSize = 0;
            this.ButtonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonHelp.Location = new System.Drawing.Point(144, 0);
            this.ButtonHelp.Name = "ButtonHelp";
            this.ButtonHelp.Size = new System.Drawing.Size(75, 24);
            this.ButtonHelp.TabIndex = 2;
            this.ButtonHelp.Text = "Справка";
            this.ButtonHelp.UseVisualStyleBackColor = true;
            // 
            // ButtonSettings
            // 
            this.ButtonSettings.FlatAppearance.BorderSize = 0;
            this.ButtonSettings.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonSettings.Location = new System.Drawing.Point(72, 0);
            this.ButtonSettings.Name = "ButtonSettings";
            this.ButtonSettings.Size = new System.Drawing.Size(75, 24);
            this.ButtonSettings.TabIndex = 1;
            this.ButtonSettings.Text = "Настройки";
            this.ButtonSettings.UseVisualStyleBackColor = true;
            // 
            // ButtonFile
            // 
            this.ButtonFile.FlatAppearance.BorderSize = 0;
            this.ButtonFile.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ButtonFile.Location = new System.Drawing.Point(0, 0);
            this.ButtonFile.Name = "ButtonFile";
            this.ButtonFile.Size = new System.Drawing.Size(75, 24);
            this.ButtonFile.TabIndex = 0;
            this.ButtonFile.Text = "Файл";
            this.ButtonFile.UseVisualStyleBackColor = true;
            this.ButtonFile.Click += new System.EventHandler(this.ButtonFile_Click);
            // 
            // TextBoxChoose
            // 
            this.TextBoxChoose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxChoose.Location = new System.Drawing.Point(116, 30);
            this.TextBoxChoose.Multiline = true;
            this.TextBoxChoose.Name = "TextBoxChoose";
            this.TextBoxChoose.Size = new System.Drawing.Size(672, 24);
            this.TextBoxChoose.TabIndex = 1;
            this.TextBoxChoose.TextChanged += new System.EventHandler(this.TextBoxChoose_TextChanged);
            // 
            // MiddlePanel
            // 
            this.MiddlePanel.Location = new System.Drawing.Point(0, 60);
            this.MiddlePanel.Name = "MiddlePanel";
            this.MiddlePanel.Size = new System.Drawing.Size(800, 32);
            this.MiddlePanel.TabIndex = 3;
            // 
            // RTBOutput
            // 
            this.RTBOutput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RTBOutput.Location = new System.Drawing.Point(12, 98);
            this.RTBOutput.Name = "RTBOutput";
            this.RTBOutput.Size = new System.Drawing.Size(776, 314);
            this.RTBOutput.TabIndex = 4;
            this.RTBOutput.Text = "";
            // 
            // TextBoxSave
            // 
            this.TextBoxSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TextBoxSave.Location = new System.Drawing.Point(299, 418);
            this.TextBoxSave.Multiline = true;
            this.TextBoxSave.Name = "TextBoxSave";
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
            // Window
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 452);
            this.Controls.Add(this.ButtonChooseFile);
            this.Controls.Add(this.LabelSave);
            this.Controls.Add(this.LabelChoose);
            this.Controls.Add(this.ButtonStart);
            this.Controls.Add(this.TextBoxSave);
            this.Controls.Add(this.RTBOutput);
            this.Controls.Add(this.MiddlePanel);
            this.Controls.Add(this.TextBoxChoose);
            this.Controls.Add(this.TopPanel);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Window";
            this.Text = "DocParser";
            this.TopPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel TopPanel;
        private System.Windows.Forms.Button ButtonFile;
        private System.Windows.Forms.Button ButtonSettings;
        private System.Windows.Forms.Button ButtonHelp;
        private System.Windows.Forms.TextBox TextBoxChoose;
        private System.Windows.Forms.Panel MiddlePanel;
        private System.Windows.Forms.RichTextBox RTBOutput;
        private System.Windows.Forms.TextBox TextBoxSave;
        private System.Windows.Forms.Button ButtonStart;
        private System.Windows.Forms.Button ButtonChooseFile;
        private System.Windows.Forms.Label LabelChoose;
        private System.Windows.Forms.Label LabelSave;
    }
}

