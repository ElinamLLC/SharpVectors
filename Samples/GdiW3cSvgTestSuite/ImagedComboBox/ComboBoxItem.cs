//--------------------------------------------------------------------------
// Source: https://www.codeproject.com/Articles/106467/How-to-Display-Images-in-ComboBox-in-5-Minutes
// Author: Bassam Alugili
//--------------------------------------------------------------------------

using System;
using System.Drawing;

namespace GdiW3cSvgTestSuite
{
    /// <summary>
    /// This class represents an ComboBox item of the ImagedComboBox which may contains an image and value.
    /// 
    /// </summary>
    [Serializable]
    public class ComboBoxItem
    {
        private object _tag;
        private object _value;
        private Image _image;

        /// <summary>
        /// ComobBox Item.
        /// </summary>
        public object Value
        {
            get {
                return _value;
            }
            set {
                _value = value;
            }
        }

        /// <summary>
        /// ComobBox Tag.
        /// </summary>
        public object Tag
        {
            get {
                return _tag;
            }
            set {
                _tag = value;
            }
        }

        /// <summary>
        /// Item image.
        /// </summary>
        public Image Image
        {
            get {
                return _image;
            }
            set {
                _image = value;
            }
        }

        public ComboBoxItem()
        {
            _value = string.Empty;
            _image = new Bitmap(1, 1);
        }

        /// <summary>
        /// Constructor item without image.
        /// </summary>
        /// <param name="value">Item value.</param>
        public ComboBoxItem(object value)
        {
            _value = value;
            _image = new Bitmap(1, 1);
        }

        /// <summary>
        ///  Constructor item with image.
        /// </summary>
        /// <param name="value">Item value.</param>
        /// <param name="image">Item image.</param>
        public ComboBoxItem(object value, Image image)
        {
            _value = value;
            _image = image;
        }

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
