using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using SharpVectors.Renderers.Wpf;

namespace WpfSvgTestBox
{
    public sealed class WpfResourceSettings : ICloneable
    {
        private bool _bindToColors;
        private bool _bindToResources;
        private bool _bindPenToBrushes;

        private string _penNameFormat;
        private string _colorNameFormat;
        private string _brushNameFormat;
        private string _resourceNameFormat;

        private bool _resourceFreeze;
        private bool _useResourceIndex;
        private WpfResourceMode _resourceMode;
        private WpfResourceAccess _resourceAccess;

        private int _indentSpaces;
        private int _numericPrecision;

        public WpfResourceSettings()
        {
            _bindToColors       = true;
            _bindToResources    = true;
            _bindPenToBrushes   = true;
            _penNameFormat      = WpfDrawingResources.DefaultPenNameFormat;
            _colorNameFormat    = WpfDrawingResources.DefaultColorNameFormat;
            _brushNameFormat    = WpfDrawingResources.DefaultBrushNameFormat;
            _resourceNameFormat = WpfDrawingResources.DefaultResourceNameFormat;
            _resourceFreeze     = true;
            _useResourceIndex   = false;
            _resourceMode       = WpfResourceMode.Drawing;
            _resourceAccess     = WpfResourceAccess.Dynamic;

            _indentSpaces       = 2;
            _numericPrecision   = 4;
        }

        public WpfResourceSettings(WpfResourceSettings source)
        {
            if (source == null)
            {
                return;
            }

            _bindToColors       = source._bindToColors;
            _bindToResources    = source._bindToResources;
            _bindPenToBrushes   = source._bindPenToBrushes;
            _penNameFormat      = source._penNameFormat;
            _colorNameFormat    = source._colorNameFormat;
            _brushNameFormat    = source._brushNameFormat;
            _resourceNameFormat = source._resourceNameFormat;
            _resourceFreeze     = source._resourceFreeze;
            _useResourceIndex   = source._useResourceIndex;
            _resourceMode       = source._resourceMode;
            _resourceAccess     = source._resourceAccess;

            _indentSpaces       = source._indentSpaces;
            _numericPrecision   = source._numericPrecision;
        }

        public bool BindToColors
        {
            get {
                return _bindToColors;
            }
            set {
                _bindToColors = value;
            }
        }

        public bool BindToResources
        {
            get {
                return _bindToResources;
            }
            set {
                _bindToResources = value;
            }
        }

        public bool BindPenToBrushes
        {
            get {
                return _bindPenToBrushes;
            }
            set {
                _bindPenToBrushes = value;
            }
        }

        public string PenNameFormat
        {
            get {
                return _penNameFormat;
            }
            set {
                if (ValidateNameFormat(value, true))
                {
                    _penNameFormat = value;
                }
            }
        }

        public string ColorNameFormat
        {
            get {
                return _colorNameFormat;
            }
            set {
                if (ValidateNameFormat(value, true))
                {
                    _colorNameFormat = value;
                }
            }
        }

        public string BrushNameFormat
        {
            get {
                return _brushNameFormat;
            }
            set {
                if (ValidateNameFormat(value, true))
                {
                    _brushNameFormat = value;
                }
            }
        }

        public string ResourceNameFormat
        {
            get {
                return _resourceNameFormat;
            }
            set {
                if (value == null)
                {
                    _resourceNameFormat = null;
                }
                else if (ValidateResourceNameFormat(value))
                {
                    _resourceNameFormat = value.Trim();
                }
            }
        }

        public bool ResourceFreeze
        {
            get {
                return _resourceFreeze;
            }
            set {
                _resourceFreeze = value;
            }
        }

        public bool UseResourceIndex
        {
            get {
                return _useResourceIndex;
            }
            set {
                _useResourceIndex = value;
            }
        }

        public WpfResourceMode ResourceMode
        {
            get {
                return _resourceMode;
            }
            set {
                _resourceMode = value;
            }
        }

        public WpfResourceAccess ResourceAccess
        {
            get {
                return _resourceAccess;
            }
            set {
                _resourceAccess = value;
            }
        }

        public int IndentSpaces
        {
            get {
                return _indentSpaces;
            }
            set {
                if (value >= 0 && value <= 8)
                {
                    _indentSpaces = value;
                }
            }
        }

        public int NumericPrecision
        {
            get {
                return _numericPrecision;
            }
            set {
                if (value >= 0 && value <= 99)
                {
                    _numericPrecision = value;
                }
                else
                {
                    _numericPrecision = -1;
                }
            }
        }

        object ICloneable.Clone()
        {
            return this.Clone();
        }

        public WpfResourceSettings Clone()
        {
            var settings = new WpfResourceSettings(this);

            return settings;
        }

        public void CopyTo(WpfDrawingResources resources)
        {
            if (resources == null)
            {
                return;
            }

            resources.BindToColors       = this._bindToColors;
            resources.BindToResources    = this._bindToResources;
            resources.BindPenToBrushes   = this._bindPenToBrushes;
            resources.PenNameFormat      = this._penNameFormat;
            resources.ColorNameFormat    = this._colorNameFormat;
            resources.BrushNameFormat    = this._brushNameFormat;
            resources.ResourceNameFormat = this._resourceNameFormat;
            resources.ResourceFreeze     = this._resourceFreeze;
            resources.UseResourceIndex   = this._useResourceIndex;
            resources.ResourceMode       = this._resourceMode;
            resources.ResourceAccess     = this._resourceAccess;
        }

        public static bool ValidateNameFormat(string nameFormat, bool isRequired)
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return !isRequired;
            }

            try
            {
                int testNumber = 1;
                string nameValue = string.Format(nameFormat, testNumber);

                return string.IsNullOrWhiteSpace(nameValue) != true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                return false;
            }
        }

        public static bool ValidateResourceNameFormat(string nameFormat)
        {
            if (string.IsNullOrWhiteSpace(nameFormat))
            {
                return true;
            }

            try
            {
                var nameParameters = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                nameParameters.Add(WpfDrawingResources.TagName, string.Empty);
                nameParameters.Add(WpfDrawingResources.TagNumber, string.Empty);

                string name1   = "test1";
                string number1 = "1";
                string name2   = "test2";
                string number2 = "2";

                nameParameters[WpfDrawingResources.TagName]  = name1;
                nameParameters[WpfDrawingResources.TagNumber] = number1;
                var resourceName1 = nameParameters.Aggregate(nameFormat, (s, kv) => s.Replace(kv.Key, kv.Value));

                nameParameters[WpfDrawingResources.TagName]  = name2;
                nameParameters[WpfDrawingResources.TagNumber] = number2;
                var resourceName2 = nameParameters.Aggregate(nameFormat, (s, kv) => s.Replace(kv.Key, kv.Value));

                return !string.IsNullOrWhiteSpace(resourceName1) && 
                    !string.Equals(resourceName1, resourceName2, StringComparison.Ordinal);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Trace.TraceError(ex.Message);
                return false;
            }
        }
    }
}
