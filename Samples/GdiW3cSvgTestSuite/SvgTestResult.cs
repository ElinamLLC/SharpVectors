using System;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace GdiW3cSvgTestSuite
{
    [Serializable]
    public sealed class SvgTestResult : IXmlSerializable
    {
        #region Private Fields

        private string _version;
        private DateTime _date;
        private IList<SvgTestCategory> _categories;

        #endregion

        #region Constructors and Destructor

        public SvgTestResult()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);

            _date       = DateTime.Now;
            _version    = fvi.FileVersion;
            _categories = new List<SvgTestCategory>();
        }

        public SvgTestResult(XmlReader reader)
            : this()
        {
            this.ReadXml(reader);
        }

        public SvgTestResult(string version, DateTime date, IList<SvgTestCategory> categories)
        {
            _version    = version;
            _date       = date;
            _categories = categories;
        }

        #endregion

        #region Public Properties

        public bool IsValid
        {
            get {
                if (string.IsNullOrWhiteSpace(_version) || _categories == null || _categories.Count == 0)
                {
                    return false;
                }
                return true;
            }
        }

        public string Version
        {
            get {
                return _version;
            }
            set {
                this._version = value;
            }
        }

        public DateTime Date
        {
            get {
                return _date;
            }
            set {
                this._date = value;
            }
        }

        public IList<SvgTestCategory> Categories
        {
            get {
                return _categories;
            }
            set {
                this._categories = value;
            }
        }

        #endregion

        #region Public Method

        public bool IsEqualTo(SvgTestResult other)
        {
            if (other == null)
            {
                return false;
            }
            if (!string.Equals(this._version, other._version))
            {
                return false;
            }
            if (this._categories == null && other._categories == null)
            {
                return true;
            }
            if (this._categories == null && other._categories != null)
            {
                return false;
            }
            if (this._categories != null && other._categories == null)
            {
                return false;
            }
            if (this._categories.Count != other._categories.Count)
            {
                return false;
            }

            int itemCount = _categories.Count;
            for (int i = 0; i < itemCount; i++)
            {
                if (!SvgTestCategory.AreEqual(this._categories[i], other._categories[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool AreEqual(SvgTestResult left, SvgTestResult right)
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
            if (!string.Equals(reader.Name, "result", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            if (_categories == null | _categories.Count != 0)
            {
                _categories = new List<SvgTestCategory>();
            }

            // <result version="" date="">
            //  <category label="" total="0" unknowns="0" failures="0" successes="0" partials="0"/>
            //  <category label="" total="0" unknowns="0" failures="0" successes="0" partials="0"/>
            // </result>
            string version = reader.GetAttribute("version");
            string date = reader.GetAttribute("date");
            if (!string.IsNullOrWhiteSpace(version) && !string.IsNullOrWhiteSpace(date))
            {
                _version = version;
                _date    = XmlConvert.ToDateTime(date, XmlDateTimeSerializationMode.RoundtripKind);

                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        if (string.Equals(reader.Name, "category", StringComparison.OrdinalIgnoreCase))
                        {
                            SvgTestCategory category = new SvgTestCategory(reader);
                            if (category.IsValid)
                            {
                                _categories.Add(category);
                            }
                        }
                    }
                    else if (reader.NodeType == XmlNodeType.EndElement)
                    {
                        if (string.Equals(reader.Name, "result", StringComparison.OrdinalIgnoreCase))
                        {
                            break;
                        }
                    }
                }
            }
        }

        public void WriteXml(XmlWriter writer)
        {
            if (writer == null || !this.IsValid)
            {
                return;
            }

            // <result version="" date="">
            //  <category label="" total="0" unknowns="0" failures="0" successes="0" partials="0"/>
            //  <category label="" total="0" unknowns="0" failures="0" successes="0" partials="0"/>
            // </result>
            writer.WriteStartElement("result");
            writer.WriteAttributeString("version", _version);
            writer.WriteAttributeString("date", XmlConvert.ToString(_date, XmlDateTimeSerializationMode.RoundtripKind));

            for (int i = 0; i < _categories.Count; i++)
            {
                SvgTestCategory category = _categories[i];
                if (category != null && category.IsValid)
                {
                    category.WriteXml(writer);
                }
            }

            writer.WriteEndElement();
        }

        #endregion

        #region Private Methods

        #endregion
    }
}
