using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using System.Collections.Generic;
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
}