using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabula;

namespace ParserGUI
{
    internal class RTFResult
    {
        private StringBuilder _stringBuilder;
        private IParser _parser;

        public RTFResult(IParser parser)
        {
            _parser = parser;
            _stringBuilder = new StringBuilder();
            _stringBuilder.Append(@"{\rtf1\ansi\deff0");
        }

        public void AddTable(Table table)
        {         
            for (int i = 0; i < table.RowCount; i++)
            {
                _stringBuilder.Append(@"\trowd ");
                var nonEmptyCells = table.Rows[i].Where(cell => cell.GetText() != "");
                for (int j = 0; j < nonEmptyCells.Count(); j++)
                {
                    _stringBuilder.Append(@"\cellx" + (j + 1) * (10000 / nonEmptyCells.Count()));
                }
                foreach (var cell in nonEmptyCells)
                {
                    _stringBuilder.Append(@"\intbl ");
                    _stringBuilder.Append(_parser.GetCellText(table, cell).Replace("\n", @"\line "));
                    _stringBuilder.Append(@"\cell ");
                }
                _stringBuilder.Append(@"\row ");
            }
        }

        public string Serialize()
        {
            foreach (var table in _parser.GetTables())
            {
                AddTable(table);
            }
            _stringBuilder.Append(@"}");
            return _stringBuilder.ToString();
        }

    }
}
