using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
using Tabula;
using ParserGUI;
using System.IO;
using System;
using System.Xml;

namespace ParserCore
{
    public class Parser
    {
        public string ParseSimpleTable(TabulaParser parser, List<int> page_numbers)
        {
            MemoryStream str = new MemoryStream();
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = "\t";
            XmlWriter writer = XmlWriter.Create(str, settings);

            List<string>[] header_sentences = new List<string>[]{new List<string>{"Номер","элемента","списка" },
                                                                 new List<string>{"Значение", "элемента", "адрес", "и", "признаки", "вывода", "на", "печать"},
                                                                 new List<string>{"Наименование", "элемента", "и", "комментарии"} };

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
}