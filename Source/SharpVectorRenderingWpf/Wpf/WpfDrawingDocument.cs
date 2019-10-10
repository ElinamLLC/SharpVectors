using System;
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

        private Transform _displayTransform;

        private bool _isEnumerated;
        private SvgDocument _svgDocument;
        private DrawingGroup _svgDrawing;
        private IDictionary<string, Drawing> _idMap;
        private IDictionary<string, Drawing> _guidMap;

        private DrawingGroup _hitGroup;

        #endregion

        #region Constructors and Destructor

        public WpfDrawingDocument()
        {
            _idMap   = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            _guidMap = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            _displayTransform = Transform.Identity;
        }

        #endregion

        #region Public Properties

        public Transform DisplayTransform
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
                return new WpfHitTestResult(point, null, svgDrawing);
//                return WpfHitTestResult.Empty;
            }
            var svgElement = this.GetSvgByUniqueId(uniqueId);
            if (svgElement == null)
            {
                return WpfHitTestResult.Empty;
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
                return WpfHitTestResult.Empty;
            }

            return new WpfHitTestResult(rect, svgElement, svgDrawing);
        }

        #endregion

        #region Private Methods

        #region HitTest Point

        private Drawing PerformHitTest(Point pt)
        {
            _hitGroup = null;

            if (_svgDrawing == null)
            {
                return null;
            }

            Point ptDisplay = _displayTransform.Transform(pt);

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
                    if (HitTestDrawing(geometryDrawing, ptDisplay))
                    {
                        string uniqueId = SvgObject.GetUniqueId(geometryDrawing);
                        if (!string.IsNullOrWhiteSpace(uniqueId))
                        {
                            foundDrawing = drawing;
                            break;
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out groupDrawing))
                {
                    if (SvgObject.GetType(groupDrawing) == SvgObjectType.Text &&
                        groupDrawing.Bounds.Contains(ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                    if (HitTestDrawing(groupDrawing, ptDisplay, out foundDrawing))
                    {
                        string uniqueId = SvgObject.GetUniqueId(foundDrawing);
                        if (!string.IsNullOrWhiteSpace(uniqueId))
                        {
                            //foundDrawing = drawing;
                            break;
                        }
                    }
                }
                else if (TryCast.Cast(drawing, out glyRunDrawing))
                {
                    if (HitTestDrawing(glyRunDrawing, ptDisplay))
                    {
                        foundDrawing = drawing;
                        break;
                    }
                }
            }

            if (foundDrawing == null)
            {
                return _hitGroup;
            }

            return foundDrawing;
        }

        private bool HitTestDrawing(GlyphRunDrawing drawing, Point pt)
        {
            if (drawing != null && drawing.Bounds.Contains(pt))
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

        private bool HitTestDrawing(DrawingGroup group, Point pt, out Drawing hitDrawing)
        {
            hitDrawing = null;

            if (group.Bounds.Contains(pt))
            {
                DrawingGroup groupDrawing       = null;
                GlyphRunDrawing glyRunDrawing   = null;
                GeometryDrawing geometryDrawing = null;

                DrawingCollection drawings = group.Children;
                for (int i = drawings.Count - 1; i >= 0; i--)
                {
                    Drawing drawing = drawings[i];
                    if (TryCast.Cast(drawing, out geometryDrawing))
                    {
                        if (HitTestDrawing(geometryDrawing, pt))
                        {
                            hitDrawing = drawing;
                            return true;
                        }
                    }
                    else if (TryCast.Cast(drawing, out groupDrawing))
                    {
                        SvgObjectType objectType = SvgObject.GetType(groupDrawing);
                        //if (objectType == SvgObjectType.Text && groupDrawing.Bounds.Contains(pt))
                        if (objectType == SvgObjectType.Text)
                        {
                            hitDrawing = drawing;
                            return true;
                        }
                        if (HitTestDrawing(groupDrawing, pt, out hitDrawing))
                        {
                            return true;
                        }
                    }
                    else if (TryCast.Cast(drawing, out glyRunDrawing))
                    {
                        if (HitTestDrawing(glyRunDrawing, pt))
                        {
                            hitDrawing = glyRunDrawing;
                            return true;
                        }
                    }
                }
                string uniqueId = SvgObject.GetUniqueId(group);
                if (!string.IsNullOrWhiteSpace(uniqueId))
                {
                    _hitGroup = group;
                }
            }

            return false;
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

        private void EnumerateDrawing()
        {
            if (_svgDocument == null || _svgDrawing == null)
            {
                return;
            }
            _idMap   = new Dictionary<string, Drawing>(StringComparer.Ordinal);
            _guidMap = new Dictionary<string, Drawing>(StringComparer.Ordinal);

            this.EnumDrawingGroup(_svgDrawing);

            _isEnumerated = true;            
        }

        // Enumerate the drawings in the DrawingGroup.
        private void EnumDrawingGroup(DrawingGroup drawingGroup)
        {
            DrawingCollection dc = drawingGroup.Children;

            DrawingGroup groupDrawing       = null;

            // Enumerate the drawings in the DrawingCollection.
            foreach (Drawing drawing in dc)
            {
                string objectId = SvgObject.GetId(drawing);
                if (!string.IsNullOrWhiteSpace(objectId))
                {
                    _idMap.Add(objectId, drawing);
                }
                string objectName = (string)drawing.GetValue(FrameworkElement.NameProperty);
                if (!string.IsNullOrWhiteSpace(objectName))
                {
                    _idMap[objectName] = drawing;
                }
                string uniqueId = SvgObject.GetUniqueId(drawing);
                if (!string.IsNullOrWhiteSpace(uniqueId))
                {
                    _guidMap.Add(uniqueId, drawing);
                }

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
