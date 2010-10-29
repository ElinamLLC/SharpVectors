using System;

namespace SharpVectors.Converters
{
    /// <summary>
    /// <para>
    /// This specifies the encoder type used to encode a collection of 
    /// bitmap frame objects to an image stream.
    /// </para>
    /// <para>
    /// This is used by the <see cref="ImageSvgConverter"/> converter.
    /// </para>
    /// </summary>
    /// <seealso cref="ImageSvgConverter"/>
    public enum ImageEncoderType
    {
        /// <summary>
        /// Specifies an encoder that is used to encode bitmap (BMP) 
        /// format images, that is 
        /// <see cref="System.Windows.Media.Imaging.BmpBitmapEncoder"/>. 
        /// </summary>
        BmpBitmap  = 0,
        /// <summary>
        /// Specifies an encoder that is used to encode Graphics Interchange 
        /// Format (GIF) images, that is 
        /// <see cref="System.Windows.Media.Imaging.GifBitmapEncoder"/>. 
        /// </summary>
        GifBitmap  = 1,
        /// <summary>
        /// Specifies an encoder that is used to encode Joint Photographics 
        /// Experts Group (JPEG) format images, that is 
        /// <see cref="System.Windows.Media.Imaging.JpegBitmapEncoder"/>. 
        /// </summary>
        JpegBitmap = 2,
        /// <summary>
        /// Specifies an encoder that is used to encode Portable Network 
        /// Graphics (PNG) format images, that is 
        /// <see cref="System.Windows.Media.Imaging.PngBitmapEncoder"/>. 
        /// </summary>
        PngBitmap  = 3,
        /// <summary>
        /// Specifies an encoder that is used to encode Tagged Image File 
        /// Format (TIFF) images, that is 
        /// <see cref="System.Windows.Media.Imaging.TiffBitmapEncoder"/>. 
        /// </summary>
        TiffBitmap = 4,
        /// <summary>
        /// Specifies an encoder that is used to Microsoft Windows Media Photo 
        /// (WDP) images, that is 
        /// <see cref="System.Windows.Media.Imaging.WmpBitmapEncoder"/>. 
        /// </summary>
        WmpBitmap  = 5
    }
}
