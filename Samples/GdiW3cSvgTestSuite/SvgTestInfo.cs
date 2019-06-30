using System;
using System.Drawing;
using System.Xml;
using System.Xml.Serialization;

namespace GdiW3cSvgTestSuite
{
    [Serializable]
    public sealed class SvgTestInfo : IXmlSerializable
    {
        #region Private Fields

        private string _category;
        private string _fileName;
        private string _title;
        private string _comment;
        private string _description;
        private SvgTestState _state;

        #endregion

        #region Constructors and Destructor

        public SvgTestInfo()
        {
            _title       = string.Empty;
            _fileName    = string.Empty;
            _description = string.Empty;
            _comment     = string.Empty;
            _state       = SvgTestState.Unknown;
        }

        public SvgTestInfo(XmlReader reader)
            : this()
        {
            this.ReadXml(reader);
        }

        public SvgTestInfo(string fileName, string title, string state,
            string comment, string description)
            : this()
        {
            this.Initialize(fileName, title, state, comment, description);
        }

        #endregion

        #region Public Fields

        public bool IsEmpty
        {
            get {
                return (string.IsNullOrWhiteSpace(_fileName));
            }
        }

        public string Path
        {
            get {
                return string.Format("{0}/{1}", _category, _title);
            }
        }

        public SvgTestState State
        {
            get { return _state; }
            set { _state = value; }
        }

        public Image Image
        {
            get {
                switch (_state)
                {
                    case SvgTestState.Unknown:
                        return Properties.Resources.Unknown;
                    case SvgTestState.Failure:
                        return Properties.Resources.Failure;
                    case SvgTestState.Success:
                        return Properties.Resources.Success;
                    case SvgTestState.Partial:
                        return Properties.Resources.Partial;
                }

                return Properties.Resources.Unknown;
            }
        }

        public int ImageIndex
        {
            get {
                switch (_state)
                {
                    case SvgTestState.Unknown:
                        return (int)SvgTestState.Unknown + 2;
                    case SvgTestState.Failure:
                        return (int)SvgTestState.Failure + 2;
                    case SvgTestState.Success:
                        return (int)SvgTestState.Success + 2;
                    case SvgTestState.Partial:
                        return (int)SvgTestState.Partial + 2;
                }

                return (int)SvgTestState.Unknown + 2;
            }
        }

        public string Category
        {
            get { return _category; }
            set { _category = value; }
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

        public string Title
        {
            get { return _title; }
            set {
                if (value == null)
                {
                    value = string.Empty;
                }
                _title = value;
            }
        }

        public string Description
        {
            get {
                return _description;
            }
            set {
                if (value == null)
                {
                    value = string.Empty;
                }
                _description = value;
            }
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

            // <test source="*.svg" title="" state="partial" comment="" description="" />
            string source = reader.GetAttribute("source");
            if (!string.IsNullOrWhiteSpace(source))
            {
                string title = reader.GetAttribute("title");
                if (string.IsNullOrWhiteSpace(title))
                {
                    title = source.Replace(".svg", string.Empty);
                }
                string state = reader.GetAttribute("state");
                string comment = reader.GetAttribute("comment");
                string description = reader.GetAttribute("description");

                this.Initialize(source, title, state, comment, description);
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            // <test source="*.svg" title="" state="partial" comment="" description="" />
            writer.WriteStartElement("test");
            writer.WriteAttributeString("source", _fileName);
            writer.WriteAttributeString("title", _title);
            writer.WriteAttributeString("state", _state.ToString().ToLowerInvariant());
            writer.WriteAttributeString("comment", _comment);
            writer.WriteAttributeString("description", _description);
            writer.WriteEndElement();
        }

        #endregion

        #region Private Methods

        private void Initialize(string fileName, string title, string state,
            string comment, string description)
        {
            if (description == null)
            {
                description = string.Empty;
            }

            _fileName = fileName;
            _title = title;
            _comment = comment;
            _description = description.Trim();
            if (!string.IsNullOrWhiteSpace(_description))
            {
                _description = _description.Replace("\n", " ");
                _description = _description.Replace("  ", string.Empty);
            }

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
