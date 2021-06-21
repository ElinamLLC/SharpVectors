using System;
using System.Xml;
using System.Windows.Media;
using System.Xml.Serialization;

namespace WpfTestOtherSvg
{
    [Serializable]
    public sealed class SvgTestItem : IXmlSerializable
    {
        #region Public Fields

        public const uint TestCount = 1350;

        #endregion

        #region Private Fields

        private uint _number;
        private string _fileName;
        private string _comment;
        private SvgTestState _state;

        private object _tag;
        private object _parent;

        #endregion

        #region Constructors and Destructor

        public SvgTestItem()
        {
            _number      = 0;
            _fileName    = string.Empty;
            _comment     = string.Empty;
            _state       = SvgTestState.Failure;
        }

        public SvgTestItem(XmlReader reader)
            : this()
        {
            this.ReadXml(reader);
        }

        public SvgTestItem(uint number, string fileName)
            : this()
        {
            this.Initialize(number, fileName);
        }

        public SvgTestItem(uint number, string fileName, string state, string comment)
            : this()
        {
            this.Initialize(number, fileName, state, comment);
        }

        #endregion

        #region Public Fields

        public bool IsEmpty
        {
            get {
                return (string.IsNullOrWhiteSpace(_fileName));
            }
        }

        public SvgTestState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public object Tag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        public object Parent
        {
            get { return _parent; }
            set { _parent = value; }
        }

        public Brush StateBrush
        {
            get {
                switch (_state)
                {
                    case SvgTestState.Unknown:
                        return Brushes.LightGray;
                    case SvgTestState.Failure:
                        return Brushes.Red;
                    case SvgTestState.Success:
                        return Brushes.Green;
                    case SvgTestState.Partial:
                        return Brushes.Yellow;
                }

                return Brushes.LightGray;
            }
        }

        public uint Number
        {
            get { return _number; }
            set { _number = value; }
        }

        public string Comment
        {
            get { return _comment; }
            set { _comment = value; }
        }

        public string FileName
        {
            get { return _fileName; }
            set { _fileName = value; }
        }

        #endregion

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
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

            // <test number=2, source="test.svg" state="partial" comment=""/>
            string number   = reader.GetAttribute("number");
            string fileName = reader.GetAttribute("filename");
            if (!string.IsNullOrWhiteSpace(fileName) && !string.IsNullOrWhiteSpace(number))
            {
                string state   = reader.GetAttribute("state");
                string comment = reader.GetAttribute("comment");

                this.Initialize(uint.Parse(number), fileName, state, comment);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            // <test number=2, source="test.svg" state="partial" comment=""/>
            writer.WriteStartElement("test");
            writer.WriteAttributeString("number", _number.ToString());
            writer.WriteAttributeString("filename", _fileName);
            writer.WriteAttributeString("state", _state.ToString().ToLowerInvariant());
            writer.WriteAttributeString("comment", _comment);
            writer.WriteEndElement();
        }

        public void Initialize(uint number, string fileName)
        {
            _number   = number;
            _fileName = fileName;
            _comment  = string.Empty;
            _state    = SvgTestState.Failure;
        }

        #endregion

        #region Private Methods

        private void Initialize(uint number, string fileName, string state, string comment)
        {
            _number   = number;
            _fileName = fileName;
            _comment  = comment;

            if (!string.IsNullOrWhiteSpace(state))
            {
                if (state.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                {
                    _state = SvgTestState.Unknown;
                }
                else if (state.Equals("failure", StringComparison.OrdinalIgnoreCase))
                {
                    _state = SvgTestState.Failure;
                }
                else if (state.Equals("success", StringComparison.OrdinalIgnoreCase))
                {
                    _state = SvgTestState.Success;
                }
                else if (state.Equals("partial", StringComparison.OrdinalIgnoreCase))
                {
                    _state = SvgTestState.Partial;
                }
            }
        }

        #endregion
    }
}
