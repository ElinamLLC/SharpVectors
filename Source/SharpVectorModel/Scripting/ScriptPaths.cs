using System;

using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Events;

namespace SharpVectors.Scripting
{
    #region Implementation - IJsSvgPathSeg

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSeg
    /// </summary>
    public class JsSvgPathSeg : JsObject<ISvgPathSeg>, IJsSvgPathSeg
    {
        public JsSvgPathSeg(ISvgPathSeg baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public ushort pathSegType
        {
            get { return (ushort)_baseObject.PathSegType; }
        }

        public string pathSegTypeAsLetter
        {
            get { return _baseObject.PathSegTypeAsLetter; }
        }

        object IJsSvgPathSeg.BasePathSeg
        {
            get { return this.BaseObject; }
        }
    }

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSeg
    /// </summary>
    public abstract class JsSvgPathSeg<T> : JsObject<T>, IJsSvgPathSeg<T>
        where T : class
    {
        protected JsSvgPathSeg(T baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public ushort pathSegType
        {
            get { return (ushort)((ISvgPathSeg)_baseObject).PathSegType; }
        }

        public string pathSegTypeAsLetter
        {
            get { return ((ISvgPathSeg)_baseObject).PathSegTypeAsLetter; }
        }

        object IJsSvgPathSeg.BasePathSeg
        {
            get { return this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegClosePath

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegClosePath
    /// </summary>
    public sealed class JsSvgPathSegClosePath : JsSvgPathSeg<ISvgPathSegClosePath>, IJsSvgPathSegClosePath
    {
        public JsSvgPathSegClosePath(ISvgPathSegClosePath baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegMovetoAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegMovetoAbs
    /// </summary>
    public sealed class JsSvgPathSegMovetoAbs : JsSvgPathSeg<ISvgPathSegMovetoAbs>, IJsSvgPathSegMovetoAbs
    {
        public JsSvgPathSegMovetoAbs(ISvgPathSegMovetoAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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
    }

    #endregion

    #region Implementation - IJsSvgPathSegMovetoRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegMovetoRel
    /// </summary>
    public sealed class JsSvgPathSegMovetoRel : JsSvgPathSeg<ISvgPathSegMovetoRel>, IJsSvgPathSegMovetoRel
    {
        public JsSvgPathSegMovetoRel(ISvgPathSegMovetoRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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
    }

    #endregion

    #region Implementation - IJsSvgPathSegLinetoAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegLinetoAbs
    /// </summary>
    public sealed class JsSvgPathSegLinetoAbs : JsSvgPathSeg<ISvgPathSegLinetoAbs>, IJsSvgPathSegLinetoAbs
    {
        public JsSvgPathSegLinetoAbs(ISvgPathSegLinetoAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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
    }

    #endregion

    #region Implementation - IJsSvgPathSegLinetoRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegLinetoRel
    /// </summary>
    public sealed class JsSvgPathSegLinetoRel : JsSvgPathSeg<ISvgPathSegLinetoRel>, IJsSvgPathSegLinetoRel
    {
        public JsSvgPathSegLinetoRel(ISvgPathSegLinetoRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoCubicAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoCubicAbs
    /// </summary>
    public sealed class JsSvgPathSegCurvetoCubicAbs : JsSvgPathSeg<ISvgPathSegCurvetoCubicAbs>, IJsSvgPathSegCurvetoCubicAbs
    {
        public JsSvgPathSegCurvetoCubicAbs(ISvgPathSegCurvetoCubicAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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

        public float x1
        {
            get { return (float)_baseObject.X1; }
            set { _baseObject.X1 = value; }
        }

        public float y1
        {
            get { return (float)_baseObject.Y1; }
            set { _baseObject.Y1 = value; }
        }

        public float x2
        {
            get { return (float)_baseObject.X2; }
            set { _baseObject.X2 = value; }
        }

        public float y2
        {
            get { return (float)_baseObject.Y2; }
            set { _baseObject.Y2 = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoCubicRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoCubicRel
    /// </summary>
    public sealed class JsSvgPathSegCurvetoCubicRel : JsSvgPathSeg<ISvgPathSegCurvetoCubicRel>, IJsSvgPathSegCurvetoCubicRel
    {
        public JsSvgPathSegCurvetoCubicRel(ISvgPathSegCurvetoCubicRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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

        public float x1
        {
            get { return (float)_baseObject.X1; }
            set { _baseObject.X1 = value; }
        }

        public float y1
        {
            get { return (float)_baseObject.Y1; }
            set { _baseObject.Y1 = value; }
        }

        public float x2
        {
            get { return (float)_baseObject.X2; }
            set { _baseObject.X2 = value; }
        }

        public float y2
        {
            get { return (float)_baseObject.Y2; }
            set { _baseObject.Y2 = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoQuadraticAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoQuadraticAbs
    /// </summary>
    public sealed class JsSvgPathSegCurvetoQuadraticAbs : JsSvgPathSeg<ISvgPathSegCurvetoQuadraticAbs>, IJsSvgPathSegCurvetoQuadraticAbs
    {
        public JsSvgPathSegCurvetoQuadraticAbs(ISvgPathSegCurvetoQuadraticAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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

        public float x1
        {
            get { return (float)_baseObject.X1; }
            set { _baseObject.X1 = value; }
        }

        public float y1
        {
            get { return (float)_baseObject.Y1; }
            set { _baseObject.Y1 = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoQuadraticRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoQuadraticRel
    /// </summary>
    public sealed class JsSvgPathSegCurvetoQuadraticRel : JsSvgPathSeg<ISvgPathSegCurvetoQuadraticRel>, IJsSvgPathSegCurvetoQuadraticRel
    {
        public JsSvgPathSegCurvetoQuadraticRel(ISvgPathSegCurvetoQuadraticRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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

        public float x1
        {
            get { return (float)_baseObject.X1; }
            set { _baseObject.X1 = value; }
        }

        public float y1
        {
            get { return (float)_baseObject.Y1; }
            set { _baseObject.Y1 = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegArcAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegArcAbs
    /// </summary>
    public sealed class JsSvgPathSegArcAbs : JsSvgPathSeg<ISvgPathSegArcAbs>, IJsSvgPathSegArcAbs
    {
        public JsSvgPathSegArcAbs(ISvgPathSegArcAbs baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

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

        public float r1
        {
            get { return (float)_baseObject.R1; }
            set { _baseObject.R1 = value; }
        }

        public float r2
        {
            get { return (float)_baseObject.R2; }
            set { _baseObject.R2 = value; }
        }

        public float angle
        {
            get { return (float)_baseObject.Angle; }
            set { _baseObject.Angle = value; }
        }

        public bool largeArcFlag
        {
            get { return _baseObject.LargeArcFlag; }
            set { _baseObject.LargeArcFlag = value; }
        }

        public bool sweepFlag
        {
            get { return _baseObject.SweepFlag; }
            set { _baseObject.SweepFlag = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegArcRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegArcRel
    /// </summary>
    public sealed class JsSvgPathSegArcRel : JsSvgPathSeg<ISvgPathSegArcRel>, IJsSvgPathSegArcRel
    {
        public JsSvgPathSegArcRel(ISvgPathSegArcRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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

        public float r1
        {
            get { return (float)_baseObject.R1; }
            set { _baseObject.R1 = value; }
        }

        public float r2
        {
            get { return (float)_baseObject.R2; }
            set { _baseObject.R2 = value; }
        }

        public float angle
        {
            get { return (float)_baseObject.Angle; }
            set { _baseObject.Angle = value; }
        }

        public bool largeArcFlag
        {
            get { return _baseObject.LargeArcFlag; }
            set { _baseObject.LargeArcFlag = value; }
        }

        public bool sweepFlag
        {
            get { return _baseObject.SweepFlag; }
            set { _baseObject.SweepFlag = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegLinetoHorizontalAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegLinetoHorizontalAbs
    /// </summary>
    public sealed class JsSvgPathSegLinetoHorizontalAbs : JsSvgPathSeg<ISvgPathSegLinetoHorizontalAbs>, IJsSvgPathSegLinetoHorizontalAbs
    {
        public JsSvgPathSegLinetoHorizontalAbs(ISvgPathSegLinetoHorizontalAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public float x
        {
            get { return (float)_baseObject.X; }
            set { _baseObject.X = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegLinetoHorizontalRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegLinetoHorizontalRel
    /// </summary>
    public sealed class JsSvgPathSegLinetoHorizontalRel : JsSvgPathSeg<ISvgPathSegLinetoHorizontalRel>, IJsSvgPathSegLinetoHorizontalRel
    {
        public JsSvgPathSegLinetoHorizontalRel(ISvgPathSegLinetoHorizontalRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public float x
        {
            get { return (float)_baseObject.X; }
            set { _baseObject.X = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegLinetoVerticalAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegLinetoVerticalAbs
    /// </summary>
    public sealed class JsSvgPathSegLinetoVerticalAbs : JsSvgPathSeg<ISvgPathSegLinetoVerticalAbs>, IJsSvgPathSegLinetoVerticalAbs
    {
        public JsSvgPathSegLinetoVerticalAbs(ISvgPathSegLinetoVerticalAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public float y
        {
            get { return (float)_baseObject.Y; }
            set { _baseObject.Y = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegLinetoVerticalRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegLinetoVerticalRel
    /// </summary>
    public sealed class JsSvgPathSegLinetoVerticalRel : JsSvgPathSeg<ISvgPathSegLinetoVerticalRel>, IJsSvgPathSegLinetoVerticalRel
    {
        public JsSvgPathSegLinetoVerticalRel(ISvgPathSegLinetoVerticalRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public float y
        {
            get { return (float)_baseObject.Y; }
            set { _baseObject.Y = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoCubicSmoothAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoCubicSmoothAbs
    /// </summary>
    public sealed class JsSvgPathSegCurvetoCubicSmoothAbs : JsSvgPathSeg<ISvgPathSegCurvetoCubicSmoothAbs>, IJsSvgPathSegCurvetoCubicSmoothAbs
    {
        public JsSvgPathSegCurvetoCubicSmoothAbs(ISvgPathSegCurvetoCubicSmoothAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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

        public float x2
        {
            get { return (float)_baseObject.X2; }
            set { _baseObject.X2 = value; }
        }

        public float y2
        {
            get { return (float)_baseObject.Y2; }
            set { _baseObject.Y2 = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoCubicSmoothRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoCubicSmoothRel
    /// </summary>
    public sealed class JsSvgPathSegCurvetoCubicSmoothRel : JsSvgPathSeg<ISvgPathSegCurvetoCubicSmoothRel>, IJsSvgPathSegCurvetoCubicSmoothRel
    {
        public JsSvgPathSegCurvetoCubicSmoothRel(ISvgPathSegCurvetoCubicSmoothRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine) { }

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

        public float x2
        {
            get { return (float)_baseObject.X2; }
            set { _baseObject.X2 = value; }
        }

        public float y2
        {
            get { return (float)_baseObject.Y2; }
            set { _baseObject.Y2 = value; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoQuadraticSmoothAbs

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoQuadraticSmoothAbs
    /// </summary>
    public sealed class JsSvgPathSegCurvetoQuadraticSmoothAbs : JsSvgPathSeg<ISvgPathSegCurvetoQuadraticSmoothAbs>, IJsSvgPathSegCurvetoQuadraticSmoothAbs
    {
        public JsSvgPathSegCurvetoQuadraticSmoothAbs(ISvgPathSegCurvetoQuadraticSmoothAbs baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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
    }

    #endregion

    #region Implementation - IJsSvgPathSegCurvetoQuadraticSmoothRel

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegCurvetoQuadraticSmoothRel
    /// </summary>
    public sealed class JsSvgPathSegCurvetoQuadraticSmoothRel : JsSvgPathSeg<ISvgPathSegCurvetoQuadraticSmoothRel>, IJsSvgPathSegCurvetoQuadraticSmoothRel
    {
        public JsSvgPathSegCurvetoQuadraticSmoothRel(ISvgPathSegCurvetoQuadraticSmoothRel baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

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
    }

    #endregion

    #region Implementation - IJsSvgPathSegList

    /// <summary>
    /// Implementation wrapper for IJsSvgPathSegList
    /// </summary>
    public sealed class JsSvgPathSegList : JsObject<ISvgPathSegList>, IJsSvgPathSegList
    {
        public JsSvgPathSegList(ISvgPathSegList baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public void clear()
        {
            _baseObject.Clear();
        }

        public IJsSvgPathSeg initialize(IJsSvgPathSeg newItem)
        {
            var wrappedValue = _baseObject.Initialize((ISvgPathSeg)newItem.BasePathSeg);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSeg>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSeg getItem(ulong index)
        {
            var wrappedValue = _baseObject.GetItem((int)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSeg>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSeg insertItemBefore(IJsSvgPathSeg newItem, ulong index)
        {
            var wrappedValue = _baseObject.InsertItemBefore((ISvgPathSeg)newItem.BasePathSeg, (int)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSeg>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSeg replaceItem(IJsSvgPathSeg newItem, ulong index)
        {
            var wrappedValue = _baseObject.ReplaceItem((ISvgPathSeg)newItem.BasePathSeg, (int)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSeg>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSeg removeItem(ulong index)
        {
            var wrappedValue = _baseObject.RemoveItem((int)index);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSeg>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSeg appendItem(IJsSvgPathSeg newItem)
        {
            var wrappedValue = _baseObject.AppendItem((ISvgPathSeg)newItem.BasePathSeg);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSeg>(wrappedValue, _engine) : null;
        }

        public ulong numberOfItems
        {
            get { return (ulong)_baseObject.NumberOfItems; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPathElement

    /// <summary>
    /// Implementation wrapper for IJsSvgPathElement
    /// </summary>
    public sealed class JsSvgPathElement : JsSvgElement, IJsSvgPathElement
    {
        public JsSvgPathElement(ISvgPathElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine) { }

        public float getTotalLength()
        {
            return (float)((ISvgPathElement)_baseObject).GetTotalLength();
        }

        public IJsSvgPoint getPointAtLength(float distance)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).GetPointAtLength(distance);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPoint>(wrappedValue, _engine) : null;
        }

        public ulong getPathSegAtLength(float distance)
        {
            return (ulong)((ISvgPathElement)_baseObject).GetPathSegAtLength(distance);
        }

        public IJsSvgPathSegClosePath createSVGPathSegClosePath()
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegClosePath();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegClosePath>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegMovetoAbs createSVGPathSegMovetoAbs(float x, float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegMovetoAbs(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegMovetoAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegMovetoRel createSVGPathSegMovetoRel(float x, float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegMovetoRel(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegMovetoRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegLinetoAbs createSVGPathSegLinetoAbs(float x, float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegLinetoAbs(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegLinetoAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegLinetoRel createSVGPathSegLinetoRel(float x, float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegLinetoRel(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegLinetoRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoCubicAbs createSVGPathSegCurvetoCubicAbs(float x, float y, float x1, float y1, float x2, float y2)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoCubicAbs(x, y, x1, y1, x2, y2);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoCubicAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoCubicRel createSVGPathSegCurvetoCubicRel(float x, float y, float x1, float y1, float x2, float y2)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoCubicRel(x, y, x1, y1, x2, y2);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoCubicRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoQuadraticAbs createSVGPathSegCurvetoQuadraticAbs(float x, float y, float x1, float y1)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoQuadraticAbs(x, y, x1, y1);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoQuadraticAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoQuadraticRel createSVGPathSegCurvetoQuadraticRel(float x, float y, float x1, float y1)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoQuadraticRel(x, y, x1, y1);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoQuadraticRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegArcAbs createSVGPathSegArcAbs(float x, float y, float r1, float r2, 
            float angle, bool largeArcFlag, bool sweepFlag)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegArcAbs(x, y, r1, r2, angle, largeArcFlag, sweepFlag);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegArcAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegArcRel createSVGPathSegArcRel(float x, float y, float r1, float r2, 
            float angle, bool largeArcFlag, bool sweepFlag)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegArcRel(x, y, r1, r2, angle, largeArcFlag, sweepFlag);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegArcRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegLinetoHorizontalAbs createSVGPathSegLinetoHorizontalAbs(float x)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegLinetoHorizontalAbs(x);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegLinetoHorizontalAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegLinetoHorizontalRel createSVGPathSegLinetoHorizontalRel(float x)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegLinetoHorizontalRel(x);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegLinetoHorizontalRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegLinetoVerticalAbs createSVGPathSegLinetoVerticalAbs(float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegLinetoVerticalAbs(y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegLinetoVerticalAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegLinetoVerticalRel createSVGPathSegLinetoVerticalRel(float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegLinetoVerticalRel(y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegLinetoVerticalRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoCubicSmoothAbs createSVGPathSegCurvetoCubicSmoothAbs(float x, float y, float x2, float y2)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoCubicSmoothAbs(x, y, x2, y2);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoCubicSmoothAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoCubicSmoothRel createSVGPathSegCurvetoCubicSmoothRel(float x, float y, float x2, float y2)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoCubicSmoothRel(x, y, x2, y2);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoCubicSmoothRel>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoQuadraticSmoothAbs createSVGPathSegCurvetoQuadraticSmoothAbs(float x, float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoQuadraticSmoothAbs(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoQuadraticSmoothAbs>(wrappedValue, _engine) : null;
        }

        public IJsSvgPathSegCurvetoQuadraticSmoothRel createSVGPathSegCurvetoQuadraticSmoothRel(float x, float y)
        {
            var wrappedValue = ((ISvgPathElement)_baseObject).CreateSvgPathSegCurvetoQuadraticSmoothRel(x, y);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegCurvetoQuadraticSmoothRel>(wrappedValue, _engine) : null;
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedNumber pathLength
        {
            get {
                var wrappedValue = ((ISvgPathElement)_baseObject).PathLength;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedNumber>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList pathSegList
        {
            get {
                var wrappedValue = ((ISvgAnimatedPathData)_baseObject).PathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList normalizedPathSegList
        {
            get {
                var wrappedValue = ((ISvgAnimatedPathData)_baseObject).NormalizedPathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList animatedPathSegList
        {
            get {
                var wrappedValue = ((ISvgAnimatedPathData)_baseObject).AnimatedPathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPathSegList animatedNormalizedPathSegList
        {
            get {
                var wrappedValue = ((ISvgAnimatedPathData)_baseObject).AnimatedNormalizedPathSegList;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPathSegList>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
        ISvgAnimatedPathData IScriptableObject<ISvgAnimatedPathData>.BaseObject
        {
            get { return (ISvgAnimatedPathData)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgRectElement

    /// <summary>
    /// Implementation wrapper for IJsSvgRectElement
    /// </summary>
    public sealed class JsSvgRectElement : JsSvgElement, IJsSvgRectElement
    {
        public JsSvgRectElement(ISvgRectElement baseObject, ISvgScriptEngine engine) : base(baseObject, engine) { }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedLength x
        {
            get {
                var wrappedValue = ((ISvgRectElement)_baseObject).X;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y
        {
            get {
                var wrappedValue = ((ISvgRectElement)_baseObject).Y;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength width
        {
            get {
                var wrappedValue = ((ISvgRectElement)_baseObject).Width;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength height
        {
            get {
                var wrappedValue = ((ISvgRectElement)_baseObject).Height;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength rx
        {
            get {
                var wrappedValue = ((ISvgRectElement)_baseObject).Rx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength ry
        {
            get {
                var wrappedValue = ((ISvgRectElement)_baseObject).Ry;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgCircleElement

    /// <summary>
    /// Implementation wrapper for IJsSvgCircleElement
    /// </summary>
    public sealed class JsSvgCircleElement : JsSvgElement, IJsSvgCircleElement
    {
        public JsSvgCircleElement(ISvgCircleElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedLength cx
        {
            get {
                var wrappedValue = ((ISvgCircleElement)_baseObject).Cx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength cy
        {
            get {
                var wrappedValue = ((ISvgCircleElement)_baseObject).Cy;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength r
        {
            get {
                var wrappedValue = ((ISvgCircleElement)_baseObject).R;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgEllipseElement

    /// <summary>
    /// Implementation wrapper for IJsSvgEllipseElement
    /// </summary>
    public sealed class JsSvgEllipseElement : JsSvgElement, IJsSvgEllipseElement
    {
        public JsSvgEllipseElement(ISvgEllipseElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedLength cx
        {
            get {
                var wrappedValue = ((ISvgEllipseElement)_baseObject).Cx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength cy
        {
            get {
                var wrappedValue = ((ISvgEllipseElement)_baseObject).Cy;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength rx
        {
            get {
                var wrappedValue = ((ISvgEllipseElement)_baseObject).Rx;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength ry
        {
            get {
                var wrappedValue = ((ISvgEllipseElement)_baseObject).Ry;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgLineElement

    /// <summary>
    /// Implementation wrapper for IJsSvgLineElement
    /// </summary>
    public sealed class JsSvgLineElement : JsSvgElement, IJsSvgLineElement
    {
        public JsSvgLineElement(ISvgLineElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgAnimatedLength x1
        {
            get {
                var wrappedValue = ((ISvgLineElement)_baseObject).X1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y1
        {
            get {
                var wrappedValue = ((ISvgLineElement)_baseObject).Y1;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength x2
        {
            get {
                var wrappedValue = ((ISvgLineElement)_baseObject).X2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedLength y2
        {
            get {
                var wrappedValue = ((ISvgLineElement)_baseObject).Y2;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedLength>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPolylineElement

    /// <summary>
    /// Implementation wrapper for IJsSvgPolylineElement
    /// </summary>
    public sealed class JsSvgPolylineElement : JsSvgElement, IJsSvgPolylineElement
    {
        public JsSvgPolylineElement(ISvgPolylineElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine)
        {
        }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPointList points
        {
            get {
                var wrappedValue = ((ISvgAnimatedPoints)_baseObject).Points;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPointList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPointList animatedPoints
        {
            get {
                var wrappedValue = ((ISvgAnimatedPoints)_baseObject).AnimatedPoints;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPointList>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
        ISvgAnimatedPoints IScriptableObject<ISvgAnimatedPoints>.BaseObject
        {
            get { return (ISvgAnimatedPoints)this.BaseObject; }
        }
    }

    #endregion

    #region Implementation - IJsSvgPolygonElement

    /// <summary>
    /// Implementation wrapper for IJsSvgPolygonElement
    /// </summary>
    public sealed class JsSvgPolygonElement : JsSvgElement, IJsSvgPolygonElement
    {
        public JsSvgPolygonElement(ISvgPolygonElement baseObject, ISvgScriptEngine engine) 
            : base(baseObject, engine) { }

        public bool hasExtension(string extension)
        {
            return ((ISvgTests)_baseObject).HasExtension(extension);
        }

        public IJsCssValue getPresentationAttribute(string name)
        {
            var wrappedValue = ((ISvgStylable)_baseObject).GetPresentationAttribute(name);
            return (wrappedValue != null) ? CreateWrapper<IJsCssValue>(wrappedValue, _engine) : null;
        }

        public IJsSvgRect getBBox()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetBBox();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgRect>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getScreenCTM()
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetScreenCTM();
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public IJsSvgMatrix getTransformToElement(IJsSvgElement element)
        {
            var wrappedValue = ((ISvgLocatable)_baseObject).GetTransformToElement((ISvgElement)element.BaseObject);
            return (wrappedValue != null) ? CreateWrapper<IJsSvgMatrix>(wrappedValue, _engine) : null;
        }

        public void addEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.AddListener(type, listener, useCapture);
            }
        }

        public void removeEventListener(string type, object listener, bool useCapture)
        {
            if (_engine != null && _engine.ScriptClosures != null)
            {
                var scriptClosures = _engine.ScriptClosures;
                scriptClosures.RemoveListener(type, listener, useCapture);
            }
        }

        public bool dispatchEvent(IJsEvent evt)
        {
            return ((IEventTarget)_baseObject).DispatchEvent(evt.BaseObject);
        }

        public IJsSvgStringList requiredFeatures
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredFeatures;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList requiredExtensions
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).RequiredExtensions;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgStringList systemLanguage
        {
            get {
                var wrappedValue = ((ISvgTests)_baseObject).SystemLanguage;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgStringList>(wrappedValue, _engine) : null;
            }
        }

        public string xmllang
        {
            get { return ((ISvgLangSpace)_baseObject).XmlLang; }
            set { ((ISvgLangSpace)_baseObject).XmlLang = value; }
        }

        public string xmlspace
        {
            get { return ((ISvgLangSpace)_baseObject).XmlSpace; }
            set { ((ISvgLangSpace)_baseObject).XmlSpace = value; }
        }

        public bool externalResourcesRequired
        {
            get { return ((ISvgExternalResourcesRequired)_baseObject).ExternalResourcesRequired.BaseVal; }
        }

        public IJsSvgAnimatedString className
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).ClassName;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedString>(wrappedValue, _engine) : null;
            }
        }

        public IJsCssStyleDeclaration style
        {
            get {
                var wrappedValue = ((ISvgStylable)_baseObject).Style;
                return (wrappedValue != null) ? CreateWrapper<IJsCssStyleDeclaration>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgAnimatedTransformList transform
        {
            get {
                var wrappedValue = ((ISvgTransformable)_baseObject).Transform;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgAnimatedTransformList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement nearestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).NearestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgElement farthestViewportElement
        {
            get {
                var wrappedValue = ((ISvgLocatable)_baseObject).FarthestViewportElement;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgElement>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPointList points
        {
            get {
                var wrappedValue = ((ISvgAnimatedPoints)_baseObject).Points;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPointList>(wrappedValue, _engine) : null;
            }
        }

        public IJsSvgPointList animatedPoints
        {
            get {
                var wrappedValue = ((ISvgAnimatedPoints)_baseObject).AnimatedPoints;
                return (wrappedValue != null) ? CreateWrapper<IJsSvgPointList>(wrappedValue, _engine) : null;
            }
        }

        ISvgTests IScriptableObject<ISvgTests>.BaseObject
        {
            get { return (ISvgTests)this.BaseObject; }
        }
        ISvgLangSpace IScriptableObject<ISvgLangSpace>.BaseObject
        {
            get { return (ISvgLangSpace)this.BaseObject; }
        }
        ISvgExternalResourcesRequired IScriptableObject<ISvgExternalResourcesRequired>.BaseObject
        {
            get { return (ISvgExternalResourcesRequired)this.BaseObject; }
        }
        ISvgStylable IScriptableObject<ISvgStylable>.BaseObject
        {
            get { return (ISvgStylable)this.BaseObject; }
        }
        ISvgLocatable IScriptableObject<ISvgLocatable>.BaseObject
        {
            get { return (ISvgLocatable)this.BaseObject; }
        }
        IEventTarget IScriptableObject<IEventTarget>.BaseObject
        {
            get { return (IEventTarget)this.BaseObject; }
        }
        ISvgAnimatedPoints IScriptableObject<ISvgAnimatedPoints>.BaseObject
        {
            get { return (ISvgAnimatedPoints)this.BaseObject; }
        }
    }

    #endregion
}
