using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

using SharpVectors.Dom.Css;
using SharpVectors.Dom.Svg;
using SharpVectors.Dom.Stylesheets;

namespace SharpVectors.Csss.Tests
{
    internal static class CssHelper
    {
        // For simple property tests (read-write, author stylesheet)
        public static CssStyleDeclaration CreateDeclaration(string cssText = "")
        {
            string css = cssText;
            return new CssStyleDeclaration(ref css, null, false, CssStyleSheetType.Author);
        }

        // For read-only declarations (simulating user agent stylesheets)
        public static CssStyleDeclaration CreateReadOnlyDeclaration(string cssText = "")
        {
            string css = cssText;
            return new CssStyleDeclaration(ref css, null, true, CssStyleSheetType.UserAgent);
        }

        // For inline styles
        public static CssStyleDeclaration CreateInlineDeclaration(string cssText = "")
        {
            string css = cssText;
            return new CssStyleDeclaration(ref css, null, false, CssStyleSheetType.Inline);
        }



        //private static CssStyleSheet CreateStyleSheetOld()
        //{
        //    var doc = new XmlDocument();
        //    return new CssStyleSheet(doc, null, null, null, null, CssStyleSheetType.Author);
        //}

        public static CssStyleSheet CreateStyleSheet()
        {
            // 1. The inline SVG test string
            string svgTemplate = @"<svg xmlns='http://www.w3.org/2000/svg'>
                        <style type='text/css'>
                        </style>
                      </svg>";

            // 2. Initialize the document and load the string layout
            SvgDocument svgDoc = new SvgDocument(TestSvgWindow.Create());
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(svgTemplate)))
            {
                svgDoc.Load(stream);
            }

            // Query the parsed stylesheet directly from the document collection
            IStyleSheetList sheets = svgDoc.StyleSheets;

            // Grab the first parsed sheet in the layout context
            CssStyleSheet testSheet = (CssStyleSheet)sheets[0];

            return testSheet;
        }

        public static CssStyleSheet CreateStyleSheet(string styles)
        {
            // 1. The inline SVG test string
            string svgTemplate = string.Format(@"<svg xmlns='http://www.w3.org/2000/svg'>
                        <style type='text/css'>
                        {0}
                        </style>
                      </svg>", styles);

            // 2. Initialize the document and load the string layout
            SvgDocument svgDoc = new SvgDocument(TestSvgWindow.Create());
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(svgTemplate)))
            {
                svgDoc.Load(stream);
            }

            // Query the parsed stylesheet directly from the document collection
            IStyleSheetList sheets = svgDoc.StyleSheets;

            // Grab the first parsed sheet in the layout context
            CssStyleSheet testSheet = (CssStyleSheet)sheets[0];

            return testSheet;
        }

        /// <summary>
        /// Creates a stylesheet with diagnostics context enabled so that parsing errors,
        /// warnings, and selector analysis results can be inspected.
        /// NOTE: Due to lazy evaluation of CssRules, this creates the stylesheet and then
        /// attaches diagnostics. The CSS is already parsed by SvgDocument.Load, so 
        /// diagnostics will only capture errors from subsequent access/re-parsing.
        /// </summary>
        /// <param name="styles">The CSS content to parse</param>
        /// <returns>A stylesheet with diagnostics tracking enabled</returns>
        public static CssStyleSheet CreateStyleSheetWithDiagnostics(string styles, out CssParsingContext diagnostics)
        {
            // 1. Create and initialize diagnostics context BEFORE stylesheet creation
            diagnostics = new CssParsingContext();
            diagnostics.StartTracking();

            // 2. Create the stylesheet 
            // NOTE: SvgDocument.Load() will parse CSS, but without diagnostics context
            // because the context is created after Load() completes
            string svgTemplate = string.Format(@"<svg xmlns='http://www.w3.org/2000/svg'>
                        <style type='text/css'>
                        {0}
                        </style>
                      </svg>", styles);

            SvgDocument svgDoc = new SvgDocument(TestSvgWindow.Create());
            using (MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(svgTemplate)))
            {
                svgDoc.Load(stream);
            }

            IStyleSheetList sheets = svgDoc.StyleSheets;
            CssStyleSheet testSheet = (CssStyleSheet)sheets[0];

            // 3. Now attach diagnostics context for any subsequent parsing
            testSheet.ParsingContext = diagnostics;

            // 4. Access CssRules to trigger any additional parsing with diagnostics enabled
            var rules = testSheet.CssRules;

            diagnostics.StopTracking();
            return testSheet;
        }

        /// <summary>
        /// Creates a stylesheet with diagnostics context enabled without returning the context.
        /// Use this when you only care about parsing side effects, not detailed diagnostics.
        /// </summary>
        /// <param name="styles">The CSS content to parse</param>
        /// <returns>A stylesheet with diagnostics tracking enabled</returns>
        public static CssStyleSheet CreateStyleSheetWithDiagnostics(string styles)
        {
            CssParsingContext unused;
            return CreateStyleSheetWithDiagnostics(styles, out unused);
        }


    }
}
