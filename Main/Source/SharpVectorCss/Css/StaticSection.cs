
using System;

namespace SharpVectors.Dom.Css
{
    /// <summary>
    /// Creates a static section for a CssXmlDocument.
    /// Typical use:
    /// using(StaticSection.Use(doc))
    /// {
    ///     // blah blah
    /// }
    /// </summary>
    public sealed class StaticSection : IDisposable
    {
        /// <summary>
        /// Previous Static state
        /// </summary>
        private bool _previousStatic;
        /// <summary>
        /// Document to be handled
        /// </summary>
        private CssXmlDocument _cssXmlDocument;

        /// <summary>
        /// Initializes a new instance of the <see cref="StaticSection"/> class.
        /// </summary>
        /// <param name="cssXmlDocument">The CSS XML document.</param>
        private StaticSection(CssXmlDocument cssXmlDocument)
        {
            _cssXmlDocument = cssXmlDocument;
            _previousStatic = cssXmlDocument.Static;
            cssXmlDocument.Static = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _cssXmlDocument.Static = _previousStatic;
        }

        /// <summary>
        /// Uses the specified CSS XML document with Static state to true.
        /// </summary>
        /// <param name="cssXmlDocument">The CSS XML document.</param>
        public static IDisposable Use(CssXmlDocument cssXmlDocument)
        {
            return new StaticSection(cssXmlDocument);
        }
    }
}
