using System;
using System.Linq;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;

using SharpVectors.Dom.Svg;
using SharpVectors.Runtime;

namespace SharpVectors.Renderers.Wpf
{
    public sealed class WpfDrawingDocument : DependencyObject
    {
        #region Private Fields

        private bool _isEnumerated;
        private SvgDocument _svgDocument;
        private DrawingGroup _svgDrawing;
        private IDictionary<string, Drawing> _idMap;
        private IDictionary<string, Drawing> _guidMap;

        private WpfHitTextLevel _textLevel;

        private WpfHitPath _hitPath;
        private DrawingGroup _hitGroup;
        private SortedList<int, Drawing> _hitList;
        private SortedList<int, WpfHitPath> _hitPaths;

        private GeneralTransform _displayTransform;

        private DrawingGroup _drawingLayer;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingDocument()
        {
            _textLevel        = WpfHitTextLevel.Glyphs;
            _idMap            = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            _guidMap          = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            _displayTransform = Transform.Identity;
            _hitList          = new SortedList<int, Drawing>();
            _hitPaths         = new SortedList<int, WpfHitPath>();
        }

        #endregion

        #region Public Properties

        public WpfHitTextLevel TextLevel
        {
            get {
                return _textLevel;
            }
            set {
                _textLevel = value;
            }
        }

        public GeneralTransform DisplayTransform
        {
            get {
                return _displayTransform;
            }
            set {
                if (value == null)
                {
                    _displayTransform = Transform.Identity;
                }
                else
                {
                    _displayTransform = value;
                }
            }
        }

        public SvgDocument Document
        {
            get {
                return _svgDocument;
            }
        }

        public DrawingGroup Drawing
        {
            get {
                return _svgDrawing;
            }
        }

        public ICollection<string> DrawingNames
        {
            get {
                if (_idMap != null)
                {
                    return _idMap.Keys;
                }
                return null;
            }
        }

        public ICollection<string> DrawingUniqueNames
        {
            get {
                if (_guidMap != null)
                {
                    return _guidMap.Keys;
                }
                return null;
            }
        }

        public ICollection<string> ElementNames
        {
            get {
                if (_svgDocument != null)
                {
                    var elementMap = _svgDocument.ElementMap;
                    if (elementMap != null)
                    {
                        return elementMap.Keys;
                    }
                }
                return null;
            }
        }

        public ICollection<string> ElementUniqueNames
        {
            get {
                if (_svgDocument != null)
                {
                    var elementUniqueMap = _svgDocument.ElementUniqueMap;
                    if (elementUniqueMap != null)
                    {
                        return elementUniqueMap.Keys;
                    }
                }
                return null;
            }
        }

        public IList<Drawing> HitList
        {
            get {
                if (_hitList != null && _hitList.Count != 0)
                {
                    return _hitList.Values;
                }
                return null;
            }
        }

        public IList<WpfHitPath> HitPaths
        {
            get {
                if (_hitPaths != null && _hitPaths.Count != 0)
                {
                    return _hitPaths.Values;
                }
                return null;
            }
        }

        #endregion

        #region Public Methods

        public void Initialize(SvgDocument svgDocument, DrawingGroup svgDrawing)
        {
            _svgDocument = svgDocument;
            _svgDrawing  = svgDrawing;
        }

        public void Add(string elementId, string uniqueId, Drawing drawing)
        {
            if (drawing == null)
            {
                return;
            }
            this.AddById(elementId, drawing);
            this.AddByUniqueId(uniqueId, drawing);
        }

        public void AddById(string elementId, Drawing drawing)
        {
            if (string.IsNullOrWhiteSpace(elementId) || drawing == null)
            {
                return;
            }
            if (_idMap == null)
            {
                _idMap = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            }
            if (_idMap.ContainsKey(elementId))
            {
                _idMap[elementId] = drawing;
            }
            else
            {
                _idMap.Add(elementId, drawing);
            }
        }

        public void AddByUniqueId(Guid uniqueId, Drawing drawing)
        {
            if (uniqueId == Guid.Empty || drawing == null)
            {
                return;
            }
            this.AddByUniqueId(uniqueId.ToString(), drawing);
        }

        public void AddByUniqueId(string uniqueId, Drawing drawing)
        {
            if (string.IsNullOrWhiteSpace(uniqueId) || drawing == null)
            {
                return;
            }
            if (_guidMap == null)
            {
                _guidMap = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            }
            if (_guidMap.ContainsKey(uniqueId))
            {
                _guidMap[uniqueId] = drawing;
            }
            else
            {
                _guidMap.Add(uniqueId, drawing);
            }
        }

        public Tuple<SvgElement, Drawing> Get(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId))
            {
                return null;
            }
            return new Tuple<SvgElement, Drawing>(this.GetSvgById(elementId), this.GetById(elementId));
        }

        public Drawing GetById(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId) || _svgDocument == null || _idMap == null)
            {
                return null;
            }
            if (_idMap.Count != 0 && _idMap.ContainsKey(elementId))
            {
                return _idMap[elementId];
            }
            if (!_isEnumerated) // Enumeration is expensive for large objects, we do it once
            {
                var svgElement = _svgDocument.GetSvgById(elementId);
                if (svgElement != null)
                {
                    // this.EnumerateDrawing();
                }
                if (_idMap.Count != 0 && _idMap.ContainsKey(elementId))
                {
                    return _idMap[elementId];
                }
            }

            return null;
        }

        public Drawing GetByUniqueId(Guid uniqueId)
        {
            if (uniqueId == Guid.Empty || _guidMap == null)
            {
                return null;
            }
            return this.GetByUniqueId(uniqueId.ToString());
        }

        public Drawing GetByUniqueId(string uniqueId)
        {
            if (string.IsNullOrWhiteSpace(uniqueId) || _svgDocument == null || _guidMap == null)
            {
                return null;
            }
            if (_guidMap.Count != 0 && _guidMap.ContainsKey(uniqueId))
            {
                return _guidMap[uniqueId];
            }

            var svgElement = _svgDocument.GetSvgByUniqueId(uniqueId);

            return null;
        }

        public SvgElement GetSvgById(string elementId)
        {
            if (string.IsNullOrWhiteSpace(elementId) || _svgDocument == null)
            {
                return null;
            }
            return _svgDocument.GetSvgById(elementId);
        }

        public SvgElement GetSvgByUniqueId(Guid uniqueId)
        {
            if (uniqueId == Guid.Empty)
            {
                return null;
            }
            return this.GetSvgByUniqueId(uniqueId.ToString());
        }

        public SvgElement GetSvgByUniqueId(string uniqueId)
        {
            if (string.IsNullOrWhiteSpace(uniqueId) || _svgDocument == null)
            {
                return null;
            }
            return _svgDocument.GetSvgByUniqueId(uniqueId);
        }

        public void EnumerateDrawing(DrawingGroup drawing)
        {
            if (drawing == null)
            {
                return;
            }

            _svgDrawing = drawing;

            //TODO: Trying a background run...
            Task.Factory.StartNew(() =>
            {
                this.EnumerateDrawing();
            });
        }

        public WpfHitTestResult HitTest(Point point)
        {
            var svgDrawing = this.PerformHitTest(point);
            if (svgDrawing == null)
            {
                return WpfHitTestResult.Empty;
            }
            string uniqueId = SvgObject.GetUniqueId(svgDrawing);
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                if (_hitGroup != null && _hitGroup.Children.Contains(svgDrawing))
                {
                    var groupElement = this.GetSvgByUniqueId(uniqueId);
                    if (groupElement != null)
                    {
                        return new WpfHitTestResult(point, groupElement, svgDrawing);
                    }
                }

                return new WpfHitTestResult(point, null, svgDrawing);
//                return WpfHitTestResult.Empty;
            }
            var svgElement = this.GetSvgByUniqueId(uniqueId);
            if (svgElement == null)
            {
                return new WpfHitTestResult(point, null, svgDrawing);
//                return WpfHitTestResult.Empty;
            }

            return new WpfHitTestResult(point, svgElement, svgDrawing);
        }

        public WpfHitTestResult HitTest(Rect rect, IntersectionDetail detail)
        {
            var svgDrawing = this.PerformHitTest(rect, detail);
            if (svgDrawing == null)
            {
                return WpfHitTestResult.Empty;
            }
            string uniqueId = SvgObject.GetUniqueId(svgDrawing);
            if (string.IsNullOrWhiteSpace(uniqueId))
            {
                return new WpfHitTestResult(rect, null, svgDrawing);
//                return WpfHitTestResult.Empty;
            }
            var svgElement = this.GetSvgByUniqueId(uniqueId);
            if (svgElement == null)
            {
                return new WpfHitTestResult(rect, null, svgDrawing);
//                return WpfHitTestResult.Empty;
            }

            return new WpfHitTestResult(rect, svgElement, svgDrawing);
        }

        public DrawingGroup GetDrawingLayer()
        {
            if (_drawingLayer != null)
            {
                return _drawingLayer;
            }

            var isFound = false;

            var drawingLayer = this.GetDrawingLayer(_svgDrawing, ref isFound);
            if (isFound)
            {
                _drawingLayer = drawingLayer;
            }
            return drawingLayer;
        }

        #endregion

        #region Private Methods

        #region HitTest Point

        private Drawing PerformHitTest(Point pt)
        {
            if (_svgDrawing == null)
            {
                return null;
            }

            _hitGroup = null;
            if (_hitList == null)
            {
                _hitList  = new SortedList<int, Drawing>();
                _hitPaths = new SortedList<int, WpfHitPath>();
            }
            else if (_hitList.Count != 0)
            {
                _hitList.Clear();
                _hitPaths.Clear();
            }

            var hitRootPath = new WpfHitPath();
            _hitPath = hitRootPath;

            Point ptDisplay = _displayTransform.Transform(pt);

            DrawingGroup groupDrawing       = null;
            GlyphRunDrawing glyRunDrawing   = null;
            GeometryDrawing geometryDrawing = null;

            Drawing foundDrawing = null;

            //var isFound = false;
            //var drawingLayer = this.GetDrawingLayer(_svgDrawing, ref isFound);
            //System.Diagnostics.Trace.WriteLine("GetId: " + SvgObject.GetId(drawingLayer));
            //System.Diagnostics.Trace.WriteLine("GetName: " + SvgObject.GetName(drawingLayer));
            //System.Diagnostics.Trace.WriteLine("GetUniqueId: " + SvgObject.GetUniqueId(drawingLayer));

            //DrawingCollection drawings = drawingLayer.Children;
            DrawingCollection drawings = _svgDrawing.Children;
            for (int i = drawings.Count - 1; i >= 0; i--)
            {
                Drawing drawing = drawings[i];
                System.Diagnostics.Trace.WriteLine("GetId: " + SvgObject.GetId(drawing));
                System.Diagnostics.Trace.WriteLine("GetName: " + SvgObject.GetName(drawing));
                System.Diagnostics.Trace.WriteLine("GetUniqueId: " + SvgObject.GetUniqueId(drawing));

                if (TryCast.Cast(drawing, out geometryDrawing))
                {
                    if (HitTestDrawing(geometryDrawing, ptDisplay))
                    {
//                        _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(geometryDrawing));

                        int orderNumber = SvgObject.GetOrder(drawing);
                        if (orderNumber >= 0)
                        {
                            _hitList[orderNumber]  = drawing;
                            _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(geometryDrawing));
                        }
                        else
                        {
                            string uniqueId = SvgObject.GetUniqueId(geometryDrawing);
                            if (!string.IsNullOrWhiteSpace(uniqueId))
                            {
                                foundDrawing = drawing;
                                break;
                            }
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out groupDrawing))
                {
                    //var ptSaved = ptDisplay;
                    //var transform = groupDrawing.Transform;
                    //if (transform != null && !transform.Value.IsIdentity)
                    //{
                    //    ptDisplay = transform.Inverse.Transform(ptDisplay);
                    //}

                    _hitPath = hitRootPath.AddChild(SvgObject.GetUniqueId(groupDrawing));

                    int orderNumber = SvgObject.GetOrder(groupDrawing);
                    //if (SvgObject.GetType(groupDrawing) == SvgObjectType.Text &&
                    //    groupDrawing.Bounds.Contains(ptDisplay))
                    if (SvgObject.GetType(groupDrawing) == SvgObjectType.Text &&
                        this.HitTestText(groupDrawing, ptDisplay, out foundDrawing))
                    {
                        if (orderNumber >= 0)
                        {
                            _hitList[orderNumber]  = drawing;
                            _hitPaths[orderNumber] = _hitPath;
                        }
                        else
                        {
                            foundDrawing = drawing;
                            break;
                        }
                    }
                    else 
                    {
//                        _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(groupDrawing));
                        
                        var currentPath = _hitPath;
                        try
                        {
                            if (HitTestDrawing(groupDrawing, ptDisplay, out foundDrawing))
                            {
                                if (orderNumber >= 0)
                                {
                                    _hitList[orderNumber]  = drawing;
                                    _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(foundDrawing));
                                }
                                else
                                {
                                    string uniqueId = SvgObject.GetUniqueId(foundDrawing);
                                    if (!string.IsNullOrWhiteSpace(uniqueId))
                                    {
                                        //foundDrawing = drawing;
                                        break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            _hitPath = currentPath;
                        }

                    }
                }
                else if (TryCast.Cast(drawing, out glyRunDrawing))
                {
                    if (HitTestDrawing(glyRunDrawing, ptDisplay))
                    {
//                        _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(glyRunDrawing));

                        int orderNumber = SvgObject.GetOrder(drawing);
                        if (orderNumber >= 0)
                        {
                            _hitList[orderNumber]  = drawing;
                            _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(glyRunDrawing));
                        }
                        else
                        {
                            foundDrawing = drawing;
                            break;
                        }
                    }
                }
            }

            if (_hitList.Count != 0)
            {
                var key = _hitList.LastOrDefault().Key;
                if (_hitPaths.ContainsKey(key))
                {
                    System.Diagnostics.Trace.WriteLine("Key Found: " + _hitPaths[key].Path);
                }
                return _hitList.LastOrDefault().Value;
            }

            if (foundDrawing == null)
            {
                return _hitGroup;
            }

            return foundDrawing;
        }

        private bool HitTestDrawing(GlyphRunDrawing drawing, Point pt)
        {
            GeometryGroup geomGroup = (GeometryGroup)drawing.GlyphRun.BuildGeometry();

            if (drawing.Bounds.Contains(pt) || geomGroup.Bounds.Contains(pt)
                 || geomGroup.FillContains(pt, 1, ToleranceType.Absolute))
            {
                return true;
            }
            return false;
        }

        private bool HitTestDrawing(GeometryDrawing drawing, Point pt)
        {
            Pen pen = drawing.Pen;
            Brush brush = drawing.Brush;
            if (pen != null)
            {
                if (drawing.Geometry.StrokeContains(pen, pt))
                {
                    return true;
                }
                Geometry geometry = drawing.Geometry;

                LineGeometry line = null;
                EllipseGeometry ellipse = null;
                RectangleGeometry rectangle = null;
                PathGeometry path = null;

                if (TryCast.Cast(geometry, out path))
                {
                    if (path.FillContains(pt, 1, ToleranceType.Absolute))
                    {
                        return true;
                    }

                    //PathFigureCollection pathFigures = path.Figures;
                    //int itemCount = pathFigures.Count;
                    //if (itemCount == 1)
                    //{
                    //    if (pathFigures[0].IsClosed && path.FillContains(pt))
                    //    {
                    //        return true;
                    //    }
                    //}
                    //else
                    //{
                    //    for (int f = 0; f < itemCount; f++)
                    //    {
                    //        PathFigure pathFigure = pathFigures[f];
                    //        if (pathFigure.IsClosed)
                    //        {
                    //            PathFigureCollection testFigures = new PathFigureCollection();
                    //            testFigures.Add(pathFigure);

                    //            PathGeometry testPath = new PathGeometry();
                    //            testPath.Figures = testFigures;

                    //            if (testPath.FillContains(pt))
                    //            {
                    //                return true;
                    //            }
                    //        }
                    //    }
                    //}
                }
                else if (TryCast.Cast(geometry, out line))
                {
                    if (line.FillContains(pt))
                    {
                        return true;
                    }
                }
                else if (TryCast.Cast(geometry, out ellipse))
                {
                    if (ellipse.FillContains(pt))
                    {
                        return true;
                    }
                }
                else if (TryCast.Cast(geometry, out rectangle))
                {
                    if (rectangle.FillContains(pt))
                    {
                        return true;
                    }
                }
            }
            else if (brush != null && drawing.Geometry.FillContains(pt))
            {
                return true;
            }
            else if (drawing.Geometry.FillContains(pt))
            {
                return true;
            }

            return false;
        }

        private bool HitTestDrawing(DrawingGroup group, Point pt, out Drawing hitDrawing, bool isText = false)
        {
            hitDrawing = null;
            bool isHit = false;

            if (!group.Bounds.Contains(pt))
            {
                return isHit;
            }
            var transform = group.Transform;
            if (transform != null && !transform.Value.IsIdentity)
            {
                pt = transform.Inverse.Transform(pt);
            }

            var groupHitPath = _hitPath;

            DrawingGroup groupDrawing       = null;
            GlyphRunDrawing glyRunDrawing   = null;
            GeometryDrawing geometryDrawing = null;

            DrawingCollection drawings = group.Children;
            for (int i = drawings.Count - 1; i >= 0; i--)
            {
                Drawing drawing = drawings[i];

//                _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(drawing));

                if (TryCast.Cast(drawing, out geometryDrawing))
                {
                    if (HitTestDrawing(geometryDrawing, pt))
                    {
//                        _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(geometryDrawing));

                        hitDrawing = drawing;
                        int orderNumber = SvgObject.GetOrder(drawing);
                        if (orderNumber >= 0)
                        {
                            _hitList[orderNumber]  = drawing;
                            _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(geometryDrawing));
                            isHit = false;
                        }
                        else
                        {
                            orderNumber = SvgObject.GetOrder(group);
                            if (orderNumber >= 0)
                            {
                                _hitList[orderNumber]  = group;
                                _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(group));
                            }
                            if (!string.IsNullOrWhiteSpace(SvgObject.GetUniqueId(group)))
                            {
                                _hitGroup = group;
                            }
                            return true;
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out groupDrawing))
                {
                    _hitPath = groupHitPath.AddChild(SvgObject.GetUniqueId(groupDrawing));

                    SvgObjectType objectType = SvgObject.GetType(groupDrawing);
//                    if (objectType == SvgObjectType.Text && groupDrawing.Bounds.Contains(pt))
                    if (objectType == SvgObjectType.Text && this.HitTestText(groupDrawing, pt, out hitDrawing))
//                    if (objectType == SvgObjectType.Text)
                    {
                        hitDrawing = drawing;
                        int orderNumber = SvgObject.GetOrder(drawing);
                        if (orderNumber >= 0)
                        {
                            _hitList[orderNumber]  = drawing;
                            _hitPaths[orderNumber] = _hitPath;
                            isHit = false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        //                        _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(hitDrawing));

                        var currentPath = _hitPath;
                        try
                        {
                            if (HitTestDrawing(groupDrawing, pt, out hitDrawing))
                            {
//                                int orderNumber = SvgObject.GetOrder(drawing);
                                int orderNumber = SvgObject.GetOrder(hitDrawing);
                                if (orderNumber >= 0)
                                {
                                    _hitList[orderNumber]  = drawing;
                                    _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(hitDrawing));
                                    isHit = false;
                                }
                                else
                                {
                                    orderNumber = SvgObject.GetOrder(groupDrawing);
                                    if (orderNumber >= 0)
                                    {
                                        _hitList[orderNumber] = groupDrawing;
                                        _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(groupDrawing));
                                    }
                                    return true;
                                }
                            }
                        }
                        finally
                        {
                            _hitPath = currentPath;
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out glyRunDrawing))
                {
                    if (HitTestDrawing(glyRunDrawing, pt))
                    {
//                        _hitPath = _hitPath.AddChild(SvgObject.GetUniqueId(glyRunDrawing));

                        hitDrawing = glyRunDrawing;
                        int orderNumber = SvgObject.GetOrder(drawing);
                        if (orderNumber >= 0)
                        {
                            _hitList[orderNumber] = drawing;
                            _hitPaths[orderNumber] = _hitPath.AddChild(SvgObject.GetUniqueId(glyRunDrawing));
                            isHit = false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
            }
            string uniqueId = SvgObject.GetUniqueId(group);
            if (!string.IsNullOrWhiteSpace(uniqueId))
            {
                _hitGroup = group;
            }

            return isHit;
        }

        private bool HitTestText(DrawingGroup group, Point pt, out Drawing hitDrawing)
        {
            if (!group.Bounds.Contains(pt))
            {
                hitDrawing = null;
                return false;
            }
            if (_textLevel == WpfHitTextLevel.Bounds)
            {
                hitDrawing = group;
                return true;
            }

            return this.HitTestDrawing(group, pt, out hitDrawing);
        }

        #endregion

        #region HitTest Geometry

        private Drawing PerformHitTest(Rect rect, IntersectionDetail detail)
        {
            if (_svgDrawing == null)
            {
                return null;
            }

            var rectDisplay = _displayTransform.TransformBounds(rect);
            var geomDisplay = new RectangleGeometry(rectDisplay);

            DrawingGroup groupDrawing = null;
            GlyphRunDrawing glyRunDrawing = null;
            GeometryDrawing geometryDrawing = null;

            Drawing foundDrawing = null;

            DrawingCollection drawings = _svgDrawing.Children;
            for (int i = drawings.Count - 1; i >= 0; i--)
            {
                Drawing drawing = drawings[i];
                if (TryCast.Cast(drawing, out geometryDrawing))
                {
                    if (HitTestDrawing(geometryDrawing, geomDisplay, detail))
                    {
                        string uniqueId = SvgObject.GetUniqueId(drawing);
                        if (!string.IsNullOrWhiteSpace(uniqueId))
                        {
                            foundDrawing = drawing;
                            break;
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out groupDrawing))
                {
                    if (SvgObject.GetType(groupDrawing) == SvgObjectType.Text)
                    {
                        var textBounds = new RectangleGeometry(groupDrawing.Bounds);
                        if (textBounds.FillContainsWithDetail(geomDisplay) == detail)
                        {
                            string uniqueId = SvgObject.GetUniqueId(drawing);
                            if (!string.IsNullOrWhiteSpace(uniqueId))
                            {
                                foundDrawing = drawing;
                                break;
                            }
                        }
                    }
                    if (HitTestDrawing(groupDrawing, geomDisplay, out foundDrawing, detail))
                    {
                        string uniqueId = SvgObject.GetUniqueId(drawing);
                        if (!string.IsNullOrWhiteSpace(uniqueId))
                        {
                            foundDrawing = drawing;
                            break;
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out glyRunDrawing))
                {
                    if (HitTestDrawing(glyRunDrawing, geomDisplay, detail))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
            }

            return foundDrawing;
        }

        private bool HitTestDrawing(GlyphRunDrawing drawing, Geometry geomDisplay, IntersectionDetail detail)
        {
            if (drawing != null)
            {
                var textBounds = new RectangleGeometry(drawing.Bounds);
                if (textBounds.FillContainsWithDetail(geomDisplay) == detail)
                {
                    return true;
                }
            }

            return false;
        }

        private bool HitTestDrawing(GeometryDrawing drawing, Geometry geomDisplay, IntersectionDetail detail)
        {
            Pen pen = drawing.Pen;
            Brush brush = drawing.Brush;
            if (pen != null && brush == null)
            {
                if (drawing.Geometry.StrokeContainsWithDetail(pen, geomDisplay) == detail)
                {
                    return true;
                }
                Geometry geometry = drawing.Geometry;

                LineGeometry line = null;
                EllipseGeometry ellipse = null;
                RectangleGeometry rectangle = null;
                PathGeometry path = null;
                if (TryCast.Cast(geometry, out path))
                {
                    PathFigureCollection pathFigures = path.Figures;
                    int itemCount = pathFigures.Count;
                    if (itemCount == 1)
                    {
                        if (pathFigures[0].IsClosed && path.FillContainsWithDetail(geomDisplay) == detail)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        for (int f = 0; f < itemCount; f++)
                        {
                            PathFigure pathFigure = pathFigures[f];
                            if (pathFigure.IsClosed)
                            {
                                PathFigureCollection testFigures = new PathFigureCollection();
                                testFigures.Add(pathFigure);

                                PathGeometry testPath = new PathGeometry();
                                testPath.Figures = testFigures;

                                if (testPath.FillContainsWithDetail(geomDisplay) == detail)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
                else if (TryCast.Cast(geometry, out line))
                {
                    if (line.FillContainsWithDetail(geomDisplay) == detail)
                    {
                        return true;
                    }
                }
                else if (TryCast.Cast(geometry, out ellipse))
                {
                    if (ellipse.FillContainsWithDetail(geomDisplay) == detail)
                    {
                        return true;
                    }
                }
                else if (TryCast.Cast(geometry, out rectangle))
                {
                    if (rectangle.FillContainsWithDetail(geomDisplay) == detail)
                    {
                        return true;
                    }
                }
            }
            else if (brush != null && drawing.Geometry.FillContainsWithDetail(geomDisplay) == detail)
            {
                return true;
            }

            return false;
        }

        private bool HitTestDrawing(DrawingGroup group, Geometry geomDisplay, out Drawing hitDrawing, IntersectionDetail detail)
        {
            hitDrawing = null;

            var geomBounds = new RectangleGeometry(group.Bounds);
            if (geomBounds.FillContainsWithDetail(geomDisplay) == detail)
            {
                DrawingGroup groupDrawing = null;
                GlyphRunDrawing glyRunDrawing = null;
                GeometryDrawing geometryDrawing = null;

                DrawingCollection drawings = group.Children;
                for (int i = drawings.Count - 1; i >= 0; i--)
                {
                    Drawing drawing = drawings[i];
                    if (TryCast.Cast(drawing, out geometryDrawing))
                    {
                        if (HitTestDrawing(geometryDrawing, geomDisplay, detail))
                        {
                            hitDrawing = geometryDrawing;
                            return true;
                        }
                    }
                    else if (TryCast.Cast(drawing, out groupDrawing))
                    {
                        SvgObjectType objectType = SvgObject.GetType(groupDrawing);
                        if (objectType == SvgObjectType.Text)
                        {
                            var textBounds = new RectangleGeometry(groupDrawing.Bounds);
                            if (textBounds.FillContainsWithDetail(geomDisplay) == detail)
                            {
                                hitDrawing = groupDrawing;
                                return true;
                            }
                        }
                        if (HitTestDrawing(groupDrawing, geomDisplay, out hitDrawing, detail))
                        {
                            return true;
                        }
                    }
                    else if (TryCast.Cast(drawing, out glyRunDrawing))
                    {
                        if (HitTestDrawing(glyRunDrawing, geomDisplay, detail))
                        {
                            hitDrawing = glyRunDrawing;
                            return true;
                        }
                    }
                }

                return true;
            }

            return false;
        }

        #endregion

        #region Enumerate Drawing

        private DrawingGroup GetDrawingLayer(DrawingGroup drawingGroup, ref bool isFound)
        {
            isFound = false;

            DrawingGroup groupDrawing = null;

            // Enumerate the drawings in the DrawingCollection.
            foreach (Drawing drawing in drawingGroup.Children)
            {
                var itemKey = SvgLink.GetKey(drawing);
                if (!string.IsNullOrWhiteSpace(itemKey) &&
                    itemKey.Equals(SvgObject.DrawLayer, StringComparison.OrdinalIgnoreCase))
                {
                    isFound = true;
                    return (DrawingGroup)drawing;
                }

                // If the drawing is a DrawingGroup, call the function recursively.
                if (TryCast.Cast(drawing, out groupDrawing))
                {
                    SvgObjectType objectType = SvgObject.GetType(groupDrawing);
                    if (objectType != SvgObjectType.Text)
                    {
                        var nextGroup = GetDrawingLayer(groupDrawing, ref isFound);
                        if (nextGroup != groupDrawing)
                        {
                            itemKey = SvgLink.GetKey(nextGroup);
                            if (!string.IsNullOrWhiteSpace(itemKey) &&
                                itemKey.Equals(SvgObject.DrawLayer, StringComparison.OrdinalIgnoreCase))
                            {
                                return nextGroup;
                            }
                        }
                    }
                }
            }
            return drawingGroup;
        }

        private void EnumerateDrawing()
        {
            if (_svgDocument == null || _svgDrawing == null)
            {
                return;
            }
            _idMap   = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            _guidMap = new Dictionary<string, Drawing>(StringComparer.Ordinal);

            bool isFound = false;

            this.EnumDrawingGroup(GetDrawingLayer(_svgDrawing, ref isFound));

            _isEnumerated = true;            
        }

        // Enumerate the drawings in the DrawingGroup.
        private void EnumDrawingGroup(DrawingGroup drawingGroup)
        {
            // Enumerate the drawings in the DrawingCollection.
            foreach (Drawing drawing in drawingGroup.Children)
            {
                string objectId = SvgObject.GetId(drawing);
                if (!string.IsNullOrWhiteSpace(objectId))
                {
                    _idMap[objectId] = drawing;
                }
                string objectName = (string)drawing.GetValue(FrameworkElement.NameProperty);
                if (!string.IsNullOrWhiteSpace(objectName))
                {
                    _idMap[objectName] = drawing;
                }
                string uniqueId = SvgObject.GetUniqueId(drawing);
                if (!string.IsNullOrWhiteSpace(uniqueId))
                {
                    _guidMap[uniqueId] = drawing;
                }

                DrawingGroup groupDrawing = null;

                // If the drawing is a DrawingGroup, call the function recursively.
                if (TryCast.Cast(drawing, out groupDrawing))
                {
                    SvgObjectType objectType = SvgObject.GetType(groupDrawing);

                    if (objectType != SvgObjectType.Text)
                    {
                        EnumDrawingGroup(groupDrawing);
                    }
                }
            }
        }

        #endregion

        #endregion
    }
}
