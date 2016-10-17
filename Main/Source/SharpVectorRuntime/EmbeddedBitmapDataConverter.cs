using System;
using System.IO;
using System.ComponentModel;

namespace SharpVectors.Runtime
{
    public sealed class EmbeddedBitmapDataConverter : TypeConverter
    {
        #region Constructors and Destructor

        public EmbeddedBitmapDataConverter()
        {   
        }

        #endregion

        #region Public Methods

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof( string );

        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if( destinationType == typeof( string ) )
            {
                return true;
            }
            return base.CanConvertTo( context, destinationType );
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            byte[] toDecode = Convert.FromBase64String((string)value);

            MemoryStream memoryStream = new MemoryStream(toDecode);
            return new EmbeddedBitmapData(memoryStream);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if( destinationType == typeof(string))
            {
                EmbeddedBitmapData bitmapInfo = (EmbeddedBitmapData)value;
                MemoryStream memoryStream = bitmapInfo.Stream;

                return Convert.ToBase64String(memoryStream.ToArray());
           }

            return base.ConvertTo( context, culture, value, destinationType );
        }
        
        #endregion Methods
    }
}
