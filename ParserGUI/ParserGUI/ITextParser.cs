using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig.Content;

namespace ParserCore
{
    public interface ITextParser
    {
        string GetText(List<Letter> letters);
    }
}
