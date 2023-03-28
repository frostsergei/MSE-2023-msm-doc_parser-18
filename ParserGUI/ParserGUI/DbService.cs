using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tabula;

namespace ParserGUI
{
    class DbService
    {
        private string connectString;
        private OleDbConnection myConnection;
        public DbService(string path)
        {
            this.connectString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={path};";
        }

        public void ConnectToDb()
        {
            myConnection = new OleDbConnection(connectString);
            myConnection.Open();
        }

        public void DisconnectFromDb()
        {
            myConnection.Close();
        }

        public void writeTableToDb()
        {
            string[,] tables_data = { { "I", "am", "Hello", "World", "Yeah", "Right" }, { "No", "I", "am", "real", "Hello", "World" } };
            string[] tables_names = { "Option1", "Option2" };
            for (int i = 0; i < tables_data.GetLength(0); i++)
            {
                for (int j = 0; j < tables_data.GetLength(1); j++)
                {
                    string q = $"INSERT INTO {tables_names[i]} (test_string) VALUES ('{tables_data[i, j]}');";
                    Console.WriteLine(q);
                    OleDbCommand command = new OleDbCommand(q, myConnection);
                    command.ExecuteNonQuery();
                }
            }


        }
    }
}
