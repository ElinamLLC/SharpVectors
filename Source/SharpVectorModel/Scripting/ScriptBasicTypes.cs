using System;

using SharpVectors.Dom.Svg;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgAnimatedBoolean

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedBoolean
    /// </summary>
    public sealed class JsSvgAnimatedBoolean : JsObject<ISvgAnimatedBoolean>, IJsSvgAnimatedBoolean
    {
        public JsSvgAnimatedBoolean(ISvgAnimatedBoolean baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public bool baseVal
        {
            get { return _baseObject.BaseVal; }
            set { _baseObject.BaseVal = value; }
        }

        public bool animVal
        {
            get { return _baseObject.AnimVal; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedString

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedString
    /// </summary>
    public sealed class JsSvgAnimatedString : JsObject<ISvgAnimatedString>, IJsSvgAnimatedString
    {
        public JsSvgAnimatedString(ISvgAnimatedString baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public string baseVal
        {
            get { return _baseObject.BaseVal; }
            set { _baseObject.BaseVal = value; }
        }

        public string animVal
        {
            get { return _baseObject.AnimVal; }
        }
    }

    #endregion

    #region Implementation - IJsSvgStringList

    /// <summary>
    /// Implementation wrapper for IJsSvgStringList
    /// </summary>
    public sealed class JsSvgStringList : JsObject<ISvgStringList>, IJsSvgStringList
    {
        public JsSvgStringList(ISvgStringList baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public void clear()
        {
            _baseObject.Clear();
        }

        public string initialize(string newItem)
        {
            return _baseObject.Initialize(newItem);
        }

        public string getItem(ulong index)
        {
            return _baseObject.GetItem((uint)index);
        }

        public string insertItemBefore(string newItem, ulong index)
        {
            return _baseObject.InsertItemBefore(newItem, (uint)index);
        }

        public string replaceItem(string newItem, ulong index)
        {
            return _baseObject.ReplaceItem(newItem, (uint)index);
        }

        public string removeItem(ulong index)
        {
            return _baseObject.RemoveItem((uint)index);
        }

        public string appendItem(string newItem)
        {
            return _baseObject.AppendItem(newItem);
        }

        public ulong numberOfItems
        {
            get { return _baseObject.NumberOfItems; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedEnumeration

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedEnumeration
    /// </summary>
    public sealed class JsSvgAnimatedEnumeration : JsObject<ISvgAnimatedEnumeration>, IJsSvgAnimatedEnumeration
    {
        public JsSvgAnimatedEnumeration(ISvgAnimatedEnumeration baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public ushort baseVal
        {
            get { return _baseObject.BaseVal; }
            set { _baseObject.BaseVal = value; }
        }

        public ushort animVal
        {
            get { return _baseObject.AnimVal; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedInteger

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedInteger
    /// </summary>
    public sealed class JsSvgAnimatedInteger : JsObject<ISvgAnimatedInteger>, IJsSvgAnimatedInteger
    {
        public JsSvgAnimatedInteger(ISvgAnimatedInteger baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public long baseVal
        {
            get { return _baseObject.BaseVal; }
            set { _baseObject.BaseVal = value; }
        }

        public long animVal
        {
            get { return _baseObject.AnimVal; }
        }
    }

    #endregion

    #region Implementation - IJsSvgNumber

    /// <summary>
    /// Implementation wrapper for IJsSvgNumber
    /// </summary>
    public sealed class JsSvgNumber : JsObject<ISvgNumber>, IJsSvgNumber
    {
        public JsSvgNumber(ISvgNumber baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public float value
        {
            get { return (float)_baseObject.Value; }
            set { _baseObject.Value = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedNumber

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedNumber
    /// </summary>
    public sealed class JsSvgAnimatedNumber : JsObject<ISvgAnimatedNumber>, IJsSvgAnimatedNumber
    {
        public JsSvgAnimatedNumber(ISvgAnimatedNumber baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public float baseVal
        {
            get { return (float)_baseObject.BaseVal; }
            set { _baseObject.BaseVal = value; }
        }

        public float animVal
        {
            get { return (float)_baseObject.AnimVal; }
        }
    }

    #endregion

    #region Implementation - IJsSvgNumberList

    /// <summary>
    /// Implementation wrapper for IJsSvgNumberList
    /// </summary>
    public sealed class JsSvgNumberList : JsObject<ISvgNumberList>, IJsSvgNumberList
    {
        public JsSvgNumberList(ISvgNumberList baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public void clear()
        {
            _baseObject.Clear();
        }

        public IJsSvgNumber initialize(IJsSvgNumber newItem)
        {
            var wrappedValue = _baseObject.Initialize(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public IJsSvgNumber getItem(ulong index)
        {
            var wrappedValue = _baseObject.GetItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public IJsSvgNumber insertItemBefore(IJsSvgNumber newItem, ulong index)
        {
            var wrappedValue = _baseObject.InsertItemBefore(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public IJsSvgNumber replaceItem(IJsSvgNumber newItem, ulong index)
        {
            var wrappedValue = _baseObject.ReplaceItem(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public IJsSvgNumber removeItem(ulong index)
        {
            var wrappedValue = _baseObject.RemoveItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public IJsSvgNumber appendItem(IJsSvgNumber newItem)
        {
            var wrappedValue = _baseObject.AppendItem(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgNumber>(wrappedValue, _engine) : null;
        }

        public ulong numberOfItems
        {
            get { return _baseObject.NumberOfItems; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedNumberList

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedNumberList
    /// </summary>
    public sealed class JsSvgAnimatedNumberList : JsObject<ISvgAnimatedNumberList>, IJsSvgAnimatedNumberList
    {
        public JsSvgAnimatedNumberList(ISvgAnimatedNumberList baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgNumberList baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgNumberList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgNumberList animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgNumberList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgLength

    /// <summary>
    /// Implementation wrapper for IJsSvgLength
    /// </summary>
    public sealed class JsSvgLength : JsObject<ISvgLength>, IJsSvgLength
    {
        public JsSvgLength(ISvgLength baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void newValueSpecifiedUnits(ushort unitType, float valueInSpecifiedUnits)
        {
            _baseObject.NewValueSpecifiedUnits((SvgLengthType)unitType, valueInSpecifiedUnits);
        }

        public void convertToSpecifiedUnits(ushort unitType)
        {
            _baseObject.ConvertToSpecifiedUnits((SvgLengthType)unitType);
        }

        public ushort unitType
        {
            get { return (ushort)_baseObject.UnitType; }
        }

        public float value
        {
            get { return (float)_baseObject.Value; }
            set { _baseObject.Value = value; }
        }

        public float valueInSpecifiedUnits
        {
            get { return (float)_baseObject.ValueInSpecifiedUnits; }
            set { _baseObject.ValueInSpecifiedUnits = value; }
        }

        public string valueAsString
        {
            get { return _baseObject.ValueAsString; }
            set { _baseObject.ValueAsString = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedLength

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedLength
    /// </summary>
    public sealed class JsSvgAnimatedLength : JsObject<ISvgAnimatedLength>, IJsSvgAnimatedLength
    {
        public JsSvgAnimatedLength(ISvgAnimatedLength baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsSvgLength baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgLength animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgLengthList

    /// <summary>
    /// Implementation wrapper for IJsSvgLengthList
    /// </summary>
    public sealed class JsSvgLengthList : JsObject<ISvgLengthList>, IJsSvgLengthList
    {
        public JsSvgLengthList(ISvgLengthList baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void clear()
        {
            _baseObject.Clear();
        }

        public IJsSvgLength initialize(IJsSvgLength newItem)
        {
            var wrappedValue = _baseObject.Initialize(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public IJsSvgLength getItem(ulong index)
        {
            var wrappedValue = _baseObject.GetItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public IJsSvgLength insertItemBefore(IJsSvgLength newItem, ulong index)
        {
            var wrappedValue = _baseObject.InsertItemBefore(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public IJsSvgLength replaceItem(IJsSvgLength newItem, ulong index)
        {
            var wrappedValue = _baseObject.ReplaceItem(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public IJsSvgLength removeItem(ulong index)
        {
            var wrappedValue = _baseObject.RemoveItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public IJsSvgLength appendItem(IJsSvgLength newItem)
        {
            var wrappedValue = _baseObject.AppendItem(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgLength>(wrappedValue, _engine) : null;
        }

        public ulong numberOfItems
        {
            get { return _baseObject.NumberOfItems; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedLengthList

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedLengthList
    /// </summary>
    public sealed class JsSvgAnimatedLengthList : JsObject<ISvgAnimatedLengthList>, IJsSvgAnimatedLengthList
    {
        public JsSvgAnimatedLengthList(ISvgAnimatedLengthList baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgLengthList baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgLengthList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgLengthList animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgLengthList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgAngle

    /// <summary>
    /// Implementation wrapper for IJsSvgAngle
    /// </summary>
    public sealed class JsSvgAngle : JsObject<ISvgAngle>, IJsSvgAngle
    {
        public JsSvgAngle(ISvgAngle baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void newValueSpecifiedUnits(ushort unitType, float valueInSpecifiedUnits)
        {
            _baseObject.NewValueSpecifiedUnits((SvgAngleType)unitType, valueInSpecifiedUnits);
        }

        public void convertToSpecifiedUnits(ushort unitType)
        {
            _baseObject.ConvertToSpecifiedUnits((SvgAngleType)unitType);
        }

        public ushort unitType
        {
            get { return (ushort)_baseObject.UnitType; }
        }

        public float value
        {
            get { return (float)_baseObject.Value; }
            set { _baseObject.Value = value; }
        }

        public float valueInSpecifiedUnits
        {
            get { return (float)_baseObject.ValueInSpecifiedUnits; }
            set { _baseObject.ValueInSpecifiedUnits = value; }
        }

        public string valueAsString
        {
            get { return _baseObject.ValueAsString; }
            set { _baseObject.ValueAsString = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedAngle

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedAngle
    /// </summary>
    public sealed class JsSvgAnimatedAngle : JsObject<ISvgAnimatedAngle>, IJsSvgAnimatedAngle
    {
        public JsSvgAnimatedAngle(ISvgAnimatedAngle baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgAngle baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAngle>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAngle animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAngle>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgColor

    /// <summary>
    /// Implementation wrapper for IJsSvgColor
    /// </summary>
    public class JsSvgColor : JsCssValue, IJsSvgColor
    {
        public JsSvgColor(ISvgColor baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void setRGBColor(string rgbColor)
        {
            ((ISvgColor)_baseObject).SetRgbColor(rgbColor);
        }

        public void setRGBColorICCColor(string rgbColor, string iccColor)
        {
            ((ISvgColor)_baseObject).SetRgbColorIccColor(rgbColor, iccColor);
        }

        public void setColor(ushort colorType, string rgbColor, string iccColor)
        {
            ((ISvgColor)_baseObject).SetColor((SvgColorType)colorType, rgbColor, iccColor);
        }

        public ushort colorType
        {
            get { return (ushort)((ISvgColor)_baseObject).ColorType; }
        }

        public IJsRgbColor rgbColor
        {
            get {
                var wrappedValue = ((ISvgColor)_baseObject).RgbColor;
                return (wrappedValue != null) ? CreateWrapper<IJsRgbColor>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgIccColor iccColor
        {
            get {
                var wrappedValue = ((ISvgColor)_baseObject).IccColor;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgIccColor>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgIccColor

    /// <summary>
    /// Implementation wrapper for IJsSvgIccColor
    /// </summary>
    public sealed class JsSvgIccColor : JsObject<ISvgIccColor>, IJsSvgIccColor
    {
        public JsSvgIccColor(ISvgIccColor baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public string colorProfile
        {
            get { return _baseObject.ColorProfile; }
            set { _baseObject.ColorProfile = value; }
        }

        public IJsSvgNumberList colors
        {
            get {
                var wrappedValue = _baseObject.Colors;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgNumberList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgRect

    /// <summary>
    /// Implementation wrapper for IJsSvgRect
    /// </summary>
    public sealed class JsSvgRect : JsObject<ISvgRect>, IJsSvgRect
    {
        public JsSvgRect(ISvgRect baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public float x
        {
            get { return (float)_baseObject.X; }
            set { _baseObject.X = value; }
        }

        public float y
        {
            get { return (float)_baseObject.Y; }
            set { _baseObject.Y = value; }
        }

        public float width
        {
            get { return (float)_baseObject.Width; }
            set { _baseObject.Width = value; }
        }

        public float height
        {
            get { return (float)_baseObject.Height; }
            set { _baseObject.Height = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedRect

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedRect
    /// </summary>
    public sealed class JsSvgAnimatedRect : JsObject<ISvgAnimatedRect>, IJsSvgAnimatedRect
    {
        public JsSvgAnimatedRect(ISvgAnimatedRect baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }

        public IJsSvgRect baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgRect animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgUnitTypes: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgUnitTypes
    /// </summary>
    public sealed class JsSvgUnitTypes : JsObject<ISvgUnitTypes>, IJsSvgUnitTypes
    {
        public JsSvgUnitTypes(ISvgUnitTypes baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgRenderingIntent: TODO

    /// <summary>
    /// Implementation wrapper for IJsSvgRenderingIntent
    /// </summary>
    public sealed class JsSvgRenderingIntent : JsObject<ISvgRenderingIntent>, IJsSvgRenderingIntent
    {
        public JsSvgRenderingIntent(ISvgRenderingIntent baseObject, ISvgScriptEngine engine)
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgPoint

    /// <summary>
    /// Implementation wrapper for IJsSvgPoint
    /// </summary>
    public sealed class JsSvgPoint : JsObject<ISvgPoint>, IJsSvgPoint
    {
        public JsSvgPoint(ISvgPoint baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsSvgPoint matrixTransform(IJsSvgMatrix matrix)
        {
            var wrappedValue = _baseObject.MatrixTransform(matrix.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public float x
        {
            get { return (float)((ISvgPoint)_baseObject).X; }
            set { ((ISvgPoint)_baseObject).X = value; }
        }

        public float y
        {
            get { return (float)((ISvgPoint)_baseObject).Y; }
            set { ((ISvgPoint)_baseObject).Y = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPointList

    /// <summary>
    /// Implementation wrapper for IJsSvgPointList
    /// </summary>
    public sealed class JsSvgPointList : JsObject<ISvgPointList>, IJsSvgPointList
    {
        public JsSvgPointList(ISvgPointList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void clear()
        {
            _baseObject.Clear();
        }

        public IJsSvgPoint initialize(IJsSvgPoint newItem)
        {
            var wrappedValue = _baseObject.Initialize(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint getItem(ulong index)
        {
            var wrappedValue = _baseObject.GetItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint insertItemBefore(IJsSvgPoint newItem, ulong index)
        {
            var wrappedValue = _baseObject.InsertItemBefore(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint replaceItem(IJsSvgPoint newItem, ulong index)
        {
            var wrappedValue = _baseObject.ReplaceItem(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint removeItem(ulong index)
        {
            var wrappedValue = _baseObject.RemoveItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public IJsSvgPoint appendItem(IJsSvgPoint newItem)
        {
            var wrappedValue = _baseObject.AppendItem(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public ulong numberOfItems
        {
            get { return _baseObject.NumberOfItems; }
        }
    }

    #endregion

    #region Implementation - IJsSvgMatrix

    /// <summary>
    /// Implementation wrapper for IJsSvgMatrix
    /// </summary>
    public sealed class JsSvgMatrix : JsObject<ISvgMatrix>, IJsSvgMatrix
    {
        public JsSvgMatrix(ISvgMatrix baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public IJsSvgMatrix multiply(IJsSvgMatrix secondMatrix)
        {
            var wrappedValue = _baseObject.Multiply(((ISvgMatrix)((JsSvgMatrix)secondMatrix)._baseObject));
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix inverse()
        {
            var wrappedValue = _baseObject.Inverse();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix translate(float x, float y)
        {
            var wrappedValue = _baseObject.Translate(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix scale(float scaleFactor)
        {
            var wrappedValue = _baseObject.Scale(scaleFactor);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix scaleNonUniform(float scaleFactorX, float scaleFactorY)
        {
            var wrappedValue = _baseObject.ScaleNonUniform(scaleFactorX, scaleFactorY);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix rotate(float angle)
        {
            var wrappedValue = _baseObject.Rotate(angle);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix rotateFromVector(float x, float y)
        {
            var wrappedValue = _baseObject.RotateFromVector(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix flipX()
        {
            var wrappedValue = _baseObject.FlipX();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix flipY()
        {
            var wrappedValue = _baseObject.FlipY();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix skewX(float angle)
        {
            var wrappedValue = _baseObject.SkewX(angle);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix skewY(float angle)
        {
            var wrappedValue = _baseObject.SkewY(angle);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public float a
        {
            get { return (float)_baseObject.A; }
            set { _baseObject.A = value; }
        }

        public float b
        {
            get { return (float)_baseObject.B; }
            set { _baseObject.B = value; }
        }

        public float c
        {
            get { return (float)_baseObject.C; }
            set { _baseObject.C = value; }
        }

        public float d
        {
            get { return (float)_baseObject.D; }
            set { _baseObject.D = value; }
        }

        public float e
        {
            get { return (float)_baseObject.E; }
            set { _baseObject.E = value; }
        }

        public float f
        {
            get { return (float)_baseObject.F; }
            set { _baseObject.F = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgTransform

    /// <summary>
    /// Implementation wrapper for IJsSvgTransform
    /// </summary>
    public sealed class JsSvgTransform : JsObject<ISvgTransform>, IJsSvgTransform
    {
        public JsSvgTransform(ISvgTransform baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public void setMatrix(IJsSvgMatrix matrix)
        {
            _baseObject.SetMatrix(matrix.BaseObject);
        }

        public void setTranslate(float tx, float ty)
        {
            _baseObject.SetTranslate(tx, ty);
        }

        public void setScale(float sx, float sy)
        {
            _baseObject.SetScale(sx, sy);
        }

        public void setRotate(float angle, float cx, float cy)
        {
            _baseObject.SetRotate(angle, cx, cy);
        }

        public void setSkewX(float angle)
        {
            _baseObject.SetSkewX(angle);
        }

        public void setSkewY(float angle)
        {
            _baseObject.SetSkewY(angle);
        }

        public ushort type
        {
            get { return (ushort)_baseObject.Type; }
        }

        public IJsSvgMatrix matrix
        {
            get {
                var wrappedValue = _baseObject.Matrix;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
            }
        }

        public float angle
        {
            get { return (float)_baseObject.Angle; }
        }
    }

    #endregion

    #region Implementation - IJsSvgTransformList

    /// <summary>
    /// Implementation wrapper for IJsSvgTransformList
    /// </summary>
    public sealed class JsSvgTransformList : JsObject<ISvgTransformList>, IJsSvgTransformList
    {
        public JsSvgTransformList(ISvgTransformList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void clear()
        {
            _baseObject.Clear();
        }

        public IJsSvgTransform initialize(IJsSvgTransform newItem)
        {
            var wrappedValue = _baseObject.Initialize(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform getItem(ulong index)
        {
            var wrappedValue = _baseObject.GetItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform insertItemBefore(IJsSvgTransform newItem, ulong index)
        {
            var wrappedValue = _baseObject.InsertItemBefore(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform replaceItem(IJsSvgTransform newItem, ulong index)
        {
            var wrappedValue = _baseObject.ReplaceItem(newItem.BaseObject, (uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform removeItem(ulong index)
        {
            var wrappedValue = _baseObject.RemoveItem((uint)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform appendItem(IJsSvgTransform newItem)
        {
            var wrappedValue = _baseObject.AppendItem(newItem.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform createSVGTransformFromMatrix(IJsSvgMatrix matrix)
        {
            var wrappedValue = _baseObject.CreateSvgTransformFromMatrix(matrix.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public IJsSvgTransform consolidate()
        {
            var wrappedValue = _baseObject.Consolidate();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgTransform>(wrappedValue, _engine) : null;
        }

        public ulong numberOfItems
        {
            get { return _baseObject.NumberOfItems; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedTransformList

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedTransformList
    /// </summary>
    public sealed class JsSvgAnimatedTransformList : JsObject<ISvgAnimatedTransformList>, IJsSvgAnimatedTransformList
    {
        public JsSvgAnimatedTransformList(ISvgAnimatedTransformList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgTransformList baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgTransformList animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgTransformList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgPreserveAspectRatio

    /// <summary>
    /// Implementation wrapper for IJsSvgPreserveAspectRatio
    /// </summary>
    public sealed class JsSvgPreserveAspectRatio : JsObject<ISvgPreserveAspectRatio>, IJsSvgPreserveAspectRatio
    {
        public JsSvgPreserveAspectRatio(ISvgPreserveAspectRatio baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public ushort align
        {
            get { return (ushort)_baseObject.Align; }
            set { _baseObject.Align = (SvgPreserveAspectRatioType)value; }
        }

        public ushort meetOrSlice
        {
            get { return (ushort)_baseObject.MeetOrSlice; }
            set { _baseObject.MeetOrSlice = (SvgMeetOrSlice)value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedPreserveAspectRatio

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedPreserveAspectRatio
    /// </summary>
    public sealed class JsSvgAnimatedPreserveAspectRatio : JsObject<ISvgAnimatedPreserveAspectRatio>, IJsSvgAnimatedPreserveAspectRatio
    {
        public JsSvgAnimatedPreserveAspectRatio(ISvgAnimatedPreserveAspectRatio baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgPreserveAspectRatio baseVal
        {
            get {
                var wrappedValue = _baseObject.BaseVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPreserveAspectRatio animVal
        {
            get {
                var wrappedValue = _baseObject.AnimVal;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPreserveAspectRatio>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedPathData

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedPathData
    /// </summary>
    public sealed class JsSvgAnimatedPathData : JsObject<ISvgAnimatedPathData>, IJsSvgAnimatedPathData
    {
        public JsSvgAnimatedPathData(ISvgAnimatedPathData baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgPathSegList pathSegList
        {
            get {
                var wrappedValue = _baseObject.PathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList normalizedPathSegList
        {
            get {
                var wrappedValue = _baseObject.NormalizedPathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList animatedPathSegList
        {
            get {
                var wrappedValue = _baseObject.AnimatedPathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList animatedNormalizedPathSegList
        {
            get {
                var wrappedValue = _baseObject.AnimatedNormalizedPathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion

    #region Implementation - IJsSvgAnimatedPoints

    /// <summary>
    /// Implementation wrapper for IJsSvgAnimatedPoints
    /// </summary>
    public sealed class JsSvgAnimatedPoints : JsObject<ISvgAnimatedPoints>, IJsSvgAnimatedPoints
    {
        public JsSvgAnimatedPoints(ISvgAnimatedPoints baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public IJsSvgPointList points
        {
            get {
                var wrappedValue = _baseObject.Points;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPointList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPointList animatedPoints
        {
            get {
                var wrappedValue = _baseObject.AnimatedPoints;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPointList>(wrappedValue, _engine) : null;
            }
        }
    }

    #endregion
}
