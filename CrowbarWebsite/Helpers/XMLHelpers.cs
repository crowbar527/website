using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace CrowbarWebsite.Helpers
{
    public static class XMLHelpers
    {
        public static Dictionary<string, string> ReadElement(this XmlReader xmlReader, string elementName) {
            Dictionary<string, string> _els = new Dictionary<string, string>();
            try
            {
                while (!(xmlReader.Name == elementName && xmlReader.NodeType == XmlNodeType.Element))
                {
                    xmlReader.Read();
                    if (xmlReader.EOF)
                        return new Dictionary<string, string>();
                }
                xmlReader.ReadStartElement(elementName);
                string cnode = "", cval = "";
                while (!(xmlReader.Name == elementName && xmlReader.NodeType == XmlNodeType.EndElement))
                {
                    switch (xmlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            cnode += (cnode.Length == 0 ? "" : ".") + xmlReader.Name;
                            break;
                        case XmlNodeType.Text:
                            cval = xmlReader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            if (cval != "")
                                _els.Add(cnode, cval);
                            if (cnode.Contains("."))
                                cnode = cnode.Substring(0, cnode.LastIndexOf("."));
                            else
                                cnode = "";
                            cval = "";
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    xmlReader.Read();

                }
                return _els;
            }
            catch { return new Dictionary<string, string>(); }
        }

        public static XmlReader CreateFromString(string data)
        {
            var ms = new MemoryStream(Encoding.UTF8.GetBytes(data));
            var xmlr = XmlReader.Create(ms);
            xmlr.Read();
            return xmlr;
        }
    }
}
