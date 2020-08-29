using System;

namespace SharpVectors.Runtime
{
    /// <summary>
    /// <para>
    /// A value specifying the type of interactivity to be supported by the conversion process
    /// and controls.
    /// </para>
    /// <para>
    /// In the conversion process, more information are added to make the interactivity 
    /// defined possible, and might be unnecessary overhead where no interactivity is needed.
    /// </para>
    /// <para>
    /// In the rendering process at the control level, keyboard and mouse operations are monitored
    /// to support interactivities.
    /// </para>
    /// </summary>
    public enum SvgInteractiveModes
    {
        /// <summary>
        /// A value specifying no interactivity.
        /// </summary>
        None     = 0,
        /// <summary>
        /// A value specifying the default or standard interactivity per the SVG specifications.
        /// </summary>
        Standard = 1,
        /// <summary>
        /// A value specifying extra or custom interactivity features.
        /// </summary>
        Advanced = 2
    }
}
