using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Collections.Generic;

namespace SharpVectors.Runtime
{
    //This class, if placed at the root of a XAML file which is loaded by XamlReader.Load()
    //will end up having all named elements contained in its nameTable automatically...
    //
    /// <summary>
    /// This class, if placed at the root of a XAML file which is loaded by XamlReader.Load()
    /// will end up having all named elements contained in its nameTable automatically.
    /// If you want to get that list, it is now in your power.
    /// </summary>
    /// <remarks>
    /// This class is based on the discussion, which can be found here
    /// <see href="http://social.msdn.microsoft.com/Forums/en-US/wpf/thread/5c226430-c54d-45b8-a8a2-7e4a79e3692a"/>
    /// </remarks>
    public sealed class SvgImage : Image, INameScope
    {
        #region Private Fields

        private Dictionary<string, object> _nameTable;

        #endregion

        #region Constructors and Destructor

        public SvgImage()
        {
            _nameTable = new Dictionary<string, object>();
        }

        #endregion

        #region INameScope Members

        object INameScope.FindName(string name)
        {
            object element = null;
            _nameTable.TryGetValue(name, out element);

            return element;
        }

        void INameScope.RegisterName(string name, object scopedElement)
        {
            _nameTable[name] = scopedElement;

            DependencyObject namedObject = scopedElement as DependencyObject;

            if (namedObject != null)
            {   
                namedObject.SetValue(FrameworkElement.NameProperty, name);
            }
        }

        void INameScope.UnregisterName(string name)
        {
            _nameTable[name] = null;
        }

        #endregion
    }
}
