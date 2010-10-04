using System;
using System.Collections;
using System.Text.RegularExpressions;

namespace SharpVectors.Dom.Svg
{
	/// <summary>
	/// Summary description for SvgTransformList.
	/// </summary>
    public sealed class SvgTransformList : SvgList<ISvgTransform>, ISvgTransformList
    {
        #region Private Fields

        //Regex re = new Regex("([A-Za-z]+)\\s*\\(([\\-0-9\\.\\,\\seE]+)\\)");
			
        private static Regex _regExtract = new Regex(
            "([A-Za-z]+)\\s*\\(([\\-0-9\\.eE\\,\\s]+)\\)", RegexOptions.Compiled);

        #endregion

        #region Constructors

        public SvgTransformList()
        {
        }

        public SvgTransformList(string listString)
        {
            this.FromString(listString);
        }

        #endregion

        #region Public Properties

        public SvgMatrix TotalMatrix
        {
            get
            {
                if (NumberOfItems == 0)
                {
                    return SvgMatrix.Identity;
                }
                else
                {
                    SvgMatrix matrix = (SvgMatrix)GetItem(0).Matrix;

                    for (uint i = 1; i < NumberOfItems; i++)
                    {
                        matrix = (SvgMatrix)matrix.Multiply(GetItem(i).Matrix);
                    }

                    return matrix;
                }
            }
        }

        #endregion

        #region Public Methods

        public void FromString(string listString)
        {
            Clear();

            if (!String.IsNullOrEmpty(listString))
            {
                Match match = _regExtract.Match(listString);
                while (match.Success)
                {
                    this.AppendItem(new SvgTransform(match.Value));
                    match = match.NextMatch();
                }
            }
        }

        #endregion

        #region ISvgTransformList Members

        public ISvgTransform CreateSvgTransformFromMatrix(ISvgMatrix matrix)
        {
            return new SvgTransform((SvgMatrix)matrix);
        }

        public ISvgTransform Consolidate()
        {
            ISvgTransform result = CreateSvgTransformFromMatrix(TotalMatrix);
                    
            Initialize(result);

            return result;
        }
		#endregion
    }
}
