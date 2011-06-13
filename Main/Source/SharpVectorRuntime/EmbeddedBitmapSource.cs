using System;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace SharpVectors.Runtime
{
    public class EmbeddedBitmapSource : BitmapSource
    {
        #region Private Fields

        private BitmapImage  _bitmap;
        private MemoryStream _stream;

        #endregion Fields

        #region Constructors and Destructor

        public EmbeddedBitmapSource()
            : base()
        {
            //
            // Set the _useVirtuals private fields of BitmapSource to true. otherwise you will not be able to call BitmapSource methods.
            FieldInfo field = typeof(BitmapSource).GetField("_useVirtuals", 
                BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(this, true);
        }

        // ------------------------------------------------------------------

        public EmbeddedBitmapSource(MemoryStream stream)
            : this()
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            _stream = stream;
            //
            // Associated this class with source.
            this.BeginInit();

            _bitmap = new BitmapImage();

            _bitmap.BeginInit();
            _bitmap.StreamSource = _stream;
            _bitmap.EndInit();

            this.InitWicInfo(_bitmap);
            this.EndInit();
        }

        #endregion Constructors

        #region Public Properties

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public EmbeddedBitmapData Data
        {
            get
            {
                return new EmbeddedBitmapData(_stream);
            }
            set
            {
                BeginInit();

                _stream = value.Stream;

                _bitmap = new BitmapImage();
                _bitmap.BeginInit();
                _bitmap.StreamSource = _stream;
                _bitmap.EndInit();

                InitWicInfo(_bitmap);
                EndInit();
            }
        }

        #endregion Properties

        /// <summary>
        /// In the designer Data is not set. To prevent exceptions when displaying in the Designer, add a dummy bitmap.
        /// </summary>
        private void EnsureStream()
        {
            if (_stream == null)
            {
                BitmapSource dummyBitmap = BitmapSource.Create(1, 1, 96.0, 96.0,
                    PixelFormats.Pbgra32, null, new byte[] { 0, 0, 0, 0 }, 4);
                PngBitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(dummyBitmap));
                MemoryStream stream = new MemoryStream();
                encoder.Save(stream);
                Data = new EmbeddedBitmapData(stream);
            }
        }

        public override double DpiX
        {
            get
            {
                EnsureStream();
                return base.DpiX;
            }
        }

        public override double DpiY
        {
            get
            {
                EnsureStream();
                return base.DpiY;
            }
        }

        public override System.Windows.Media.PixelFormat Format
        {
            get
            {
                EnsureStream();
                return base.Format;
            }
        }

        public override BitmapPalette Palette
        {
            get
            {
                EnsureStream();
                return base.Palette;
            }
        }

        public override int PixelWidth
        {
            get
            {
                EnsureStream();
                return base.PixelWidth;
            }
        }

        public override int PixelHeight
        {
            get
            {
                EnsureStream();
                return base.PixelHeight;
            }
        }

        public override void CopyPixels(Int32Rect sourceRect, IntPtr buffer, int bufferSize, int stride)
        {
            EnsureStream();
            base.CopyPixels(sourceRect, buffer, bufferSize, stride);
        }

        #region Protected Methods

        protected override void CloneCore(Freezable sourceFreezable)
        {
            EmbeddedBitmapSource cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.CloneCore( sourceFreezable );
        }

        protected override void CloneCurrentValueCore(Freezable sourceFreezable)
        {
            EmbeddedBitmapSource cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.CloneCurrentValueCore( sourceFreezable );
        }

        protected override void GetAsFrozenCore(Freezable sourceFreezable)
        {
            EmbeddedBitmapSource cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.GetAsFrozenCore( sourceFreezable );
        }

        protected override void GetCurrentValueAsFrozenCore(Freezable sourceFreezable)
        {
            EmbeddedBitmapSource cloneSource = (EmbeddedBitmapSource)sourceFreezable;
            CopyFrom(cloneSource);
            //base.GetCurrentValueAsFrozenCore( sourceFreezable );
        }

        protected override Freezable CreateInstanceCore()
        {
            return new EmbeddedBitmapSource();
        }

        #endregion Override Methods

        #region Private Methods

        /// <summary>
        /// Call BeginInit every time the WICSourceHandle is going to be change.
        /// again this methods is not exposed and reflection is needed.
        /// </summary>
        private void BeginInit()
        {
            FieldInfo field = typeof(BitmapSource).GetField(
                "_bitmapInit", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo beginInit = field.FieldType.GetMethod(
                "BeginInit", BindingFlags.Public | BindingFlags.Instance);
            beginInit.Invoke(field.GetValue(this), null);
        }

        /// <summary>
        /// Call EndInit after the WICSourceHandle was changed and after using BeginInit.
        /// again this methods is not exposed and reflection is needed.
        /// </summary>
        private void EndInit()
        {
            FieldInfo field = typeof(BitmapSource).GetField(
                "_bitmapInit", BindingFlags.NonPublic | BindingFlags.Instance);
            MethodInfo endInit = field.FieldType.GetMethod(
                "EndInit", BindingFlags.Public | BindingFlags.Instance);
            endInit.Invoke(field.GetValue(this), null);
        }

        /// <summary>
        /// Set the WicSourceHandle property with the source associated with this class.
        /// again this methods is not exposed and reflection is needed.
        /// </summary>
        /// <param name="source"></param>
        private void InitWicInfo(BitmapSource source)
        {
            //
            // Use reflection to get the private property WicSourceHandle Get and Set methods.
            PropertyInfo wicSourceHandle = typeof(BitmapSource).GetProperty(
                "WicSourceHandle", BindingFlags.NonPublic | BindingFlags.Instance);

            MethodInfo wicSourceHandleGetMethod = wicSourceHandle.GetGetMethod(true);
            MethodInfo wicSourceHandleSetMethod = wicSourceHandle.GetSetMethod(true);
            //
            // Call the Get method of the WicSourceHandle of source.
            object wicHandle = wicSourceHandleGetMethod.Invoke(source, null);
            //
            // Call the Set method of the WicSourceHandle of this with the value from source.
            wicSourceHandleSetMethod.Invoke(this, new object[] { wicHandle });
        }

        private void CopyFrom(EmbeddedBitmapSource source)
        {
            this.BeginInit();

            _bitmap = source._bitmap;

            this.InitWicInfo(_bitmap);
            this.EndInit();
        }

        #endregion Methods
    }
}
