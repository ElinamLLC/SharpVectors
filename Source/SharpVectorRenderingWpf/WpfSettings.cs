using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using System.Collections.Generic;

using System.Windows;

namespace SharpVectors.Renderers
{
    /// <summary>
    /// This is the typed or generic <see langword="abstract"/> base class for 
    /// most objects in this build library, and it defines the basic cloneable 
    /// and serialization interfaces. This is used as the base object to create 
    /// components object hierarchy.
    /// </summary>
    /// <typeparam name="T">
    /// The underlying value type of the <see cref="WpfSettings{T}"/> generic type. 
    /// </typeparam>
    /// <remarks>
    /// This also provides a base class for component object hierarchy whose state
    /// can be serialized to an <c>XML</c> format. 
    /// </remarks>
    [Serializable]
    public abstract class WpfSettings<T> : WpfObject, ICloneable, IXmlSerializable
        where T : WpfSettings<T>
    {
        #region Protected Fields

        protected StringComparison _comparer;

        #endregion

        #region Constructors and Destructor

        /// <overloads>
        /// Initializes a new instance of the <see cref="WpfSettings{T}"/> class.
        /// </overloads>
        /// <summary>
        /// Initializes a new instance of the <see cref="WpfSettings{T}"/> class
        /// to the default properties or values.
        /// </summary>
        protected WpfSettings()
        {
            _comparer = StringComparison.OrdinalIgnoreCase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WpfSettings{T}"/> class
        /// with initial parameters copied from the specified instance of the 
        /// specified <see cref="WpfSettings{T}"/> class, a copy constructor.
        /// </summary>
        /// <param name="source">
        /// An instance of the <see cref="WpfSettings{T}"/> class from which the
        /// initialization parameters or values will be copied.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the parameter <paramref name="source"/> is <see langword="null"/>.
        /// </exception>
        protected WpfSettings(WpfSettings<T> source)
            : this()
        {
            NotNull(source, nameof(source));
        }

        #endregion

        #region IXmlSerializable Members

        /// <summary>
        /// This property is reserved, apply the <see cref="XmlSchemaProviderAttribute"/> to the class instead.
        /// </summary>
        /// <returns>
        /// An <see cref="XmlSchema"/> that describes the <c>XML</c> representation of 
        /// the object that is produced by the <see cref="WriteXml"/> method and 
        /// consumed by the <see cref="ReadXml"/> method.
        /// </returns>
        XmlSchema IXmlSerializable.GetSchema()
        {
            return null;
        }

        /// <summary>
        /// This reads and sets its state or attributes stored in a <c>XML</c> format
        /// with the given reader. 
        /// </summary>
        /// <param name="reader">
        /// The reader with which the <c>XML</c> attributes of this object are accessed.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public virtual void ReadXml(XmlReader reader)
        {
        }

        /// <summary>
        /// This writes the current state or attributes of this object,
        /// in the <c>XML</c> format, to the media or storage accessible by the given writer.
        /// </summary>
        /// <param name="writer">
        /// The <c>XML</c> writer with which the <c>XML</c> format of this object's state 
        /// is written.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If the <paramref name="reader"/> is <see langword="null"/>.
        /// </exception>
        public virtual void WriteXml(XmlWriter writer)
        {
        }

        protected static void WriteEnum(XmlWriter writer, string name, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "enum");
            writer.WriteAttributeString("value", value);
            writer.WriteEndElement();
        }

        protected static void WriteEnum(XmlWriter writer, string name, string value, string content)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            if (content == null)
            {
                content = string.Empty;
            }
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "enum");
            writer.WriteAttributeString("value", value);
            writer.WriteString(content);
            writer.WriteEndElement();
        }

        protected static void WriteCData(XmlWriter writer, string name, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteCData(value);
            writer.WriteEndElement();
        }

        protected static void WriteCData(XmlWriter writer, string name, string value, string content)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            if (content == null)
            {
                content = string.Empty;
            }
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("value", value);
            writer.WriteCData(content);
            writer.WriteEndElement();
        }

        protected static void WriteProperty(XmlWriter writer, string name, string value)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "string");
            writer.WriteAttributeString("value", value);
            writer.WriteEndElement();
        }

        protected static void WriteProperty(XmlWriter writer, string name, string value, string content)
        {
            if (value == null)
            {
                value = string.Empty;
            }

            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "string");
            if (string.IsNullOrWhiteSpace(content))
            {
                writer.WriteString(value);
            }
            else
            {
                writer.WriteAttributeString("value", value);
                writer.WriteString(content);
            }
            writer.WriteEndElement();
        }

        protected static void WriteProperty(XmlWriter writer, string name, object value)
        {
            if (value == null)
            {
                value = string.Empty;
            }
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "object");
            writer.WriteString(value.ToString());
            writer.WriteEndElement();
        }

        protected static void WriteProperty(XmlWriter writer, string name, bool value)
        {
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "bool");
            writer.WriteAttributeString("value", XmlConvert.ToString(value));
            writer.WriteEndElement();
        }

        protected static void WriteProperty(XmlWriter writer, string name, int value)
        {
            writer.WriteStartElement("property");
            writer.WriteAttributeString("name", name);
            writer.WriteAttributeString("type", "int");
            writer.WriteAttributeString("value", XmlConvert.ToString(value));
            writer.WriteEndElement();
        }

        #endregion

        #region ICloneable Members

        /// <overloads>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </overloads>
        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this build object. If you 
        /// need just a copy, use the copy constructor to create a new instance.
        /// </remarks>
        public abstract T Clone();

        /// <summary>
        /// This creates a new build object that is a deep copy of the current 
        /// instance.
        /// </summary>
        /// <returns>
        /// A new build object that is a deep copy of this instance.
        /// </returns>
        /// <remarks>
        /// This is deep cloning of the members of this style object. If you need just a copy,
        /// use the copy constructor to create a new instance.
        /// </remarks>
        object ICloneable.Clone()
        {
            return Clone();
        }

        #endregion
    }
}
