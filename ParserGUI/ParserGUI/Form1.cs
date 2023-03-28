using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using ParserCore;

namespace ParserGUI
{
    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        string Result;
        IParser parser;
        WaitForm WaitForm2 = new WaitForm();

        private void TextBoxChoose_TextChanged(object sender, EventArgs e)
        {
            TextBoxChoose.BackColor = SystemColors.Window;
        }

        private void CheckBoxSave_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void TextBoxSave_TextChanged(object sender, EventArgs e)
        {
            TextBoxSave.BackColor = SystemColors.Window;
        }

        private void ButtonChooseFile_Click(object sender, EventArgs e)
        {
            if (ofd.FileName != "")
            {
                SaveFileDialog sfd = new SaveFileDialog();
                sfd.Filter = "XML|*.xml";
                sfd.Title = "Сохранить результат";
                sfd.ShowDialog();

                if (sfd.FileName != "")
                {
                    TextBoxSave.Text = sfd.FileName;
                    FileStream NewFile = File.OpenWrite(sfd.FileName);
                    NewFile.SetLength(0); // C# почему-то не стирает содержимое уже существующих файлов, если они открыты для записи
                    XMLGenerator.ToFile(parser.GetTables(), NewFile);
                    NewFile.Close();
                    MessageBox.Show("Файл успешно сохранен!");
                }
            }
            else
            {
                MessageBox.Show("Вы не выбрали файл!");
            }
        }

        private void ButtonFile_Click(object sender, EventArgs e)
        {
            ofd.Filter = "PDF|*.pdf";
            ofd.Title = "Выбор документа";
            if (ofd.ShowDialog(this) == DialogResult.OK)
            {
                if (ofd.FileName != null)
                {
                    TextBoxChoose.Text = ofd.FileName;
                }
            }
        }
        private async void ButtonStart_Click(object sender, EventArgs e)
        {
            if (ofd.FileName != "")
            {
                WaitForm2.Show();
                WaitForm2.Refresh();
            }
            await Task.Run(() =>
            {
                DbService dbService = new DbService("YOUR .mdb FILE");
                if (ofd.FileName != "")
                {
                    parser = new TabulaParser(ofd.FileName, new NearestNeighbourTextParser());
                    parser.Parse();
                    RTFResult result = new RTFResult(parser);
                    Result = result.Serialize();
                }
                else
                {
                    MessageBox.Show("Вы не выбрали файл!");
                }
            }
            );
            WaitForm2.Close();
            RTBOutput.Rtf = Result;
        }

        private void RTBOutput_TextChanged(object sender, EventArgs e)
        {
            if (RTBOutput.ReadOnly)
            {
                RTBOutput.BackColor = SystemColors.Window;
            }
        }
    }
}


