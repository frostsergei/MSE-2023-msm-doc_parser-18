using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabula;

namespace ParserCore
{
    internal class SimpleTableFilter : ITableFilter
    {
        private readonly float _minTableWidth;
        private readonly int _minTableRows = 2;
        private readonly PageArea _page;

        public SimpleTableFilter(PageArea page,float minTableWidth = 0.7f, int minTableRows = 2)
        {
            _minTableRows = minTableRows;
            _minTableWidth = minTableWidth;
            _page = page;
        }
        public bool CheckTable(Table table)
        {
            return table.RowCount >= _minTableRows && table.Width > _minTableWidth * _page.Width;
        }

        public IEnumerable<Table> Filter(IEnumerable<Table> tables)
        {
            return tables.Where(CheckTable);
        }
    }
}
