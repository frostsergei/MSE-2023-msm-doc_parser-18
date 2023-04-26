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

        static private string NormalizeField(string s)
        {
            IEnumerable<char> c_out = s.Normalize(NormalizationForm.FormD);
            return new string(c_out.Where(c => !char.IsControl(c)).ToArray());
        }

        static public void WriteData(Data data, Stream fout)
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true; settings.IndentChars = "\t";
            using(XmlWriter writer = XmlWriter.Create(fout, settings)){
                writer.WriteStartElement("TagList");
                foreach(Data.Parameter param in data.ReadAll()){
                    writer.WriteStartElement("Row");
                    if(IsValidAttribute(param.Name)) writer.WriteAttributeString("Param", NormalizeField(param.Name));
                    if(IsValidAttribute(param.Description)) writer.WriteAttributeString("Description", NormalizeField(param.Description));
                    if(IsValidAttribute(param.Range)) writer.WriteAttributeString("Range", NormalizeField(param.Range));
                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }
    }
}
