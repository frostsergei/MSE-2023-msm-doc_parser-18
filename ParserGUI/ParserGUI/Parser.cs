using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
using System;
using System.IO;
using System.Xml;
using Tabula;
using System.Linq;
using System.Text;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using System.Reflection;
using System.Runtime.InteropServices;
using UglyToad.PdfPig.Graphics.Colors;
using System.Drawing;
using UglyToad.PdfPig.Graphics;

namespace ParserCore
{
    public class Parser
    {
        private List<int> _pageNumbers;
        private PdfDocument _document;

        public Parser(string filename)
        {
            _document = PdfDocument.Open(filename, new ParsingOptions() { ClipPaths = true });
            ParseContent(_document);
        }

        public void ParseContent(PdfDocument document)
        {
            var documentContentParser = new DocumentContentParser(document);
            _pageNumbers = documentContentParser.Parse();
        }

        public Data ParseLineParams(List<int> page_numbers)
        {
            Data data = new Data();
            Func<PdfPath, bool> isVertical = path => { var rect = path[0].GetBoundingRectangle(); return rect?.Width * 3 < rect?.Height; };
            Func<PdfPath, bool> isBlack = path => { var rgb = path.FillColor?.ToRGBValues(); if (rgb == null) return false; return rgb?.r != 1 && rgb?.g != 1 && rgb?.b != 1; };
            Func<Letter, Tuple<PointF, PointF>, bool> isNearLine = (letter, line) =>
            {
                return letter.GlyphRectangle.Top <= line.Item2.Y &&
                letter.GlyphRectangle.Bottom >= line.Item1.Y;
            };
            Func<Tuple<PointF, PointF>, Tuple<PointF, PointF>, bool> isIntersecting = (line1, line2) =>
            {
                return (line1.Item1.Y >= line2.Item1.Y - 1 && line1.Item1.Y <= line2.Item2.Y + 1)
                || (line1.Item2.Y >= line2.Item1.Y - 1 && line1.Item2.Y <= line2.Item2.Y + 1);
            };
            Func<PdfPath, Tuple<PointF, PointF>> toLine = path => {
                var rect = path[0].GetBoundingRectangle();
                return new Tuple<PointF, PointF>(new PointF((float)rect?.Left, (float)rect?.Bottom), new PointF((float)rect?.Left, (float)rect?.Top));
            };

            foreach (var pageNum in page_numbers)
            {
                var page = _document.GetPage(pageNum);
                var paths = page.ExperimentalAccess.Paths.ToList();               
                var lines = paths.Where(path =>  isVertical(path)&& isBlack(path))
                    .Select(toLine)
                    .ToList();

                List<Tuple<PointF, PointF>> lines2 = new List<Tuple<PointF, PointF>>();
                List<Tuple<PointF, PointF>> remove = new List<Tuple<PointF, PointF>>();
                for (int i = 0; i < lines.Count; i++)
                {
                    var intersecting = lines2.Where(x => isIntersecting(x, lines[i]));
                    if(intersecting.Count() == 0)
                    {
                        lines2.Add(lines[i]);
                    }
                    else
                    {
                        remove.AddRange(intersecting);
                    }
                }
                foreach (var line in remove)
                    lines2.Remove(line);

                var letters = page.Letters;
                List<Tuple<string,string>> parameters = new List<Tuple<string, string>>();
                foreach (var line in lines2)
                {
                    var lettersNearLine = letters.Where(letter => isNearLine(letter, line));
                    var left =  lettersNearLine.Where(letter => letter.GlyphRectangle.Right < line.Item1.X).ToList();
                    var right = lettersNearLine.Where(letter => letter.GlyphRectangle.Left > line.Item1.X).ToList();
                    var name = GetText(left);
                    var description = GetText(right);
                    parameters.Add(new Tuple<string, string>(name, description));
                }
                foreach (var parameter in parameters)
                {
                    var names = parameter.Item1.Replace('\r','\n').Split('\n').Select(line=>line.Trim()).Where(line=>!string.IsNullOrEmpty(line));
                    foreach(var name in names)
                    {
                        data.WriteElem(new Data.Parameter {
                            Name = name,
                            Description = parameter.Item2.Replace("\r","").Replace('\n',' ').Trim(),
                            Range = ""
                        });
                    }
                }

            }
            return data;
        }


      
        public Data ParseSimpleTable(TabulaParser parser, List<int> page_numbers)
        {
            Data dat = new Data();

            List<string>[] header_sentences = new List<string>[]{new List<string>{"Номер","элемента","списка" },
                                                                 new List<string>{"Значение", "элемента", "адрес", "и", "признаки", "вывода", "на", "печать"},
                                                                 new List<string>{"Наименование", "элемента", "и", "комментарии"} };

            foreach (int page_num in page_numbers)
            {
                List<Table> tables = parser.ParsePage(page_num);
                foreach (Table table in tables)
                {
                    foreach (IReadOnlyList<Cell> row in table.Rows)
                    {
                        Data.Parameter param = new Data.Parameter();
                        bool wrote_row = false;
                        for (int i = 0; i < row.Count; ++i)
                        {
                            Cell cell = row[i];
                            string cell_text = "";
                            foreach (TextChunk chunk in cell.TextElements)
                            {
                                foreach (TextElement elem in chunk.TextElements)
                                {
                                    cell_text += elem.GetText();
                                }
                            }
                            if (cell_text.Length == 0)
                                continue;
                            string[] cell_words = cell_text.Split(' ');

                            int word_i = 0;
                            bool row_is_header = true; bool has_valid_header_words = false;
                            foreach(string word in cell_words){
                                if(word.Length == 0 || !char.IsLetter(word[0]))
                                    continue;
                                has_valid_header_words = true;
                                bool matches_header = false;
                                foreach(List<string> sentence in header_sentences){
                                    if(word_i >= sentence.Count)
                                        continue;
                                    if(sentence[word_i] == word){
                                        matches_header = true;
                                        break;
                                    }
                                }
                                if(!matches_header){
                                    row_is_header = false;
                                    break;
                                }
                                ++word_i;
                            }
                            if(row_is_header && has_valid_header_words)
                                break;

                            cell_text = cell_text.Trim();
                            if(cell_text.Length == 0)
                                continue;

                            if(!wrote_row)
                                wrote_row = true;
                            switch (i)
                            {
                                case 0: // Номер элемента списка
                                    param.Name = cell_text;
                                    break;
                                case 1:
                                    // Адрес параметра, не используется
                                    break;
                                case 2:
                                    param.Description = cell_text;
                                    break;
                            }
                        }
                        if(wrote_row)
                            dat.WriteElem(param);
                    }
                }
            }
            return dat;
        }


        private class TableColumn
        {
            public double left;
            public List<Word> words = new List<Word>();
        }

        private class WordComparer : IComparer<Word>
        {
            public int Compare(Word word1, Word word2)
            {
                double top_margin = (word1.BoundingBox.Height + word2.BoundingBox.Height) / 2;
                if (object.ReferenceEquals(word1, word2))
                    return 0;
                else
                {
                    if (Math.Abs(word1.BoundingBox.Top - word2.BoundingBox.Top) <= top_margin)
                        return word1.BoundingBox.Left < word2.BoundingBox.Left ? -1 :
                            word1.BoundingBox.Left > word2.BoundingBox.Left ? 1 : 0;
                    return word1.BoundingBox.Top < word2.BoundingBox.Top ? 1 :
                            word1.BoundingBox.Top > word2.BoundingBox.Top ? -1 : 0;
                }
            }
        }

        public Data ParseStringParams(List<int> pages_numbers, string doc_name)
        {
            Data data = new Data();
            const double column_margin = 10;

            foreach (int page_num in pages_numbers)
            {
                PdfDocument doc = PdfDocument.Open(doc_name);
                IEnumerable<Word> raw_words = doc.GetPage(page_num).GetWords();
                List<Word> words = new List<Word>(raw_words);
                words.Sort(new WordComparer());
                List<TableColumn> table_columns = new List<TableColumn>();
                bool parse = false;
                foreach (Word w in words)
                {
                    if (w.FontName.ToLower().Contains("bold"))
                        parse = true;
                    if (!parse)
                        continue;
                    bool create_new_col = true;
                    foreach (TableColumn column in table_columns)
                    {
                        if (Math.Abs(w.BoundingBox.Left - column.left) <= column_margin)
                        {
                            column.words.Add(w);
                            create_new_col = false;
                            break;
                        }
                    }
                    if (create_new_col)
                    {
                        var new_col = new TableColumn();
                        new_col.left = w.BoundingBox.Left;
                        new_col.words.Add(w);
                        table_columns.Add(new_col);
                    }
                }
                for (int i = 2; i < table_columns.Count; i++)
                {
                    table_columns[1].words.AddRange(table_columns[i].words);
                }
                table_columns.RemoveRange(2, table_columns.Count - 2);
                table_columns[1].words.Sort(new WordComparer());
                int column_1_i = 0, column_2_i = 0;
                double line_height_1 = table_columns[0].words[0].BoundingBox.Height;
                double line_height_2 = table_columns[1].words[0].BoundingBox.Height;
                List<List<TableColumn>> rows = new List<List<TableColumn>>();
                while (column_1_i < table_columns[0].words.Count && column_2_i < table_columns[1].words.Count)
                {
                    List<TableColumn> row = new List<TableColumn>();
                    TableColumn tc1 = new TableColumn(), tc2 = new TableColumn();
                    double last_y = table_columns[0].words[column_1_i].BoundingBox.Top;
                    for (; column_1_i < table_columns[0].words.Count
                        && table_columns[0].words[column_1_i].BoundingBox.Top >= last_y - line_height_1 * 2; column_1_i++)
                    {
                        if (!table_columns[0].words[column_1_i].FontName.ToLower().Contains("bold"))
                        {
                            goto end_table;
                        }
                        if (table_columns[0].words[column_1_i].BoundingBox.Top <= last_y)
                        {
                            last_y = table_columns[0].words[column_1_i].BoundingBox.Top;
                        }
                        tc1.words.Add(table_columns[0].words[column_1_i]);
                    }
                    double last_y_2 = table_columns[1].words[column_2_i].BoundingBox.Top;
                    for (; column_2_i < table_columns[1].words.Count
                        && table_columns[1].words[column_2_i].BoundingBox.Top >= last_y_2 - line_height_2 * 2; column_2_i++)
                    {
                        if (table_columns[1].words[column_2_i].BoundingBox.Top <= last_y_2)
                        {
                            last_y_2 = table_columns[1].words[column_2_i].BoundingBox.Top;
                        }
                        tc2.words.Add(table_columns[1].words[column_2_i]);
                    }
                    row.Add(tc1);
                    row.Add(tc2);
                    rows.Add(row);
                }
            end_table:
                foreach (List<TableColumn> row in rows)
                {
                    string description = "";
                    foreach (Word w in row[1].words)
                    {
                        description += w.Text + ' ';
                    }
                    foreach (Word w in row[0].words)
                    {
                        string param = w.Text.Replace(",", "");
                        Data.Parameter parameter = new Data.Parameter();
                        parameter.Name = param;
                        parameter.Range = "";
                        parameter.Description = description;
                        data.WriteElem(parameter);
                    }
                }
            }
            return data;
        }

        private static string GetText(List<Letter> letters)
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


        private class DocumentContentParser
        {
            struct ContentTableItem
            {
                public string Number;
                public string Title;
                public int Page;
            }


            private PdfDocument _document;
            private List<string> _parsedLines;
            private static string _contentTableTitle = "содержание";
            private static int _maxPageNumber = 10;
            private int _pageNumberOffset = 0;
            private List<ContentTableItem> _contentItems;
            public DocumentContentParser(PdfDocument document)
            {
                _document = document;
                _parsedLines = new List<string>();
            }
            public List<int> Parse()
            {
                var contentPageNumber = FindContentPage();
                ParseAllContent(contentPageNumber);
                _contentItems = _parsedLines.Select(line => ParseContentTableItem(line)).ToList();
                return GetPages();
            }

            private int FindContentPage()
            {
                var pageNumber = 1;
                while (pageNumber < _maxPageNumber)
                {
                    var pageLines = GetPageLines(pageNumber);
                    bool found = false;
                    for (int i = 0; i < pageLines.Count; i++)
                    {
                        if (pageLines[i].ToLower().Contains(_contentTableTitle))
                        {
                            _pageNumberOffset = pageNumber - ParsePageNumber(pageLines[0],pageNumber);
                            _parsedLines.AddRange(pageLines.Where((line, index) => index > i));
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                    pageNumber++;
                }
                return pageNumber;
            }

            private int ParsePageNumber(string line,int page)
            {
                var words = line.Split(' ').ToList();
                var firstWord = words.First(word => !string.IsNullOrWhiteSpace(word));
                int pageNumber;
                if (int.TryParse(firstWord, out int _))
                {
                    pageNumber = int.Parse(firstWord);
                }
                else
                {
                    try
                    {
                        var lastWord = words.LastOrDefault(word => !string.IsNullOrWhiteSpace(word) && int.TryParse(word, out int _));
                        if (lastWord == "") throw new Exception("Cannot find page number");
                        pageNumber = int.Parse(lastWord);
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return page;
                    }
                }
                return pageNumber;
            }

            private int GetRealPageNumber(int documentPageNumber)
            {
                return documentPageNumber + _pageNumberOffset;
            }

            private ContentTableItem ParseContentTableItem(string line)
            {
                var words = line.Split(' ','.').Where(word => !string.IsNullOrWhiteSpace(word)).ToList();
                ContentTableItem item = new ContentTableItem()
                {
                    Number = char.IsDigit(words[0][0]) ? words[0] : "",
                    Title = string.Join(" ",
                            words.Where(word => !string.IsNullOrWhiteSpace(word) &&
                                                !char.IsDigit(word[0]) &&
                                                 word.Count(x => x == '.') < 3
                                                 )
                            ),
                    Page = GetRealPageNumber(int.Parse(words.Last(word => !string.IsNullOrWhiteSpace(word)).Replace(".", "")))
                };
                return item;
            }

            private List<string> GetPageLines(int pageNumber)
            {
                var page = _document.GetPage(pageNumber);
                var text = GetText((List<Letter>)page.Letters);
                var lines = text.Split('\n')
                            .Select(line => line.Replace("\r", ""))
                            .Where(line => !string.IsNullOrWhiteSpace(line))
                            .ToList();
                return lines;
            }

            private void ParseAllContent(int firstPage)
            {
                var firstEntryPage = ParseContentTableItem(_parsedLines[0]).Page;
                for (int i = firstPage + 1; i < firstEntryPage; i++)
                {
                    _parsedLines.AddRange(GetPageLines(i).Where((line, index) => index > 0));
                }
            }

            private ContentTableItem GetNextOnTheSameLevel(ContentTableItem item)
            {
                var digits = item.Number.Split('.').Length;
                var currentIndex = _contentItems.IndexOf(item);
                return _contentItems.FirstOrDefault((listItem) =>
                {
                    var listDigits = listItem.Number.Split('.').Length;
                    return _contentItems.IndexOf(listItem) > currentIndex && listDigits == digits;
                });
            }

            private List<int> GetPages()
            {
                string[] whiteList = new string[] { "ТЕК", "НАСТР", "БД", "Общесистем", "Вычисляемые", "вычисляемые", "параметр", "настроечн", "Настроечн", "Текущ" };
                string[] blackList = new string[] { "Приложение", "списки", "список", "Списки", "Список", "контр", "Контр", "Структур", "Ввод", "Режим", "режим", "справка", "Справка" };

                var pages = new SortedSet<int>();
                foreach (var item in _contentItems)
                {
                    if (whiteList.Any(word => item.Title.Contains(word)) &&
                    !blackList.Any(word => item.Title.Contains(word)))
                    {
                        var startPage = item.Page;
                        var endPage = GetNextOnTheSameLevel(item).Page;
                        for (int j = startPage; j <= endPage; j++)
                        {
                            pages.Add(j);
                        }
                    }
                }
                foreach (var item in _contentItems)
                {
                    if (blackList.Any(word => item.Title.Contains(word)))
                    {
                        var startPage = item.Page;
                        var prevIndex = _contentItems.IndexOf(item) - 1;
                        bool keepFirstPage = true;
                        if (prevIndex >= 0 && _contentItems[prevIndex].Number.Split('.').Length < item.Number.Split('.').Length)
                        {
                            keepFirstPage = false;
                        }
                        int endPage;
                        if (_contentItems.IndexOf(item) + 1 >= _contentItems.Count)
                        {
                            endPage = _document.NumberOfPages;
                        }
                        else
                        {
                            endPage = _contentItems[_contentItems.IndexOf(item) + 1].Page;
                        }

                        for (int j = startPage + (keepFirstPage ? 1 : 0); j < endPage; j++)
                        {
                            pages.Remove(j);
                        }
                    }
                }
                return pages.ToList();
            }
          
        }
    }
}