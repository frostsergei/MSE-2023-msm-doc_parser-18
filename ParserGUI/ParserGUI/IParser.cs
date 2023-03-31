using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabula;

namespace ParserCore
{
    public interface IParser
    {
        void Parse();
        int TableCount { get; }
        List<Table> GetTables();
        Table GetTable(int tableNumber);
        string GetCellText(Table table, Cell cell);
        List<Table> ParsePage(int pageNumber);
    }
}
