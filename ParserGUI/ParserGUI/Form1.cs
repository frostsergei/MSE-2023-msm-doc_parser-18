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
using System.Xml;

namespace ParserGUI
{
    public partial class Window : Form
    {
        public Window()
        {
            InitializeComponent();
        }

        OpenFileDialog ofd = new OpenFileDialog();
        WaitForm WaitForm2;
        Parser parser;

        private void TextBoxChoose_TextChanged(object sender, EventArgs e)
        {
            TextBoxChoose.BackColor = SystemColors.Window;
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
                sfd.Filter = "Data base file|*.mdb|XML|*.xml";
                sfd.Title = "Сохранить результат";
                sfd.ShowDialog();
                string fileExtension = Path.GetExtension(sfd.FileName);
                if (sfd.FileName != "" && fileExtension != ".mdb")
                {
                    TextBoxSave.Text = sfd.FileName;
                    FileStream NewFile = File.OpenWrite(sfd.FileName);
                    NewFile.SetLength(0); // C# почему-то не стирает содержимое уже существующих файлов, если они открыты для записи
                    XMLGenerator.WriteData(parser.GetData(), NewFile);
                    NewFile.Close();
                    MessageBox.Show("Файл успешно сохранен!");
                }
                else if (sfd.FileName != "" && fileExtension == ".mdb")
                {
                    var tableName = ofd.FileName.Split('\\');
                    DbService dbService = new DbService(sfd.FileName, tableName.Last().ToLower().Replace(".pdf", ""));
                    dbService.writeParamsToDb(parser.GetData());
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
                DataTable.Rows.Clear();
                WaitForm2 = new WaitForm();
                CenterWaitFormToWindow();
                WaitForm2.Show(this);

                await Task.Run(() => {
                    parser = new Parser(ofd.FileName);
                }
                );

                foreach (Data.Parameter param in parser.GetData().ReadAll())
                {
                    DataTable.Rows.Add(param.Name, param.Range, param.Description);
                }
                DataTable.AutoResizeColumns();
                DataTable.AllowUserToAddRows = false;
                WaitForm2.Close();
            }
            else
            {
                MessageBox.Show("Вы не выбрали файл!");
            }
        }

        private Point m_PreviousLocation = new Point(int.MinValue, int.MinValue);

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (WaitForm2 != null)
            {
                if (m_PreviousLocation.X != int.MinValue)
                {
                    WaitForm2.Location = new Point(
                        WaitForm2.Location.X + Location.X - m_PreviousLocation.X,
                        WaitForm2.Location.Y + Location.Y - m_PreviousLocation.Y
                        );
                }
                m_PreviousLocation = Location;
            }
        }
        private void CenterWaitFormToWindow()
        {
            WaitForm2.Location = new Point(
            this.Location.X + this.Width / 2 - WaitForm2.ClientSize.Width / 2,
            this.Location.Y + this.Height / 2 - WaitForm2.ClientSize.Height / 2);
        }
        private void AutoSizeRowsMode(Object sender, EventArgs es)
        {
            DataTable.AutoSizeRowsMode =
                DataGridViewAutoSizeRowsMode.AllCells;
        }

    }
}


