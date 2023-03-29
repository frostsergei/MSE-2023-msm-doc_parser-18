using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabula;

namespace ParserCore
{
    internal interface ITableFilter
    {
        bool CheckTable(Table table);
        IEnumerable<Table> Filter(IEnumerable<Table> tables);
    }
}
