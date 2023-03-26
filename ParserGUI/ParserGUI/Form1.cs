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
                sfd.Filter = "Data base file|*.mdb|XML|*.xml";
                sfd.Title = "Сохранить результат";
                sfd.ShowDialog();

                if (sfd.FileName != "")
                {
                    TextBoxSave.Text = sfd.FileName;
                    FileStream NewFile = File.OpenWrite(sfd.FileName);
                    XMLGenerator.ToFile(Result, NewFile);
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
                //WaitForm WaitForm2 = new WaitForm();
                WaitForm2.Show(this);
                WaitForm2.Refresh();
                await Task.Run(() =>
                {
                    IParser parser = new TabulaParser(ofd.FileName, new NearestNeighbourTextParser());
                    parser.Parse();
                    RTFResult result = new RTFResult(parser);
                    Result = result.Serialize();
                }
                );
                RTBOutput.Text = Result;
                WaitForm2.Close();
            }
            else
            {
                MessageBox.Show("Вы не выбрали файл!");
            }
        }

        private void RTBOutput_TextChanged(object sender, EventArgs e)
        {
            if (RTBOutput.ReadOnly)
                {
                    RTBOutput.BackColor = SystemColors.Window;
                }
        }

        private Point m_PreviousLocation = new Point(int.MinValue, int.MinValue);

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (m_PreviousLocation.X != int.MinValue) 
                WaitForm2.Location = new Point(
                      WaitForm2.Location.X + Location.X - m_PreviousLocation.X,
                      WaitForm2.Location.Y + Location.Y - m_PreviousLocation.Y
                    );

            m_PreviousLocation = Location;
        }
    }
    }
    
    

