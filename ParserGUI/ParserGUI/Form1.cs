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
        public static bool BFClicked = false;

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
                    Stream NewFile = sfd.OpenFile();
                    StreamWriter sw = new StreamWriter(NewFile);
                    sw.WriteLine(Result);
                    sw.Close();
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
            await Task.Run(() =>
            {
                if (ofd.FileName != "")
                {
                    BFClicked = true;
                    Result = Parser.PDFToString(ofd.FileName);
                }
                else
                {
                    MessageBox.Show("Вы не выбрали файл!");
                }
            }
            );
            RTBOutput.Text = Result;
        }

        private void RTBOutput_TextChanged(object sender, EventArgs e)
        {
            if (RTBOutput.ReadOnly)
                {
                    RTBOutput.BackColor = SystemColors.Window;
                }
        }

        private void progressBar_Click(object sender, EventArgs e)
        {
          
        }
        
    }
    }
    

