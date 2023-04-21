using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tabula;

namespace ParserCore
{
    internal class RTFResult
    {
        private StringBuilder _stringBuilder;
        private Data data;

        public RTFResult(Data _data)
        {
            data = _data;
            _stringBuilder = new StringBuilder();
            _stringBuilder.Append(@"{\rtf1 ");
        }

        public string Serialize()
        {
            _stringBuilder.Append(@"\trowd ");
            _stringBuilder.Append(@"\cellx2000 ");
            _stringBuilder.Append(@"\cellx4000 ");
            _stringBuilder.Append(@"\cellx10000 ");

            _stringBuilder.Append(@"\intbl \b " + "Название параметра" + @"\b0 \cell");
            _stringBuilder.Append(@"\intbl \b " + "Диапазон" + @"\b0 \cell");
            _stringBuilder.Append(@"\intbl \b " + "Описание" + @"\b0 \cell");
            _stringBuilder.Append(@"\row ");

            foreach(Data.Parameter param in data.ReadAll())
            {
                _stringBuilder.Append(@"\trowd ");
                _stringBuilder.Append(@"\cellx2000 ");
                _stringBuilder.Append(@"\cellx4000 ");
                _stringBuilder.Append(@"\cellx10000 ");

                _stringBuilder.Append(@"\intbl " + param.Name + @"\cell");
                _stringBuilder.Append(@"\intbl " + param.Range + @"\cell");
                _stringBuilder.Append(@"\intbl " + param.Description + @"\cell");
                _stringBuilder.Append(@"\row ");
            }
            _stringBuilder.Append(@"\pard");
            _stringBuilder.Append(@"}");
            return _stringBuilder.ToString();
        }

    }
}
