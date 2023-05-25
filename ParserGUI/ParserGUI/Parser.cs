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
using System.Runtime.InteropServices.WindowsRuntime;
using static ParserCore.Data;

namespace ParserCore
{
    public class Parser
    {
        private List<int> _pageNumbers;
        private PdfDocument _document;
        private TabulaParser tabparser;

        private Data data = new Data();
        public Data GetData() { return data; }

        public Parser(string filename)
        {
            tabparser = new TabulaParser(filename, new NearestNeighbourTextParser());
            _document = PdfDocument.Open(filename, new ParsingOptions() { ClipPaths = true });
            ParseContent(_document);

            foreach(int pnum in _pageNumbers){
                if(ParseDoubleRowTable(tabparser, new List<int>{pnum})){}
                else if(ParseSimpleTable(tabparser, new List<int>{pnum})){}
                else if(ParseSimplestTable(tabparser, new List<int>{pnum})){}
                else if(ParseLineParams(new List<int>{pnum})){}
                else if(ParseStringParams(new List<int>{pnum})){}
                else if(ParseParagraphParams(new List<int>{pnum})){}
            }
        }

        public void ParseContent(PdfDocument document)
        {
            var documentContentParser = new DocumentContentParser(document);
            _pageNumbers = documentContentParser.Parse();
        }

        private Tuple<string,string> ProcessDescription(string desc)
        {
            var openCurvyBracketIndex = desc.IndexOf('{');
            var closeCurvyBracketIndex = desc.IndexOf('}');
           
            var hasRange = openCurvyBracketIndex != -1 && closeCurvyBracketIndex != -1;
           
            string range = "";
            try
            {
                if (hasRange)
                {
                    range = desc.Substring(openCurvyBracketIndex + 1, closeCurvyBracketIndex - openCurvyBracketIndex - 1);
                    desc = desc.Substring(0, openCurvyBracketIndex) + desc.Substring(closeCurvyBracketIndex + 1, desc.Length - closeCurvyBracketIndex - 1);
                    var openSqBracketIndex = desc.IndexOf('[');
                    var closeSqBracketIndex = desc.IndexOf(']');
                    var hasUnit = hasRange && openSqBracketIndex != -1 && closeSqBracketIndex != -1;
                    if (hasUnit && false)
                    {
                        range += " " + desc.Substring(openSqBracketIndex, closeSqBracketIndex - openSqBracketIndex + 1);
                        desc = desc.Substring(0, openSqBracketIndex) + desc.Substring(closeSqBracketIndex + 1, desc.Length - closeSqBracketIndex - 1);
                    }
                }

                List<int> dashIndices = desc.ToList().Select((c, i) =>
                {
                    return c == '-' ? i : -1;
                }).Where(index =>
                {
                    if (index == -1) return false;
                    if (index == 0 && index == desc.Length - 1) return false;
                    if (desc[index - 1] != ' ' && desc[index + 1] != ' ')
                    {
                        return true;
                    }
                    return false;
                }).ToList();
                if (dashIndices.Count > 0)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(desc.Substring(0, dashIndices[0]));
                    for (int i = 0; i < dashIndices.Count; i++)
                    {
                        var end = desc.Length - 1;
                        if (i + 1 < dashIndices.Count)
                        {
                            end = dashIndices[i + 1];
                        }
                        sb.Append(desc.Substring(dashIndices[i] + 1, end - dashIndices[i] - 1));
                    }
                    desc = sb.ToString();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            // Удаление идущих "подряд" (через пробельные символы) точек
            for(int i = 0; (i = desc.IndexOf('.', i)) != -1; ++i){
                int j = i + 1;
                if(j >= desc.Length)
                    break;
                while(j < desc.Length - 1 && Char.IsWhiteSpace(desc[j]))
                    ++j;
                //Console.WriteLine("between " + i + "("+ desc[i] + ") and " + j + "(" + desc[j] + ")");
                if(desc[j] == '.')
                    desc = desc.Remove(i + 1, j - i);
            }
            // Удаление идущих подряд точек и пробелов
            while(desc.IndexOf("..") != -1)
                desc = desc.Replace("..", ".");
            while(desc.IndexOf("  ") != -1)
                desc = desc.Replace("  ", " ");

            return new Tuple<string, string>(range, desc);
        }


        public bool ParseLineParams(List<int> page_numbers)
        {
            bool got_params = false; 
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
                    if(names.Count() > 0)
                        got_params = true;
                    foreach(var name in names)
                    {
                        var rangedesc = ProcessDescription(parameter.Item2.Replace("\r", "").Replace('\n', ' ').Trim());
                        data.WriteElem(new Data.Parameter {
                            Name = name,
                            Description = rangedesc.Item2,
                            Range = rangedesc.Item1
                        });
                    }
                }

            }
            return got_params;
        }


        private int parse_simplest_table_last_page;
        public bool ParseSimplestTable(TabulaParser parser, List<int> page_numbers)
        {
            List<string>[] header_sentences = new List<string>[]{new List<string>{"Параметр" },
                                                                 new List<string>{"Наименование", "параметра" },
                                                                 new List<string>{"Краткое", "описание"} };
            bool got_params = false;
            bool got_header = (page_numbers[0] - parse_simplest_table_last_page == 1);
            if(page_numbers[0] - parse_simplest_table_last_page == 1)
                parse_simplest_table_last_page = page_numbers.Last();
            foreach (int page_num in page_numbers)
            {
                List<Table> tables = parser.ParsePage(page_num);
                foreach (Table table in tables)
                {
                    foreach (IReadOnlyList<Cell> row in table.Rows)
                    {
                        if(row.Count == 0)
                            continue;
                        if(row.Count != 2)
                            return false;
                        List<Data.Parameter> _params = new List<Data.Parameter>();
                        bool wrote_row = false;
                        for (int i = 0; i < row.Count; ++i)
                        {
                            Cell cell = row[i];
                            string cell_text = "";
                            bool cell_text_bold = true;
                            foreach (TextChunk chunk in cell.TextElements)
                            {
                                foreach (TextElement elem in chunk.TextElements)
                                {
                                    cell_text += elem.GetText();
                                    if(!elem.Font.Name.ToLower().Contains("bold"))
                                        cell_text_bold = false;
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
                            if(row_is_header && has_valid_header_words){
                                got_header = true; 
                                parse_simplest_table_last_page = page_num;
                                break;
                            }

                            if(!got_header)
                                break;

                            cell_text = cell_text.Trim();
                            if(cell_text.Length == 0)
                                continue;

                            if(!wrote_row)
                                wrote_row = true;
                            switch (i)
                            {
                                case 0:
                                    if(!cell_text_bold)
                                        goto end_row;
                                    foreach(string pname in cell_text.Split(',')){
                                        Data.Parameter param = new Data.Parameter();
                                        param.Name = pname.Trim();
                                        _params.Add(param);
                                    }
                                    break;
                                case 1:
                                    for(int j = 0; j < _params.Count; ++j){
                                        Data.Parameter param = _params[j];
                                        var rangedesc = ProcessDescription(cell_text);
                                        param.Description = rangedesc.Item2;
                                        param.Range = rangedesc.Item1;
                                        _params[j] = param;
                                    }
                                    break;
                            }
                        }
                        end_row:
                        if(wrote_row){
                            foreach(Data.Parameter param in _params)
                                data.WriteElem(param);
                            got_params = true;
                        }
                    }
                }
            }
            return got_params;
        }
      
        private int parse_simple_table_last_page;
        public bool ParseSimpleTable(TabulaParser parser, List<int> page_numbers)
        {
            List<string>[] header_sentences = new List<string>[]{new List<string>{"Номер","элемента","списка" },
                                                                 new List<string>{"Значение", "элемента", "адрес", "и", "признаки", "вывода", "на", "печать"},
                                                                 new List<string>{"Наименование", "элемента", "и", "комментарии"} };
            bool got_params = false;
            bool got_header = (page_numbers[0] - parse_simple_table_last_page == 1);
            if(page_numbers[0] - parse_simple_table_last_page == 1)
                parse_simple_table_last_page = page_numbers.Last();
            foreach (int page_num in page_numbers)
            {
                parse_simple_table_last_page = page_num;
                List<Table> tables = parser.ParsePage(page_num);
                foreach (Table table in tables)
                {
                    foreach (IReadOnlyList<Cell> row in table.Rows)
                    {
                        if(row.Count == 0)
                            continue;
                        if(row.Count != 3)
                            return false;
                        List<Data.Parameter> _params = new List<Data.Parameter>();
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
                            if(row_is_header && has_valid_header_words){
                                got_header = true; 
                                parse_simple_table_last_page = page_num;
                                break;
                            }

                            if(!got_header)
                                break;

                            cell_text = cell_text.Trim();
                            if(cell_text.Length == 0)
                                continue;

                            if(!wrote_row)
                                wrote_row = true;
                            switch(i)
                            {
                                case 0: 
                                    foreach(string pname in cell_text.Split(',')){
                                        Data.Parameter param = new Data.Parameter();
                                        param.Name = pname.Trim();
                                        _params.Add(param);
                                    }
                                    break;
                                case 1:
                                    // Адрес параметра, не используется
                                    break;
                                case 2:
                                    for(int j = 0; j < _params.Count; ++j){
                                        Data.Parameter param = _params[j];
                                        var rangedesc = ProcessDescription(cell_text);
                                        param.Description = rangedesc.Item2;
                                        param.Range = rangedesc.Item1;
                                        _params[j] = param;
                                    }
                                    break;
                            }
                        }
                        if(wrote_row){
                            foreach(Data.Parameter param in _params)
                                data.WriteElem(param);
                            got_params = true;
                        }
                    }
                }
            }
            return got_params;
        }

        public bool ParseDoubleRowTable(TabulaParser parser, List<int> page_numbers)
        {
            bool got_params = false;
            List<string> range_white_words = new List<string>{"Строка", "Опр.", "XXXX"};
            foreach(int page_num in page_numbers)
            {
                List<Table> tables = parser.ParsePage(page_num);
                foreach (Table table in tables)
                {
                    uint detected_row = 0;
                    Data.Parameter param = new Data.Parameter();
                    for(int j = 0; j < table.Rows.Count; ++j){ // Таблица без жирного заголовка, в которой почему-то в каждом ряду 4 ячейки (СПЕ543)
                        IReadOnlyList<Cell> row = table.Rows[j];
                        if(j < table.Rows.Count - 1 && (row.Count == 4 && table.Rows[j + 1].Count == 4)){
                            for(int i = 0; i < row.Count; ++i)
                            {
                                Cell cell = row[i];
                                string cell_text = "";
                                foreach (TextChunk chunk in cell.TextElements)
                                    foreach (TextElement elem in chunk.TextElements)
                                        cell_text += elem.GetText();
                                cell_text = cell_text.Trim();
                                switch(i){
                                    case 0:
                                        param.Description = param.Range = null;
                                        if(cell_text.Length == 0 || !Char.IsDigit(cell_text[0]))
                                            goto end_table;
                                        param.Name = cell_text;
                                        break;
                                    case 2:
                                        bool detected_range = cell_text.Length > 0 && Char.IsDigit(cell_text[0]);
                                        for(int k = 0; k < range_white_words.Count && !detected_range; ++k)
                                            if(cell_text.IndexOf(range_white_words[k]) == 0)
                                                detected_range = true;
                                        if(detected_range)
                                            param.Range = cell_text;
                                        break;
                                }
                            }
                            param.Description = "";
                            IReadOnlyList<Cell> row2 = table.Rows[j + 1];
                            for(int i = 0; i < row2.Count; ++i)
                            {
                                Cell cell = row2[i];
                                string cell_text = "";
                                foreach (TextChunk chunk in cell.TextElements)
                                    foreach (TextElement elem in chunk.TextElements)
                                        cell_text += elem.GetText();
                                param.Description += cell_text.Trim();
                            }
                            if(param.Name.Length > 0 && param.Description.Length > 0){
                                data.WriteElem(param);
                                got_params = true;
                            }
                            ++j;
                            end_table:;
                        }
                        else{ // Таблица с жирным заголовком, возможны пустые таблицы между рядами
                            if(row.Count == 0)
                                continue;
                            if(detected_row == 0 && row.Count != 4)
                                return false;

                            for(int i = 0; i < row.Count; ++i)
                            {
                                Cell cell = row[i];
                                string cell_text = "";
                                foreach (TextChunk chunk in cell.TextElements)
                                {
                                    foreach (TextElement elem in chunk.TextElements)
                                    {
                                        if(elem.Font.Name.ToLower().Contains("bold") && i == 0)
                                            detected_row = 2;
                                        cell_text += elem.GetText();
                                    }
                                }
                                cell_text = cell_text.Trim();
                                if (cell_text.Length == 0 || detected_row == 0)
                                    continue;
    
                                if(detected_row == 2){ // Заголовок таблицы
                                    switch(i){
                                        case 0:
                                            param.Description = param.Range = null;
                                            param.Name = cell_text;
                                            break;
                                        case 2:
                                            bool detected_range = cell_text.Length > 0 && Char.IsDigit(cell_text[0]);
                                            for(int k = 0; k < range_white_words.Count && !detected_range; ++k)
                                                if(cell_text.IndexOf(range_white_words[k]) == 0)
                                                    detected_range = true;
                                            if(detected_range)
                                                param.Range = cell_text;
                                            break;
                                    }
                                }
                                else{ // Описание
                                    if(i == 0){
                                        param.Description = cell_text;
                                        data.WriteElem(param);
                                        got_params = true;
                                    }
                                }
                            if(detected_row > 0)
                                --detected_row;
                            }
                        }
                    }
                }
            }
            return got_params;
        }

        private class TableColumn
        {
            public double left;
            public List<Word> words = new List<Word>();
        }

        private class WordComparer : IComparer<Word>
        {
            public WordComparer(double top_margin) { this.top_margin = top_margin; }
            private double top_margin;
            public int Compare(Word word1, Word word2)
            {
                //double top_margin = Math.Max(word1.BoundingBox.Height, word2.BoundingBox.Height);
                if(object.ReferenceEquals(word1, word2))
                    return 0;
                else
                {
                    if(Math.Abs(word1.BoundingBox.Top - word2.BoundingBox.Top) <= top_margin)
                        return word1.BoundingBox.Left < word2.BoundingBox.Left ? -1 :
                            word1.BoundingBox.Left > word2.BoundingBox.Left ? 1 : 0;
                    return word1.BoundingBox.Top < word2.BoundingBox.Top ? 1 :
                            word1.BoundingBox.Top > word2.BoundingBox.Top ? -1 : 0;
                }
            }
        }

        private List<string> param_names_blackwords = new List<string>{"текущие", "архивы", "параметры", "база", "реализация", "пример", "задача"};
        public bool ParseStringParams(List<int> pages_numbers) 
        {
            const double column_margin = 10;

            List<Data.Parameter> _params = new List<Data.Parameter>();
            foreach (int page_num in pages_numbers)
            {
                IEnumerable<Word> raw_words = _document.GetPage(page_num).GetWords();
                List<Word> words = new List<Word>(raw_words);
                words.Sort(new WordComparer(words[0].BoundingBox.Height));
                List<TableColumn> table_columns = new List<TableColumn>();
                bool parse = false;
                double last_blacklisted_y = double.PositiveInfinity;
                foreach (Word w in words)
                {
                    if (w.FontName.ToLower().Contains("bold") && w.Letters[0].PointSize >= 9
                        && (double.IsPositiveInfinity(last_blacklisted_y) || Math.Abs(w.BoundingBox.Top - last_blacklisted_y) >= w.BoundingBox.Height)){
                        bool is_blacklisted = false;
                        foreach(string bl in param_names_blackwords)
                            if(w.Text.Trim().ToLower() == bl)
                            { is_blacklisted = true; break; }
                        if(!is_blacklisted)
                            parse = true;
                        else
                            last_blacklisted_y = w.BoundingBox.Top;
                    }
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

                if(table_columns.Count < 2)
                    return false;

                for (int i = 2; i < table_columns.Count; i++)
                {
                    table_columns[1].words.AddRange(table_columns[i].words);
                }
                table_columns.RemoveRange(2, table_columns.Count - 2);
                table_columns[1].words.Sort(new WordComparer(table_columns[1].words[0].BoundingBox.Height));
                int column_1_i = 0, column_2_i = 0;
                double line_height_1 = table_columns[0].words[0].BoundingBox.Height;
                double line_height_2 = table_columns[1].words[0].BoundingBox.Height;
                List<List<TableColumn>> rows = new List<List<TableColumn>>();

                int repeated_bold_gap = 0;
                while(column_1_i < table_columns[0].words.Count && column_2_i < table_columns[1].words.Count)
                {
                    if(repeated_bold_gap >= 2)
                        return false;
                    while (column_1_i < table_columns[0].words.Count && column_2_i < table_columns[1].words.Count)
                    {
                        List<TableColumn> row = new List<TableColumn>();
                        TableColumn tc1 = new TableColumn(), tc2 = new TableColumn();
                        double last_y = table_columns[0].words[column_1_i].BoundingBox.Top;
                        for (; column_1_i < table_columns[0].words.Count
                            && table_columns[0].words[column_1_i].BoundingBox.Top >= last_y - line_height_1 * 1.8; column_1_i++)
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
                            && table_columns[1].words[column_2_i].BoundingBox.Top >= last_y_2 - line_height_2 * 1.8; column_2_i++)
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
                            if(w.FontName.ToLower().Contains("bold"))
                                goto end_row;
                            description += w.Text + ' ';
                        }
                        foreach (Word w in row[0].words)
                        {
                            string param = w.Text.Replace(",", "");
                            Data.Parameter parameter = new Data.Parameter();
                            parameter.Name = param;
                            var rangedesc = ProcessDescription(description);
                            parameter.Description = rangedesc.Item2;
                            parameter.Range = rangedesc.Item1;
                            _params.Add(parameter);
                        }
                        end_row:;
                    }
                    //++column_1_i; ++column_2_i;
                    ++repeated_bold_gap;
                }
            }
            foreach(Data.Parameter param in _params)
                data.WriteElem(param);
            return _params.Count > 0;
        }

        public bool ParseParagraphParams(List<int> page_numbers)
        {
            List<Data.Parameter> _params = new List<Data.Parameter>();
            foreach(int page_num in page_numbers){
                List<Word> words = new List<Word>(_document.GetPage(page_num).GetWords());
                words.Sort(new WordComparer(words[0].BoundingBox.Height)); 
                bool scan_for_bold = true;
                int repeated_scan = 0; // повторно нашли жирное слово более чем 1 раз - не тот тип параметров
                bool check_bold = true; // проверка на то что в начале строки стоит жирное слово
                bool track_bold = true; // отслеживание жирных слов в начале строки (название параметра)
                string param_names = ""; string param_desc = "";
                for(int i = 1; i < words.Count; ++i){
                    Word word1 = words[i - 1], word2 = words[i];
                    if(word1.Text == "∆∆") // не знаю почему так возникает в СПГ742, pdfpig выдаёт такое
                        continue;
                    if(scan_for_bold){
                        if(word1.FontName.ToLower().Contains("bold") && word1.Letters[0].PointSize >= 9){
                            if(repeated_scan < 2){
                                scan_for_bold = false;
                                --i;
                            }
                            else
                                return false;
                        }
                    }
                    else{
                        bool is_bold = word1.FontName.ToLower().Contains("bold") && word1.Letters[0].PointSize >= 9;
                        if(check_bold && !is_bold){
                                scan_for_bold = true;
                                ++repeated_scan;
                        }
                        else{
                            check_bold = false;

                            if(track_bold && !is_bold){
                                track_bold = false;
                                param_desc += word1 + " ";
                            }
                            else if(track_bold) // && is_bold
                                param_names += word1 + " ";
                            else // !track_bold && !is_bold
                                param_desc += word1 + " ";

                            if(word1.FontName.ToLower().Contains("bold") && word2.FontName.ToLower().Contains("bold")
                                && word1.Letters[0].FontSize != word2.Letters[0].FontSize) // отделяем заголовок от названия параметра (СПТ942)
                               param_names = "";

                            double top_margin = (word1.BoundingBox.Height + word2.BoundingBox.Height) / 2 * 3;
                            if(i == words.Count - 1 || Math.Abs(words[i - 1].BoundingBox.Top - words[i].BoundingBox.Top) > top_margin){
                                    if(i == words.Count - 1)
                                        param_desc += word2;
                                    param_desc = param_desc.Trim();

                                    foreach(string pname in param_names.Split(',')){
                                        bool is_blacklisted = false;
                                        foreach(string bl in param_names_blackwords)
                                            if(pname.Trim().ToLower().Contains(bl))
                                            { is_blacklisted = true; break; }

                                        if(!is_blacklisted && pname.Any(c => Char.IsDigit(c) || Char.IsLetter(c))){
                                            Data.Parameter param = new Data.Parameter();
                                            param.Name = pname.Trim().Replace("∆∆", "∆"); // не знаю почему так возникает в СПГ742, pdfpig выдаёт такое
                                            var rangedesc = ProcessDescription(param_desc);
                                            param.Description = rangedesc.Item2;
                                            param.Range = rangedesc.Item1;
                                            _params.Add(param);
                                        }
                                    }
                                    param_names = ""; param_desc = "";
                                    check_bold = true; track_bold = true;
                            }
                        }
                    }
                }
            }
            foreach(Data.Parameter param in _params)
                data.WriteElem(param);
            return _params.Count > 0;
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
                string[] blackList = new string[] { "Приложение", "списки", "список", "Список", "контр", "Контр", "Структур", "Ввод", "Режим", "режим", "справка", "Справка" };

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