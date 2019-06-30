//--------------------------------------------------------------------------
// Source: https://www.codeproject.com/Articles/30625/Circular-Progress-Indicator
// Copyright c Emilio Simoes 2008-2010
//--------------------------------------------------------------------------

using System;

namespace ProgressControls
{
    /// <summary>
    /// This enum is used to choose what text should be shown in the control.
    /// </summary>
    [Flags]
    public enum TextDisplayModes
    {
        /// <summary>
        /// No text will be shown by the control.
        /// </summary>
        None = 0,

        /// <summary>
        /// The control will show the value of the Percentage property.
        /// </summary>
        Percentage = 1,

        /// <summary>
        /// The control will show the value of the Text property.
        /// </summary>
        Text = 2,

        /// <summary>
        /// The control will show the values of the Text and Percentage properties.
        /// </summary>
        Both = Percentage | Text
    }
}