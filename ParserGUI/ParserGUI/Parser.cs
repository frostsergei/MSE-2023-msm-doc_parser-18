using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
using ParserCore;
using System;
using System.IO;
using System.Xml;
using Tabula;
using ParserGUI;
using System.IO;
using System;
using System.Xml;
using System.Linq;
using static UglyToad.PdfPig.Core.PdfSubpath;

namespace ParserCore
{
    public class Parser
    {
        private List<int> _pageNumbers;

        public Parser(string filename)
        {
            var document = PdfDocument.Open(filename);
            ParseContent(document);
        }

        public void ParseContent(PdfDocument document)
        {
            var documentContentParser = new DocumentContentParser(document);
            _pageNumbers = documentContentParser.Parse();
        }

      
        public string ParseSimpleTable(TabulaParser parser, List<int> page_numbers)
        {
            MemoryStream str = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = "\t";
            XmlWriter writer = XmlWriter.Create(str, settings);

            writer.WriteStartElement("TagList");
            foreach(int page_num in page_numbers){
                List<Table> tables = parser.ParsePage(page_num);
                foreach(Table table in tables){
                     foreach(IReadOnlyList<Cell> row in table.Rows){
                        bool wrote_row = false;  
                        for(int i = 0; i < row.Count; ++i){
                            Cell cell = row[i];
                            string cell_text = "";
                            foreach(TextChunk chunk in cell.TextElements){
                                foreach(TextElement elem in chunk.TextElements){
                                    cell_text += elem.GetText();
                                }
                            }
                            if(cell_text.Length == 0)
                                continue;
                            if(!wrote_row){
                                writer.WriteStartElement("Row");
                                wrote_row = true;
                            }
                            switch(i){
                                case 0: // Номер элемента списка
                                    writer.WriteStartAttribute("Param");
                                    writer.WriteRaw(cell_text);
                                    writer.WriteEndAttribute();
                                    break;
                                case 1:
                                    writer.WriteStartAttribute("Address");
                                    writer.WriteRaw(cell_text);
                                    writer.WriteEndAttribute();
                                    break;
                                case 2:
                                    writer.WriteStartAttribute("Description");
                                    writer.WriteRaw(cell_text);
                                    writer.WriteEndAttribute();
                                    break;
                            }
                        }
                        if(wrote_row)
                            writer.WriteEndElement();
                     }
                }
            }
            writer.WriteEndElement();

            writer.Flush();
            str.Position = 0;
            StreamReader str_rd = new StreamReader(str);
            return str_rd.ReadToEnd();
        }
    }

    private class TableColumn {
        public double left;
        public List<Word> words = new List<Word>();
    }

    private class WordComparer: IComparer<Word>
    {
        public int Compare(Word word1, Word word2)
        {
            double top_margin = (word1.BoundingBox.Height + word2.BoundingBox.Height)/2;
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

    public string ParseStringParams(List<int> pages_numbers, string doc_name)
    {
        const double column_margin = 10;
        MemoryStream str = new MemoryStream();
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true; settings.IndentChars = "\t";
        XmlWriter writer = XmlWriter.Create(str, settings);

        writer.WriteStartElement("TagList");
        foreach (int page_num in pages_numbers)
        {
            PdfDocument doc = PdfDocument.Open(doc_name);
            IEnumerable<Word> raw_words = doc.GetPage(page_num).GetWords();
            List<Word> words = new List<Word>(raw_words);
            words.Sort(new WordComparer());
            List<TableColumn> table_columns = new List<TableColumn>();
            bool parse = false;
            foreach(Word w in words)
            {
                if (w.FontName.ToLower().Contains("bold"))
                    parse = true;
                if(!parse) 
                    continue;
                bool create_new_col = true;
                foreach(TableColumn column in table_columns)
                {
                    if(Math.Abs(w.BoundingBox.Left - column.left) <= column_margin)
                    {
                        column.words.Add(w);
                        create_new_col = false;
                        break;
                    }
                }
                if(create_new_col)
                {
                    var new_col = new TableColumn();
                    new_col.left = w.BoundingBox.Left;
                    new_col.words.Add(w);
                    table_columns.Add(new_col);
                }
            }
            for(int i=2; i<table_columns.Count; i++)
            {
                table_columns[1].words.AddRange(table_columns[i].words);
            }
            table_columns.RemoveRange(2, table_columns.Count-2);
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
                for(;column_1_i < table_columns[0].words.Count 
                    && table_columns[0].words[column_1_i].BoundingBox.Top >= last_y-line_height_1*2; column_1_i++)
                {
                    if (!table_columns[0].words[column_1_i].FontName.ToLower().Contains("bold"))
                    {
                        goto end_table;
                    }
                    if(table_columns[0].words[column_1_i].BoundingBox.Top <= last_y)
                    {
                        last_y = table_columns[0].words[column_1_i].BoundingBox.Top;
                    }
                    tc1.words.Add(table_columns[0].words[column_1_i]);
                }
                double last_y_2 = table_columns[1].words[column_2_i].BoundingBox.Top;
                for (;column_2_i < table_columns[1].words.Count 
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
            foreach(List<TableColumn> row in rows)
            {
                string description = "";
                foreach (Word w in row[1].words)
                {
                    description += w.Text + ' ';
                }
                foreach (Word w in row[0].words)
                {
                    string param = w.Text.Replace(",", "");
                    writer.WriteStartElement("Row");
                    writer.WriteStartAttribute("Param");
                    writer.WriteString(param);
                    writer.WriteEndAttribute();
                    writer.WriteStartAttribute("Description");
                    writer.WriteString(description);
                    writer.WriteEndAttribute();
                    writer.WriteEndElement();
                }
            }
        }
        writer.WriteEndElement();
        writer.Flush();
        str.Position = 0;
        StreamReader str_rd = new StreamReader(str);
        return str_rd.ReadToEnd();
    }
}