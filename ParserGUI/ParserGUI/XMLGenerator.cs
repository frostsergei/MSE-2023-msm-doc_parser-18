using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using Tabula;
using System.Data;
using System;

namespace ParserGUI
{
    class XMLGenerator
    {
        static public void ToFile(List<Table> tables, FileStream fout)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = "\t";
            using(XmlWriter writer = XmlWriter.Create(fout, settings))
                WriteParseResult(tables, writer);
        }
        
        static private void WriteParseResult(List<Table> tables, XmlWriter writer)
        {
            writer.WriteStartElement("parameters");
            foreach(Table table in tables){
                bool no_table_yet = true;
                foreach(IReadOnlyList<Cell> row in table.Rows){
                    bool no_row_yet = true;
                    foreach(Cell cell in row){
                        bool no_text_yet = true;
                        foreach(TextChunk chunk in cell.TextElements){
                            foreach(TextElement elem in chunk.TextElements){
                                if(no_table_yet){
                                    no_table_yet = false;
                                    writer.WriteStartElement("table");
                                }
                                if(no_row_yet){
                                    no_row_yet = false;
                                    writer.WriteStartElement("row");
                                }
                                if(no_text_yet){
                                    no_text_yet = false;
                                    writer.WriteStartElement("cell");
                                }
                                writer.WriteString(elem.GetText());
                            }
                        }
                        if(!no_text_yet)
                            writer.WriteEndElement();
                    }
                    if(!no_row_yet)
                        writer.WriteEndElement();
                }
                if(!no_table_yet)
                    writer.WriteEndElement(); 
            }
            writer.WriteEndElement();
        }
    }
}
