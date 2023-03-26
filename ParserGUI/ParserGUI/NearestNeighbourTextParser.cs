using System;
using System.Collections.Generic;
using System.Text;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;

namespace ParserGUI
{
    internal class NearestNeighbourTextParser : ITextParser
    {
        public string GetText(List<Letter> letters)
        {
            StringBuilder stringBuilder = new StringBuilder();
            var wordExtractorOptions = new NearestNeighbourWordExtractor.NearestNeighbourWordExtractorOptions()
            {
                Filter = (pivot, candidate) =>
                {
                    if (string.IsNullOrWhiteSpace(candidate.Value))
                    {
                        return false;
                    }

                    var maxHeight = Math.Max(pivot.PointSize, candidate.PointSize);
                    var minHeight = Math.Min(pivot.PointSize, candidate.PointSize);
                    if (minHeight != 0 && maxHeight / minHeight > 2.0)
                    {
                        return false;
                    }
                    var pivotRgb = pivot.Color.ToRGBValues();
                    var candidateRgb = candidate.Color.ToRGBValues();
                    if (!pivotRgb.Equals(candidateRgb))
                    {
                        return false;
                    }
                    return true;
                }
            };
            var wordExtractor = new NearestNeighbourWordExtractor(wordExtractorOptions);

            var words = wordExtractor.GetWords(letters);

            Word previous = null;
            foreach (var word in words)
            {
                if (previous != null)
                {
                    var hasInsertedWhitespace = false;
                    var bothNonEmpty = previous.Letters.Count > 0 && word.Letters.Count > 0;
                    if (bothNonEmpty)
                    {
                        var prevLetter1 = previous.Letters[0];
                        var currentLetter1 = word.Letters[0];

                        var baselineGap = Math.Abs(prevLetter1.StartBaseLine.Y - currentLetter1.StartBaseLine.Y);

                        if (baselineGap > 3)
                        {
                            hasInsertedWhitespace = true;
                            stringBuilder.AppendLine();
                        }
                    }

                    if (!hasInsertedWhitespace)
                    {
                        //stringBuilder.Append(" ");
                    }
                }

                stringBuilder.Append(word.Text);

                previous = word;
            }
            return stringBuilder.ToString();
        }
    }
}
