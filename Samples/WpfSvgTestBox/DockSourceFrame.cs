using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

using YDock;
using YDock.Enum;
using YDock.Interface;

namespace WpfSvgTestBox
{
    public class DockSourceFrame : Frame, IDockSource
    {
        #region Protected Fields

        protected string _dockHeader;
        protected ImageSource _dockIcon;
        protected IDockControl _dockControl;

        #endregion

        #region Constructors and Destructor

        public DockSourceFrame()
        {
            _dockHeader = string.Empty;
        }

        #endregion

        #region IDockSource Members

        public IDockControl DockControl
        {
            get {
                return _dockControl;
            }
            set {
                _dockControl = value;
            }
        }

        public string Header
        {
            get {
                return _dockHeader;
            }
            protected set {
                _dockHeader = value;
            }
        }

        public ImageSource Icon
        {
            get {
                return _dockIcon;
            }
            protected set {
                _dockIcon = value;
            }
        }

        #endregion
    }
}
