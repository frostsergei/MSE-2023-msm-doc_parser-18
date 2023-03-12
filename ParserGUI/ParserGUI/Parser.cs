using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;


public class Parser
{
    public static string PDFToString(string fpath)
    {
        PdfDocument doc = PdfDocument.Open(fpath);
        string _out = (" ");
        foreach (Page p in doc.GetPages())
        {
            IEnumerable<Word> words = p.GetWords();
            double last_y = double.PositiveInfinity;
            foreach (Word w in words)
            {
                double y = w.BoundingBox.Top;
                if (double.IsInfinity(last_y) || last_y - y >= 10)
                {
                    if (!double.IsInfinity(last_y))
                        _out += "\r\n";
                    last_y = y;
                }
                _out += w + " ";
            }
            _out += "\r\n";
        }
        return _out;
    }
}