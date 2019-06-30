using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GdiW3cSvgTestSuite
{
    [Serializable]
    public sealed class SvgTestCase : IXmlSerializable
    {
        private string _revision;
        private string _name;
        private IList<string> _paragraphs;

        public SvgTestCase()
        {
            _revision   = "";
            _name       = "";
            _paragraphs = new List<string>();
        }

        public string Revision
        {
            get { return _revision; }
            set { _revision = value; }
        }
        public string Name { get => _name; set => _name = value; }
        public IList<string> Paragraphs { get => _paragraphs; set => _paragraphs = value; }

        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader == null || reader.NodeType != XmlNodeType.Element)
            {
                return;
            }
            if (!string.Equals(reader.Name, "test", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }
        }
    }
}
