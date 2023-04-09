using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Linq;
using Tabula;
using System.Data;
using System;

namespace ParserCore
{
    class XMLGenerator
    {
        static private bool IsValidAttribute(string attrib) { return attrib != null && attrib.Length > 0; }

        static public void WriteData(Data data, Stream fout)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = "\t";
            using(XmlWriter writer = XmlWriter.Create(fout, settings)){
                writer.WriteStartElement("TagList");
                foreach(Data.Parameter param in data.ReadAll()){
                    writer.WriteStartElement("Row");
                    if(IsValidAttribute(param.Name)) writer.WriteAttributeString("Param", param.Name);
                    if(IsValidAttribute(param.Description)) writer.WriteAttributeString("Description", param.Description);
                    if(IsValidAttribute(param.Range)) writer.WriteAttributeString("Range", param.Range);
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
    }
}
