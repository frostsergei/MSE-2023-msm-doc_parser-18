using ParserCore;
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
        private string tableName;
        public DbService(string path, string tableName)
        {
            this.connectString = $"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={path};";
            this.tableName = tableName;
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

        public void writeParamsToDb(Data data)
        {
            ConnectToDb();
            string q = $"CREATE TABLE [{this.tableName}] ([name] TEXT, [range] TEXT, [description] TEXT)";
            OleDbCommand command = new OleDbCommand(q, myConnection);
            command.ExecuteNonQuery();
            foreach (Data.Parameter param in data.ReadAll())
            {
                string qq = $"INSERT INTO [{this.tableName}] ([name], [range], [description]) VALUES ('{param.Name}', '{param.Range}', '{param.Description}');";
                OleDbCommand command_ = new OleDbCommand(qq, myConnection);
                command_.ExecuteNonQuery();
            }
            DisconnectFromDb();
        }

    }
}
