using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;

namespace ParserGUI
{
    class XMLGenerator
    {
        static public void ToFile(string parse_result, FileStream fout)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = "\t";
            using(XmlWriter writer = XmlWriter.Create(fout, settings))
                WriteParseResult(parse_result, writer);
        }
        
        static private void WriteParseResult(string parse_result, XmlWriter writer)
        {
            writer.WriteStartElement("text");
            string[] lines = parse_result.Split('\n');
            foreach(string l in lines)
            {
                string l_out = l.Replace("\r", "");
                if(l_out == "")
                    continue;
                writer.WriteStartElement("line");
                writer.WriteString(l_out); 
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }
    }
}
