using System;

namespace SharpVectors.Dom.Svg
{
	public abstract class SvgPathSeg : ISvgPathSeg
    {
        #region Private Fields

        private int _index;
        private SvgPathSegList _list;
        private SvgPathSegType _type;

        #endregion

        #region Constructors

        protected SvgPathSeg(SvgPathSegType type)
		{
			this._type = type;
		}

		#endregion

        #region Public Properties

        public abstract string PathText { get; }
        public abstract SvgPointF AbsXY { get; }
        public abstract double StartAngle { get; }
        public abstract double EndAngle { get; }

        public SvgPathSeg PreviousSeg
        {
            get
            {
                return _list.GetPreviousSegment(this);
            }
        }

        public SvgPathSeg NextSeg
        {
            get
            {
                return _list.GetNextSegment(this);
            }
        }

        public int Index
        {
            get
            {
                return _index;
            }
        }

        public virtual double Length
        {
            get
            {
                return 0;
            }
        }

        #endregion

        #region Internal Methods

        internal void SetList(SvgPathSegList list)
		{
            this._list = list;
		}

		internal void SetIndex(int index)
		{
			this._index = index;
		}

		internal void SetIndexWithDiff(int diff)
		{
			this._index += diff;
        }

        #endregion

        #region ISvgPathSeg Members

        public SvgPathSegType PathSegType
		{
			get 
            {
                return _type;
            }
		}

		public string PathSegTypeAsLetter	
		{
			get
			{
				switch(_type)
				{
					case SvgPathSegType.ArcAbs:
						return "A";
					case SvgPathSegType.ArcRel:
						return "a";
					case SvgPathSegType.ClosePath:
						return "z";
					case SvgPathSegType.CurveToCubicAbs:
						return "C";
					case SvgPathSegType.CurveToCubicRel:
						return "c";
					case SvgPathSegType.CurveToCubicSmoothAbs:
						return "S";
					case SvgPathSegType.CurveToCubicSmoothRel:
						return "s";
					case SvgPathSegType.CurveToQuadraticAbs:
						return "Q";
					case SvgPathSegType.CurveToQuadraticRel:
						return "q";
					case SvgPathSegType.CurveToQuadraticSmoothAbs:
						return "T";
					case SvgPathSegType.CurveToQuadraticSmoothRel:
						return "t";
					case SvgPathSegType.LineToAbs:
						return "L";
					case SvgPathSegType.LineToHorizontalAbs:
						return "H";
					case SvgPathSegType.LineToHorizontalRel:
						return "h";
					case SvgPathSegType.LineToRel:
						return "l";
					case SvgPathSegType.LineToVerticalAbs:
						return "V";
					case SvgPathSegType.LineToVerticalRel:
						return "v";
					case SvgPathSegType.MoveToAbs:
						return "M";
					case SvgPathSegType.MoveToRel:
						return "m";
					default:
						return String.Empty;
				}
			}
		}

		#endregion
	}
}
