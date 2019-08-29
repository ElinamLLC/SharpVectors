using System;
using System.Xml;
using System.Xml.Serialization;

namespace WpfW3cSvgTestSuite
{
    [Serializable]
    public sealed class SvgTestCategory : IXmlSerializable
    {
        #region Private Fields
 
        private string _label;

        private int _total;
        private int _unknowns;
        private int _failures;
        private int _successes;
        private int _partials;

        #endregion

        #region Constructors and Destructor

        public SvgTestCategory()
        {
            _label = "";
        }

        public SvgTestCategory(string label)
        {
            _label = label;
        }

        public SvgTestCategory(XmlReader reader)
            : this()
        {
            this.ReadXml(reader);
        }

        public SvgTestCategory(string label, int total, int unknowns, int failures, int successes, int partials)
        {
            _label = label;

            this.SetValues(total, unknowns, failures, successes, partials);
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get {
                if (string.IsNullOrWhiteSpace(_label) || _total == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public string Label
        {
            get {
                return _label;
            }
            set {
                _label = value;
            }
        }

        public int Total
        {
            get {
                return _total;
            }
            set {
                _total = value;
            }
        }

        public int Unknowns
        {
            get {
                return _unknowns;
            }
            set {
                _unknowns = value;
            }
        }

        public int Failures
        {
            get {
                return _failures;
            }
            set {
                _failures = value;
            }
        }

        public int Successes
        {
            get {
                return _successes;
            }
            set {
                _successes = value;
            }
        }

        public int Partials
        {
            get {
                return _partials;
            }
            set {
                _partials = value;
            }
        }

        #endregion

        #region Public Method

        public void SetValues(int total, int unknowns, int failures, int successes, int partials)
        {
            _total     = total;
            _unknowns  = unknowns;
            _failures  = failures;
            _successes = successes;
            _partials  = partials;
        }

        public bool IsEqualTo(SvgTestCategory other)
        {
            if (other == null)
            {
                return false;
            }
            if (!string.Equals(_label, other._label))
            {
                return false;
            }
            if (_total != other._total)
            {
                return false;
            }
            if (_unknowns != other._unknowns)
            {
                return false;
            }
            if (_failures != other._failures)
            {
                return false;
            }
            if (_successes != other._successes)
            {
                return false;
            }
            if (_partials != other._partials)
            {
                return false;
            }
            return true;
        }

        public static bool AreEqual(SvgTestCategory left, SvgTestCategory right)
        {
            if (left == null && right == null)
            {
                return true;
            }
            if (left == null && right != null)
            {
                return false;
            }
            if (left != null && right == null)
            {
                return false;
            }
            return left.IsEqualTo(right);
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
            if (!string.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // <category label="" total="0" unknowns="0" failures="0" successes="0" partials="0"/>
            string label = reader.GetAttribute("label");
            if (!string.IsNullOrWhiteSpace(label))
            {
                _label = label;

                _total     = ToInt(reader.GetAttribute("total"    ));
                _unknowns  = ToInt(reader.GetAttribute("unknowns" ));
                _failures  = ToInt(reader.GetAttribute("failures" ));
                _successes = ToInt(reader.GetAttribute("successes"));
                _partials  = ToInt(reader.GetAttribute("partials" ));
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null)
            {
                return;
            }

            // <category label="" total="0" unknowns="0" failures="0" successes="0" partials="0"/>
            writer.WriteStartElement("category");
            writer.WriteAttributeString("label", _label);
            writer.WriteAttributeString("total", _total.ToString());
            writer.WriteAttributeString("unknowns", _unknowns.ToString());
            writer.WriteAttributeString("failures", _failures.ToString());
            writer.WriteAttributeString("successes", _successes.ToString());
            writer.WriteAttributeString("partials", _partials.ToString());
            writer.WriteEndElement();
        }

        #endregion

        #region Private Methods

        private static int ToInt(string attribute)
        {
            int value = 0;
            if (!string.IsNullOrWhiteSpace(attribute) && int.TryParse(attribute, out value))
            {
                return value;
            }
            return 0;
        }

        #endregion
    }
}
